using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    public Transform TargetBone;
    public bool IsLocal = true;
    public bool IsOffseted = true;

    [SerializeField] bool _isInvert = true;

    ConfigurableJoint _joint;
    Quaternion _offset;

    private void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _offset = transform.localRotation;
    }

    void FixedUpdate()
    {
        if (_joint != null && TargetBone != null)
        {
            Quaternion rot;

            if (IsLocal)
                rot = TargetBone.localRotation;
            else
                rot = TargetBone.rotation;

            if (_isInvert)
                rot = Quaternion.Inverse(rot);

            if (IsOffseted)
                rot = rot * _offset;

            _joint.targetRotation = rot;
        }
    }
}