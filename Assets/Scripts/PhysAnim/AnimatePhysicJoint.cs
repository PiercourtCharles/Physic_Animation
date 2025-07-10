using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    public Transform TargetBone;
    public bool IsLocal = true;
    public bool IsOffseted = true;
    public bool IsFollowing = true;

    [SerializeField] bool _isInvert = true;

    ConfigurableJoint _joint;
    Quaternion _offsetRot;
    Vector3 _offsetLocalPos;
    Vector3 _offsetPos;

    private void Start()
    {
        _joint = GetComponent<ConfigurableJoint>();
        _offsetRot = transform.localRotation;
        //_offsetLocalPos = TargetBone.localPosition;
        //_offsetPos = TargetBone.position;
    }

    void FixedUpdate()
    {
        if (_joint != null && TargetBone != null)
        {
            Quaternion rot;
            //Vector3 pos;

            if (IsLocal)
            {
                rot = TargetBone.localRotation;
                //pos = TargetBone.localPosition - _offsetPos;
            }
            else
            {
                rot = TargetBone.rotation;
                //pos = TargetBone.position - _offsetLocalPos;
            }

            if (_isInvert)
            {
                rot = Quaternion.Inverse(rot);
                //pos = -pos;
            }

            if (IsOffseted)
                rot = rot * _offsetRot;

            if (IsFollowing)
            {
                _joint.targetRotation = rot;
                //_joint.targetPosition = pos;
            }
        }
    }
}