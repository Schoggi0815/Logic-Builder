using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
	public Transform parentCamera;

	private bool _mouseIsOn;
	private bool _mouseWasOn;

	private Vector3 _startMousePos;

	void Update()
	{
		if (!_mouseIsOn) return;

		if (Input.GetMouseButtonDown(2))
		{
			_startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_startMousePos.z = 0.0f;
			_mouseWasOn = true;
		}

		if (Input.GetMouseButton(2) && _mouseWasOn)
		{
			Vector3 nowMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			nowMousePos.z = 0.0f;
			parentCamera.position += _startMousePos - nowMousePos;
		}
		else
		{
			_mouseWasOn = false;
		}
	}

	private void OnMouseEnter()
	{
		_mouseIsOn = true;
	}

	private void OnMouseExit()
	{
		_mouseIsOn = false;
	}
}