using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Knob : MonoBehaviour
{
	private bool _isActive;
	private bool _isSelected;
	private SpriteRenderer _spriteRenderer;

	private Vector3 _offset;
	private bool _isMoving;

	internal Line LineDraw;

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			_isSelected = value;

			_spriteRenderer.color = _isSelected ? Constants.C.knobSelectedColor : _isActive ? Constants.C.knobActiveColor : Constants.C.knobColor;

			SetSelectedExtension();
		}
	}

	public bool IsActive
	{
		get => _isActive;
		set
		{
			_isActive = value;

			_spriteRenderer.color = _isActive ? Constants.C.knobActiveColor : _isSelected ? Constants.C.knobSelectedColor : Constants.C.knobColor;

			SetActiveExtension();
		}
	}

	protected virtual void SetSelectedExtension()
	{
	}

	protected virtual void SetActiveExtension()
	{
	}

	private void Start()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();

		_spriteRenderer.color = Constants.C.knobColor;

		Constants.C.knobs.Add(this);

		LineDraw = Line.Create(transform.position, transform.position, false);

		ExtendedStart();
	}

	protected virtual void ExtendedStart()
	{
	}

	private readonly List<Vector3> _currentLineSnapPositions = new List<Vector3>();
	internal readonly List<Vector3> LineSnapPositions = new List<Vector3>();

	private void Update()
	{
		if (_isMoving)
		{
			if (Input.GetMouseButton(2))
			{
				MoveToCursor();

				if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
				{
					Delete();
				}
			}
			else
			{
				_isMoving = false;
			}
		}

		if (IsSelected)
		{
			if (Input.GetMouseButton(1))
			{
				var worldPosOfMouse = Physics.GetWorldPosOfMouse();
				
				
				
				if (Input.GetKey(KeyCode.LeftControl))
				{
					var array = Constants.C.knobs.Select(t => t.transform.position).Concat(_currentLineSnapPositions).Concat(LineSnapPositions);
					
					foreach (var knob in array)
					{
						if (Math.Abs(knob.x - worldPosOfMouse.x) < .25f)
						{
							worldPosOfMouse = new Vector3(knob.x, worldPosOfMouse.y, worldPosOfMouse.z);
						}

						if (Math.Abs(knob.y - worldPosOfMouse.y) < .25f)
						{
							worldPosOfMouse = new Vector3(worldPosOfMouse.x, knob.y, worldPosOfMouse.z);
						}
					}
				}

				LineDraw.UpdatePosition(0, transform.position);
				LineDraw.UpdatePosition(LineDraw.GetNumPos() - 1, worldPosOfMouse);
				LineDraw.SetActive(IsActive);
				
				LineDraw.SetColor(Constants.C.knobSelectedColor);

				if (Input.GetMouseButtonDown(0))
				{
					LineDraw.AddAfter(worldPosOfMouse);
					_currentLineSnapPositions.Add(worldPosOfMouse);
				}
			}
			else
			{
				var array = LineDraw.GetAllPositions();
				foreach (var position in array)
				{
					_currentLineSnapPositions.Remove(position);
				}
				
				LineDraw.ResetLine();
				IsSelected = false;
			}
		}
	}

	private void Delete()
	{
		if (GetType() == typeof(OutputKnob))
		{
			OutputKnob outputKnob = (OutputKnob) this;
			
			List<InputKnob> inputKnobs = new List<InputKnob>();
			
			foreach (var connection in outputKnob.Connections)
			{
				Destroy(connection.Value.gameObject);
				inputKnobs.Add(connection.Key);
			}
			
			inputKnobs.ForEach(x => x.Parent = null);
		}
		else
		{
			InputKnob inputKnob = (InputKnob) this;

			inputKnob.Parent = null;
		}
		
		Destroy(LineDraw.gameObject);
		
		Destroy(gameObject);
	}

	private void MoveToCursor()
	{
		var snap = Input.GetKey(KeyCode.LeftControl);
		var mouse = Physics.GetWorldPosOfMouse();
		var worldPosOfMouse = mouse - _offset;

		if (snap)
		{
			for (int i = 0; i < Constants.C.knobs.Count; i++)
			{
				var knob = Constants.C.knobs[i].transform.position;

				if (Constants.C.knobs[i] == this)
				{
					continue;
				}

				if (Math.Abs(knob.x - mouse.x) < .25f)
				{
					worldPosOfMouse = new Vector3(knob.x, worldPosOfMouse.y, worldPosOfMouse.z);
				}

				if (Math.Abs(knob.y - mouse.y) < .25f)
				{
					worldPosOfMouse = new Vector3(worldPosOfMouse.x, knob.y, worldPosOfMouse.z);
				}
			}
		}

		if (GetType() == typeof(OutputKnob))
		{
			OutputKnob outputKnob = (OutputKnob) this;
			foreach (var outputKnobConnection in outputKnob.Connections)
			{
				outputKnobConnection.Value.UpdatePosition(0, outputKnob.transform.position);
			}
		}
		else
		{
			InputKnob inputKnob = (InputKnob) this;
			if (inputKnob.Parent != null)
			{
				var parentConnection = inputKnob.Parent.Connections[inputKnob];
				parentConnection.UpdatePosition(parentConnection.GetNumPos() - 1, inputKnob.transform.position);
			}
		}

		transform.position = worldPosOfMouse;
	}

	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			LeftClick();
		}

		if (Input.GetMouseButtonDown(1))
		{
			IsSelected = true;
		}

		if (Input.GetMouseButtonDown(2))
		{
			_isMoving = true;
			_offset = Physics.GetWorldPosOfMouse() - transform.position;
		}

		if (Input.GetMouseButtonUp(1))
		{
			var selectedKnob = Constants.GetSelectedKnob();

			if (selectedKnob != null && selectedKnob != this)
			{
				if (GetKnobTypes(this, selectedKnob, out var inputKnob, out var outputKnob))
				{
					inputKnob.Parent = null;
					
					inputKnob.Parent = outputKnob;
				}
			}
		}
	}

	protected virtual void LeftClick()
	{
	}

	private bool GetKnobTypes(Knob knob1, Knob knob2, out InputKnob inputKnob, out OutputKnob outputKnob)
	{
		inputKnob = null;
		outputKnob = null;

		bool isInput;

		if (knob1.GetType() != typeof(InputKnob) && knob1.GetType() != typeof(OutputKnob)) return false;
		if (knob2.GetType() != typeof(InputKnob) && knob2.GetType() != typeof(OutputKnob)) return false;

		if (knob1.GetType() == typeof(InputKnob))
		{
			inputKnob = (InputKnob) knob1;
			isInput = true;
		}
		else
		{
			outputKnob = (OutputKnob) knob1;
			isInput = false;
		}

		if (knob2.GetType() == typeof(OutputKnob))
		{
			if (!isInput)
			{
				return false;
			}

			outputKnob = (OutputKnob) knob2;
		}
		else
		{
			if (inputKnob)
			{
				return false;
			}

			inputKnob = (InputKnob) knob2;
		}

		return true;
	}
}