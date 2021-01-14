using UnityEngine;

public class InputKnob : Knob
{
	private OutputKnob _parent;

	public OutputKnob Parent
	{
		get => _parent;
		set
		{
			if (value != null)
			{
				var line = Line.Create(value.LineDraw, this, true);
				value.Connections.Add(this, line);

				var allPositions = line.GetAllPositions();

				foreach (var position in allPositions)
				{
					value.LineSnapPositions.Add(position);
				}

				line.SetActive(value.IsActive);
				
				line.UpdatePosition(line.GetNumPos() - 1, transform.position);
				
				IsActive = value.IsActive;
			}
			else
			{
				IsActive = false;
			}
			
			if (_parent != null)
			{
				var line = _parent.Connections[this];

				var allPositions = line.GetAllPositions();

				foreach (var position in allPositions)
				{
					_parent.LineSnapPositions.Remove(position);
				}

				Destroy(line.gameObject);
				
				_parent.Connections.Remove(this);
			}

			_parent = value;
		}
	}
}