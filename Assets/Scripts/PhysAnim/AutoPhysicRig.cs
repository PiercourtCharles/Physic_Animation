using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoPhysicRig : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AutoPhysicRig))]
    public class AutoPhysicRigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AutoPhysicRig autoRig = (AutoPhysicRig)target;

            if (GUILayout.Button("Setup model"))
            {

                autoRig.ResetPhysic();

                autoRig.Initialize(); //If Rigidbody not set correctly

                for (int i = 0; i < autoRig._endChains.Length; i++)
                {
                    autoRig.Physicate(autoRig._endChains[i], autoRig._endChainsAnim[i]);
                }

                autoRig._isSetup = true;
                Debug.Log("Setup components");
            }

            if (GUILayout.Button("Update values"))
            {
                autoRig.UpdateValues();
                Debug.Log("Update values");
            }

            if (GUILayout.Button("Clean model"))
            {
                autoRig.ResetPhysic();
                autoRig._isSetup = false;
                Debug.Log("Reset components");
            }

            if (GUILayout.Button("Physic enable"))
            {
                if (autoRig._connectedMassScaleValueMemory == null)
                    autoRig._connectedMassScaleValueMemory = autoRig._connectedMassScale;

                autoRig.ActivatePhysic(autoRig._isPhysic);
                autoRig._isPhysic = !autoRig._isPhysic;
                Debug.Log("Physic enable : " + autoRig._isPhysic);
            }
        }
    }
#endif
    [Header("End branches :")]
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;

    [Header("Anim scripts config :")]
    [SerializeField] bool _isLocalRotationBonesEnable = true;
    [SerializeField] bool _isOffsetedBonesEnable = true;
    [SerializeField] bool _isFollowingRotation = true;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField] float _maxForce = 500f;
    [SerializeField][Range(0f, 1f)] float _connectedMassScale = 0.5f; //Scale spring short/long
    [SerializeField] bool _useAcceleration = true;
    [SerializeField] bool _configuredInWorldSpace = false; //Get world position/rotation

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    List<ConfigurableJoint> _joints = new List<ConfigurableJoint>();
    float? _connectedMassScaleValueMemory = null;
    float _maxForceValueMemory = 0;
    bool _isPhysic = false;
    bool _isSetup = false;

    [Header("Debug :")][SerializeField][Range(0f, 1f)] float _blendRagdoll = 1f;
    float _blendRagdollValueMemory = 0;

    private void Start()
    {
        _connectedMassScaleValueMemory = _connectedMassScale;
        _maxForceValueMemory = _maxForce;

        if (_isSetup)
            return;

        Initialize(); //If Rigidbody not set correctly

        for (int i = 0; i < _endChains.Length; i++)
        {
            Physicate(_endChains[i], _endChainsAnim[i]);
        }
    }

    private void Update()
    {
        if (_blendRagdoll != _blendRagdollValueMemory)
        {
            BlendRagdoll(_blendRagdoll);
            _blendRagdollValueMemory = _blendRagdoll;
        }
    }

    public void ActivatePhysic(bool value)
    {
        if (value)
            _connectedMassScale = (float)_connectedMassScaleValueMemory;
        else
            _connectedMassScale = 0;

        JointsUpdate();
    }

    public void BlendRagdoll(float value)
    {
        if (value < 0) value = 0;
        if (value > 1) value = 1;

        var massScale = Mathf.Lerp(1, _connectedMassScale, value);
        var force = Mathf.Lerp(0, _maxForceValueMemory, value);

        for (int i = 0; i < _joints.Count; i++)
        {
            _joints[i].GetComponent<AnimatePhysicJoint>().IsFollowing = _isFollowingRotation;

            var joint = _joints[i].GetComponent<ConfigurableJoint>();
            joint.connectedMassScale = massScale;

            JointDrive drive = new JointDrive();  //Setup joint drive for motion respond
            drive.positionSpring = _positionSpring;
            drive.positionDamper = _positionDamper;
            drive.maximumForce = force;
            drive.useAcceleration = _useAcceleration;

            joint.angularXDrive = drive;   //Apply motion parameter on axis
            joint.angularYZDrive = drive;
        }
    }



    void Initialize() //Root set-up rigidbody
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.velocity = Vector3.zero;    //"linearVelocity" because of unity 6 else "velocity"
        //rb.angularVelocity = Vector3.zero;
    }

    Rigidbody Physicate(Transform tf, Transform tfAnim)
    {
        Rigidbody rb;

        if (tf.GetComponent<AnimatePhysicJoint>() != null)
        {
            rb = tf.GetComponent<Rigidbody>();
            return rb;
        }

        var joint = tf.AddComponent<ConfigurableJoint>();
        var anim = tf.AddComponent<AnimatePhysicJoint>();
        rb = tf.GetComponent<Rigidbody>();
        _joints.Add(joint);

        JointSetUp(joint); //Joint

        //Rigidbody
        rb.useGravity = _useGravity;
        rb.interpolation = _interpolation;
        rb.collisionDetectionMode = _collisionDetectionMode;

        //AnimeScript
        anim.TargetBone = tfAnim;
        anim.IsLocal = _isLocalRotationBonesEnable;
        anim.IsOffseted = _isOffsetedBonesEnable;
        anim.IsFollowing = _isFollowingRotation;

        if (tf.parent.GetComponent<AnimatePhysicJoint>() != null || tf.parent.GetComponent<AutoPhysicRig>() != null)
            joint.connectedBody = tf.parent.GetComponent<Rigidbody>();
        else
            joint.connectedBody = Physicate(tf.parent, tfAnim.parent);

        return rb;
    }

    void JointSetUp(ConfigurableJoint joint)
    {
        //Joint config
        JointDrive drive = new JointDrive();  //Setup joint drive for motion respond
        drive.positionSpring = _positionSpring;
        drive.positionDamper = _positionDamper;
        drive.maximumForce = Mathf.Infinity;
        drive.useAcceleration = _useAcceleration;

        //joint.xDrive = drive;  //Apply motion parameter on position
        //joint.yDrive = drive;
        //joint.zDrive = drive;
        joint.angularXDrive = drive;   //Apply motion parameter on axis
        joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;  //Rotation mod
        joint.configuredInWorldSpace = _configuredInWorldSpace;    //Can recieve worldspace values
        joint.connectedMassScale = _connectedMassScale;
        joint.enableCollision = true;

        //Liberty of movements for articulations
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
    }

    void JointsUpdate()
    {
        for (int i = 0; i < _joints.Count; i++)
        {
            JointSetUp(_joints[i]);
        }
    }

    void UpdateValues()
    {
        for (int i = 0; i < _joints.Count; i++)
        {
            JointSetUp(_joints[i]);

            //Rigidbody
            var rb = _joints[i].GetComponent<Rigidbody>();
            rb.useGravity = _useGravity;
            rb.interpolation = _interpolation;
            rb.collisionDetectionMode = _collisionDetectionMode;

            //AnimeScript
            var anim = _joints[i].GetComponent<AnimatePhysicJoint>();
            anim.IsLocal = _isLocalRotationBonesEnable;
            anim.IsOffseted = _isOffsetedBonesEnable;
            anim.IsFollowing = _isFollowingRotation;
        }
    }

    void ResetPhysic()
    {
        if (_joints.Count > 0)
        {
            for (int i = 0; i < _joints.Count; i++)
            {
                var config = _joints[i].GetComponent<ConfigurableJoint>();
                var anim = _joints[i].GetComponent<AnimatePhysicJoint>();
                var rb = _joints[i].GetComponent<Rigidbody>();
                DestroyImmediate(config);
                DestroyImmediate(anim);
                DestroyImmediate(rb);
            }

            _joints.Clear();
        }
        else
        {
            for (int i = 0; i < _endChains.Length; i++)
            {
                ExtremeClean(_endChains[i]);
            }
        }
    }

    void ExtremeClean(Transform tf)
    {
        var config = tf.GetComponent<ConfigurableJoint>();
        var anim = tf.GetComponent<AnimatePhysicJoint>();
        var rb = tf.GetComponent<Rigidbody>();
        DestroyImmediate(config);
        DestroyImmediate(anim);
        DestroyImmediate(rb);

        if (tf.parent.GetComponent<AnimatePhysicJoint>() != null)
            ExtremeClean(tf.parent);
    }
}
