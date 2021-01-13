using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Constants : MonoBehaviour
{
	public static Constants C;

	public List<Knob> knobs = new List<Knob>();
	
	public Color knobActiveColor;
	public Color knobSelectedColor;
	public Color knobColor;

	public Material lineMaterial;

	public Transform lineParent;

	private void Awake()
	{
		C = this;
	}

	public static Knob GetSelectedKnob()
	{
		var list = C.knobs.Where(x => x.IsSelected).ToArray();

		return list.Length == 0 ? null : list.First();
	}
}