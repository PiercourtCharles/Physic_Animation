using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    [HideInInspector] public Transform TargetBone;
    [HideInInspector] public bool IsFollowing = true;
    [HideInInspector] public bool UsePhysic = true;
    [HideInInspector] public bool IsRoot = false;

    ConfigurableJoint _joint;
    Rigidbody _rb;
    Quaternion _offsetRot;

    public void Initialize()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _rb = GetComponent<Rigidbody>();
        _offsetRot = transform.localRotation;
    }

    void FixedUpdate()
    {
        if (_joint != null && TargetBone != null)
        {
            Quaternion rot;
            rot = TargetBone.localRotation;
            rot = Quaternion.Inverse(rot);
            rot = rot * _offsetRot;

            if (IsFollowing)
            {
                if (UsePhysic)
                    _joint.targetRotation = rot;
                else
                    transform.localRotation = TargetBone.localRotation;

                //if (IsRoot)
                //    _joint.targetPosition = TargetBone.localPosition;
            }
        }
    }

    public void ActivatePhysic(bool value)
    {
        UsePhysic = value;
        _rb.useGravity = value;

        if (value)
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        else
            _rb.interpolation = RigidbodyInterpolation.None;
    }
}