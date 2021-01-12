using System;
using System.Collections.Generic;
using UnityEngine;

public class OutputKnob : Knob
{
	internal readonly Dictionary<InputKnob, Line> Connections = new Dictionary<InputKnob, Line>();

	protected override void LeftClick()
	{
		foreach (var connection in Connections)
		{
			connection.Key.IsActive = !IsActive;
			connection.Value.SetActive(!IsActive);
		}

		IsActive = !IsActive;
	}
}