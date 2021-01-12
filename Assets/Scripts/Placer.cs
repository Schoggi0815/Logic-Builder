using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    private bool _isPickedUp;

    private Vector3 _offset;

    private SpriteRenderer _spriteRenderer;
    private Color _normalColor;

    [SerializeField] private Color selectColor;

    private bool _snap;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _normalColor = _spriteRenderer.color;
    }

    private void Update()
    {
        _snap = Input.GetKey(KeyCode.LeftControl);
        
        if (_isPickedUp)
        {
            var mouse = Physics.GetWorldPosOfMouse();
            var worldPosOfMouse = mouse - _offset;

            if (_snap)
            {
                for (int i = 0; i < Constants.C.knobs.Count; i++)
                {
                    var knob = Constants.C.knobs[i].transform.position;
                    if (Physics.IsClose(mouse, knob))
                    {
                        worldPosOfMouse = knob;
                        break;
                    }

                    if (Math.Abs(knob.x - mouse.x) < .25f)
                    {
                        worldPosOfMouse = new Vector3(knob.x, worldPosOfMouse.y, worldPosOfMouse.z);
                        break;
                    }

                    if (Math.Abs(knob.y - mouse.y) < .25f)
                    {
                        worldPosOfMouse = new Vector3(worldPosOfMouse.x, knob.y, worldPosOfMouse.z);
                        break;
                    }
                }
            }

            transform.position = worldPosOfMouse;
        }
    }

    private void OnMouseDown()
    {
        _isPickedUp = !_isPickedUp;

        if (_isPickedUp)
        {
            _offset = Physics.GetWorldPosOfMouse() - transform.position;
            _spriteRenderer.color = selectColor;
        }
        else
        {
            _spriteRenderer.color = _normalColor;
        }
    }
}
