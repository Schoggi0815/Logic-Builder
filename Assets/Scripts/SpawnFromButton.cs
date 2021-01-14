using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFromButton : MonoBehaviour
{
    private RectTransform _rectTransform;

    public GameObject prefab;

    public Vector3 offset;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        var screenToWorldPoint = Camera.main.ScreenToWorldPoint(_rectTransform.position);
        Instantiate(prefab, new Vector3(screenToWorldPoint.x + offset.x * Camera.main.orthographicSize, screenToWorldPoint.y + offset.y * Camera.main.orthographicSize, 0), new Quaternion());
    }
}
