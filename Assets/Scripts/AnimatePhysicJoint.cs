using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    public Transform TargetBone;

    [SerializeField] bool _isLocal = true;
    [SerializeField] bool _isInvert = true;

    ConfigurableJoint _joint;

    private void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
    }

    void FixedUpdate()
    {
        if (_joint != null && TargetBone != null)
        {
            Quaternion rot;

            if (_isLocal)
                rot = TargetBone.localRotation;
            else
                rot = TargetBone.rotation;

            if (_isInvert)
                rot = Quaternion.Inverse(rot);

            _joint.targetRotation = rot;
        }
    }
}