using System;
using UnityEngine;

public class Line : MonoBehaviour
{
	private LineRenderer _lineRenderer;

	private InputKnob _parent;

	private const int Smoothness = 5;

	private bool _isActive;

	private void Start()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	public static Line Create(Vector3 from, Vector3 to)
	{
		GameObject gameObject = new GameObject("Line");

		gameObject.layer = 8;

		gameObject.transform.parent = Constants.C.lineParent;

		var line = gameObject.AddComponent<Line>();
		var lineRenderer = gameObject.AddComponent<LineRenderer>();

		line._lineRenderer = lineRenderer;

		lineRenderer.widthMultiplier = .2f;
		lineRenderer.material = new Material(Constants.C.lineMaterial) {color = Constants.C.knobColor};

		lineRenderer.positionCount = 2;

		lineRenderer.numCornerVertices = Smoothness;
		
		lineRenderer.SetPositions(new []{GetLinePos(from), GetLinePos(to)});

		return line;
	}
	
	public static Line Create(Line lineToCopy, InputKnob knobParent)
	{
		GameObject gameObject = new GameObject("Line");

		gameObject.layer = 8;

		gameObject.transform.parent = lineToCopy.transform.parent;

		var line = gameObject.AddComponent<Line>();
		var lineRenderer = gameObject.AddComponent<LineRenderer>();

		line._lineRenderer = lineRenderer;

		line._parent = knobParent;

		lineRenderer.widthMultiplier = .2f;
		lineRenderer.material = new Material(Constants.C.lineMaterial) {color = Constants.C.knobColor};
		
		lineRenderer.numCornerVertices = Smoothness;

		lineRenderer.positionCount = lineToCopy._lineRenderer.positionCount;
		
		Vector3[] positions = new Vector3[lineRenderer.positionCount];

		lineToCopy._lineRenderer.GetPositions(positions);
		
		lineRenderer.SetPositions(positions);

		var meshCollider = gameObject.AddComponent<MeshCollider>();
		
		var mesh = new Mesh();
		
		lineRenderer.BakeMesh(mesh, true);

		meshCollider.sharedMesh = mesh;

		return line;
	}

	public void SetActive(bool active)
	{
		_isActive = active;
		_lineRenderer.material.color = active ? Constants.C.knobActiveColor : Constants.C.knobColor;
	}

	public void UpdatePosition(Vector3 from, Vector3 to)
	{
		_lineRenderer.SetPositions(new []{GetLinePos(from), GetLinePos(to)});
	}
	
	public void UpdatePosition(int index, Vector3 pos)
	{
		_lineRenderer.SetPosition(index, GetLinePos(pos));
	}

	public void AddAfter(Vector3 pos)
	{
		int index = _lineRenderer.positionCount - 1;
		
		Vector3[] positions = new Vector3[_lineRenderer.positionCount];
		
		_lineRenderer.GetPositions(positions);
		
		Vector3[] newPositions = new Vector3[_lineRenderer.positionCount + 1];

		for (int i = 0; i < _lineRenderer.positionCount + 1; i++)
		{
			if (i == index)
			{
				newPositions[i] = GetLinePos(pos);
				continue;
			}

			if (i >= index)
			{
				newPositions[i] = GetLinePos(positions[i - 1]);
				continue;
			}

			newPositions[i] = GetLinePos(positions[i]);
		}

		_lineRenderer.positionCount += 1;
		_lineRenderer.SetPositions(newPositions);
	}

	public int GetNumPos()
	{
		return _lineRenderer.positionCount;
	}

	public void ResetLine()
	{
		Vector3 firstPos = _lineRenderer.GetPosition(0);

		_lineRenderer.positionCount = 2;
		_lineRenderer.SetPositions(new []{firstPos, firstPos});
	}

	public Vector3[] GetAllPositions()
	{
		Vector3[] array = new Vector3[_lineRenderer.positionCount];

		_lineRenderer.GetPositions(array);

		return array;
	}

	public void SetColor(Color color)
	{
		_lineRenderer.material.color = color;
	}

	private void OnMouseDown()
	{
		if (Constants.GetSelectedKnob() == null)
		{
			_parent.Parent = null;
		}
	}

	private void OnMouseEnter()
	{
		if (Constants.GetSelectedKnob() == null)
		{
			_lineRenderer.material.color = Constants.C.knobSelectedColor;
		}
	}

	private void OnMouseExit()
	{
		_lineRenderer.material.color = _isActive ? Constants.C.knobActiveColor : Constants.C.knobColor;
	}

	private static Vector3 GetLinePos(Vector3 vector3)
	{
		return new Vector3(vector3.x, vector3.y, 1);
	}
}