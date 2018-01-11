using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 10.0f;

    public Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);

    private Transform tr;

    void Awake()
    {
        tr = gameObject.transform;
    }

    void LateUpdate()
    {
        //tr.Rotate(axis, Time.deltaTime * speed);
        tr.RotateAround(origin, axis, Time.deltaTime * speed);
    }
}
