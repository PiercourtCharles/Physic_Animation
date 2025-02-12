using System.Collections;
using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    public Transform TargetBone;
    [SerializeField] ConfigurableJoint _joint;
    [SerializeField] bool _isLocal = true;

    void FixedUpdate()
    {
        if (_joint != null && TargetBone != null)
        {
            Quaternion rot;

            if (_isLocal)
                rot = TargetBone.localRotation;
            else
                rot = TargetBone.rotation;

            Quaternion newTargetRotation = Quaternion.Inverse(rot);
            _joint.targetRotation = newTargetRotation;
        }
    }
}