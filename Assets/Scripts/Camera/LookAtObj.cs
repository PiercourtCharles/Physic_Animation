using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObj : MonoBehaviour
{
    [SerializeField] Transform _target;

    private void FixedUpdate()
    {
        transform.LookAt(_target);
    }
}
