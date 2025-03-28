using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCam : MonoBehaviour
{
    [SerializeField] Transform _pivot;
    [SerializeField] Vector3 _speed;

    private void FixedUpdate()
    {
        _pivot.Rotate(_speed);
    }
}
