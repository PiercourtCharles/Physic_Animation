using UnityEngine;

public class AnimatePhysicJoint : MonoBehaviour
{
    [HideInInspector] public Transform TargetBone;
    [HideInInspector] public bool IsFollowing;
    [HideInInspector] public bool UsePhysic = true;
    [HideInInspector] public bool IsRoot = false;

    [HideInInspector] public float PositionSpring;
    [HideInInspector] public float PositionDamper;
    [HideInInspector] public float MaxForceValueMemory;
    [HideInInspector] public float ConnectedMassScale;
    [HideInInspector] public bool UseAcceleration;
    [HideInInspector] public bool IsRagdoll;

    Transform _tfParent;
    ConfigurableJoint _joint;
    Rigidbody _rb;
    RigidbodyInterpolation _rbInterpoMemory;
    Quaternion _offsetRot;
    Quaternion _originRot;
    Vector3 _originPos;
    bool _usePhysicMemory;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _usePhysicMemory = UsePhysic;
        _joint = GetComponent<ConfigurableJoint>();
        _rb = GetComponent<Rigidbody>();
        _offsetRot = transform.localRotation;
        _originPos = transform.localPosition;
        _originRot = transform.localRotation;
        _tfParent = transform.parent;
        _rbInterpoMemory = _rb.interpolation;
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
            }
        }
    }

    public void ActivatePhysic(bool value)
    {
        UsePhysic = value;
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = value;
        _rb.isKinematic = !value;

        if (value)
            _rb.interpolation = _rbInterpoMemory;
        else
            _rb.interpolation = RigidbodyInterpolation.None;
    }

    public void RagdollJoint(Transform joint, float value, bool free)
    {
        var massScale = Mathf.Lerp(1, ConnectedMassScale, value);
        var force = Mathf.Lerp(0, MaxForceValueMemory, value);

        _joint.connectedMassScale = massScale;

        JointDrive drive = new JointDrive();
        drive.positionSpring = PositionSpring;
        drive.positionDamper = PositionDamper;
        drive.maximumForce = force;
        drive.useAcceleration = UseAcceleration;

        _joint.angularXDrive = drive;
        _joint.angularYZDrive = drive;

        if (!_usePhysicMemory)
            ActivatePhysic(free);
    }

    public void FreeMotionRoot(bool value)
    {
        if (IsRoot)
            FreeMotion(value);
    }

    public void FreeMotion(bool value)
    {
        if (value)
        {
            if (IsRagdoll)
                return;

            if (!_usePhysicMemory)
                ActivatePhysic(true);

            JointDrive drive = new JointDrive();
            _joint.angularXDrive = drive;
            _joint.angularYZDrive = drive;

            _joint.xMotion = ConfigurableJointMotion.Free;
            _joint.yMotion = ConfigurableJointMotion.Free;
            _joint.zMotion = ConfigurableJointMotion.Free;

            IsRagdoll = true;

            if (!IsRoot)
                transform.SetParent(null);
        }
        else
        {
            if (!IsRagdoll)
                return;

            if (!_usePhysicMemory)
                ActivatePhysic(false);

            if (!IsRoot)
                transform.SetParent(_tfParent);

            IsRagdoll = false;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            JointDrive drive = new JointDrive();
            drive.positionSpring = PositionSpring;
            drive.positionDamper = PositionDamper;
            drive.maximumForce = MaxForceValueMemory;
            drive.useAcceleration = UseAcceleration;

            _joint.angularXDrive = drive;
            _joint.angularYZDrive = drive;

            _joint.xMotion = ConfigurableJointMotion.Locked;
            _joint.yMotion = ConfigurableJointMotion.Locked;
            _joint.zMotion = ConfigurableJointMotion.Locked;

            transform.localPosition = _originPos;
            transform.localRotation = _originRot;
        }
    }
}