using System;
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

            GUILayout.Label("Setup model :");

            if (GUILayout.Button("Setup model"))
            {
                //autoRig.ResetPhysic();

                //autoRig.Initialize(); //If Rigidbody not set correctly

                //for (int i = 0; i < autoRig.SubParts.Length; i++)
                //{
                //    autoRig.SubParts[i].Initialize();
                //}

                //for (int i = 0; i < autoRig._endChains.Length; i++)
                //{
                //    autoRig.Physicate(autoRig._endChains[i], autoRig._endChainsAnim[i], null);
                //}

                //autoRig._isSetup = true;
                //Debug.Log("Setup components");
                Debug.Log("Actualy in summer break, maybe an other time :/");
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

            GUILayout.Label("Other stuffs :");

            if (GUILayout.Button("Rig search"))
            {
                List<Transform> tf = new();
                tf = autoRig.ChildSearch(autoRig.transform, tf);
                autoRig._endChains = new Transform[tf.Count];

                for (int i = 0; i < tf.Count; i++)
                {
                    autoRig._endChains[i] = tf[i];
                }
            }

            if (GUILayout.Button("Sub search"))
            {
                for (int i = 0; i < autoRig.SubParts.Length; i++)
                {
                    autoRig.SubParts[i].AutoRig = autoRig;
                    autoRig.SubParts[i].Initialize();
                }
            }
        }
    }
#endif

    [Header("End branches :")]
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;
    //[SerializeField] Transform[] _limitPoints;

    [Header("Anim scripts config :")]
    [HideInInspector] public bool IsFollowingRotation = true;
    bool _isRagdoll = false;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField] float _maxForce = 500f;
    [SerializeField][Range(0f, 1f)] float _connectedMassScale = 0.5f; //Scale spring short/long
    [SerializeField] bool _useAcceleration = true;

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    List<ConfigurableJoint> _joints = new List<ConfigurableJoint>();
    float _maxForceValueMemory = 0;
    bool _isSetup = false;

    [SerializeField][Range(0f, 1f)] float _blendRagdollValue = 1f;
    float _blendRagdollValueMemory = 0;
    public SubParts[] SubParts;

    #region Start
    private void Start()
    {
        _maxForceValueMemory = _maxForce;

        if (_isSetup)
            return;

        Initialize();
    }

    void Initialize() //Root set-up rigidbody
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.velocity = Vector3.zero;    //"linearVelocity" because of unity 6 else "velocity"
        //rb.angularVelocity = Vector3.zero;

        for (int i = 0; i < _endChains.Length; i++)
        {
            Physicate(_endChains[i], _endChainsAnim[i], null);
        }

        for (int i = 0; i < SubParts.Length; i++)
        {
            SubParts[i].Initialize();
        }
    }

    Rigidbody Physicate(Transform tf, Transform tfAnim, AnimatePhysicJoint jointChild)
    {
        Rigidbody rb;

        if (tf.GetComponent<AnimatePhysicJoint>() != null)
        {
            rb = tf.GetComponent<Rigidbody>();
            return rb;
        }

        var joint = tf.AddComponent<ConfigurableJoint>();
        var anim = tf.AddComponent<AnimatePhysicJoint>();
        anim.Initialize();
        rb = tf.GetComponent<Rigidbody>();
        _joints.Add(joint);

        if (tf.parent.GetComponent<AutoPhysicRig>() != null)
            anim.IsRoot = true;

        JointSetUp(joint, _isRagdoll); //Joint

        //Rigidbody
        rb.useGravity = _useGravity;
        rb.interpolation = _interpolation;
        rb.collisionDetectionMode = _collisionDetectionMode;

        //AnimeScript
        anim.TargetBone = tfAnim;
        anim.IsFollowing = IsFollowingRotation;

        //Parent check
        //for (int i = 0; i < _limitPoints.Length; i++)
        //{
        //    if (tf.parent == _limitPoints[i])
        //    {
        //        anim.ActivatePhysic(false);
        //        break;
        //    }
        //}

        var parentJointScript = tf.parent.GetComponent<AnimatePhysicJoint>();

        if (parentJointScript != null || tf.parent.GetComponent<AutoPhysicRig>() != null)
            joint.connectedBody = tf.parent.GetComponent<Rigidbody>();
        else
            joint.connectedBody = Physicate(tf.parent, tfAnim.parent, anim);

        //if (!anim.UsePhysic && jointChild != null)
        //    jointChild.ActivatePhysic(false);

        return rb;
    }

    void JointSetUp(ConfigurableJoint joint, bool ragdoll)
    {
        if (!joint.transform.GetComponent<AnimatePhysicJoint>().UsePhysic)
            return;

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
        joint.connectedMassScale = _connectedMassScale;
        joint.enableCollision = true;

        //Liberty of movements for articulations
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        if (ragdoll && joint.GetComponent<AnimatePhysicJoint>() != null && joint.GetComponent<AnimatePhysicJoint>().IsRoot)
        {
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_blendRagdollValue != _blendRagdollValueMemory)
            BlendRagdoll(_blendRagdollValue);

        for (int i = 0; i < SubParts.Length; i++)
        {
            if (SubParts[i].BlendRagdollValue != SubParts[i].BlendRagdollValueMemory)
                SubParts[i].BlendRagdoll(SubParts[i].BlendRagdollValue);
        }
    }

    void UpdateValues()
    {
        _maxForceValueMemory = _maxForce;

        for (int i = 0; i < _joints.Count; i++)
        {
            JointSetUp(_joints[i], _isRagdoll);

            //Rigidbody
            var rb = _joints[i].GetComponent<Rigidbody>();
            rb.useGravity = _useGravity;
            rb.interpolation = _interpolation;
            rb.collisionDetectionMode = _collisionDetectionMode;

            //AnimeScript
            var anim = _joints[i].GetComponent<AnimatePhysicJoint>();
            anim.IsFollowing = IsFollowingRotation;
            anim.ActivatePhysic(anim.UsePhysic);
        }

        for (int i = 0; i < SubParts.Length; i++)
        {
            for (int j = 0; j < SubParts[i].Joints.Count; j++)
            {
                JointSetUp(SubParts[i].Joints[j].GetComponent<ConfigurableJoint>(), _isRagdoll);

                //Rigidbody
                var rb = SubParts[i].Joints[j].GetComponent<Rigidbody>();
                rb.useGravity = _useGravity;
                rb.interpolation = _interpolation;
                rb.collisionDetectionMode = _collisionDetectionMode;

                //AnimeScript
                var anim = SubParts[i].Joints[j].GetComponent<AnimatePhysicJoint>();
                anim.IsFollowing = IsFollowingRotation;

                SubParts[i].ActivatePhysic(SubParts[i].UsePhysic);
            }
        }
    }

    public void BlendRagdoll(float value)
    {
        if (value < 0) value = 0;
        if (value > 1) value = 1;

        var massScale = Mathf.Lerp(1, _connectedMassScale, value);
        var force = Mathf.Lerp(0, _maxForceValueMemory, value);

        for (int i = 0; i < _joints.Count; i++)
        {
            _joints[i].GetComponent<AnimatePhysicJoint>().IsFollowing = IsFollowingRotation;

            var joint = _joints[i].GetComponent<ConfigurableJoint>();
            joint.connectedMassScale = massScale;

            JointDrive drive = new JointDrive();  //Setup joint drive for motion respond
            drive.positionSpring = _positionSpring;
            drive.positionDamper = _positionDamper;
            drive.maximumForce = force;
            drive.useAcceleration = _useAcceleration;

            joint.angularXDrive = drive;   //Apply motion parameter on axis
            joint.angularYZDrive = drive;

            if (_joints[i].GetComponent<AnimatePhysicJoint>().IsRoot)
            {
                if (value <= 0.1f)
                {
                    _isRagdoll = true;
                    joint.xMotion = ConfigurableJointMotion.Free;
                    joint.yMotion = ConfigurableJointMotion.Free;
                    joint.zMotion = ConfigurableJointMotion.Free;
                }
                else
                {
                    _isRagdoll = false;
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;
                }
            }
        }

        _blendRagdollValueMemory = value;
    }
    #endregion

    #region Clean
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
    #endregion

    List<Transform> ChildSearch(Transform tf, List<Transform> list)
    {
        if (tf.childCount == 0)
            list.Add(tf);
        else
        {
            int number = tf.childCount;

            for (int i = 0; i < number; i++)
            {
                ChildSearch(tf.GetChild(i), list);
            }
        }

        return list;
    }
}

[Serializable]
public class SubParts
{
    [Tooltip("Bones A : Parent")]
    public Transform A;
    [Tooltip("Bones B : Child")]
    public Transform B;

    [Header("Anim scripts config :")]
    bool _isFollowingRotation = true;
    bool _isRagdoll = false;
    public bool UsePhysic = true;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField] float _maxForce = 0;
    [SerializeField] bool _useAcceleration = true;
    [Range(0f, 1f)]
    public float ConnectedMassScale = 0.5f; //Scale spring short/long
    float _maxForceValueMemory = 0;

    [Header("Debug :")]
    [Range(0f, 1f)]
    public float BlendRagdollValue = 1f;
    [HideInInspector] public float BlendRagdollValueMemory = 0;
    [HideInInspector] public AutoPhysicRig AutoRig;

    public List<Transform> Joints = new List<Transform>();

    public void Initialize()
    {
        _isFollowingRotation = AutoRig.IsFollowingRotation;
        _maxForceValueMemory = _maxForce;

        Joints.Clear();

        //Check order
        if (A != null && B != null && FindTransform(A, B.name) == null)
        {
            Debug.LogError("A/B not set correctly");
            return;
        }

        //Get all bones
        Transform tf = B;

        while (tf != A)
        {
            Joints.Add(tf);

            tf = tf.parent;
        }

        Joints.Add(A);
        ActivatePhysic(UsePhysic);
    }

    public void BlendRagdoll(float value)
    {
        if (value < 0) value = 0;
        if (value > 1) value = 1;

        var massScale = Mathf.Lerp(1, ConnectedMassScale, value);
        var force = Mathf.Lerp(0, _maxForceValueMemory, value);

        for (int i = 0; i < Joints.Count; i++)
        {
            Joints[i].GetComponent<AnimatePhysicJoint>().IsFollowing = _isFollowingRotation;

            var joint = Joints[i].GetComponent<ConfigurableJoint>();
            joint.connectedMassScale = massScale;

            JointDrive drive = new JointDrive();  //Setup joint drive for motion respond
            drive.positionSpring = _positionSpring;
            drive.positionDamper = _positionDamper;
            drive.maximumForce = force;
            drive.useAcceleration = _useAcceleration;

            joint.angularXDrive = drive;   //Apply motion parameter on axis
            joint.angularYZDrive = drive;

            if (Joints[i].GetComponent<AnimatePhysicJoint>().IsRoot)
            {
                if (value <= 0.1f)
                {
                    _isRagdoll = true;
                    joint.xMotion = ConfigurableJointMotion.Free;
                    joint.yMotion = ConfigurableJointMotion.Free;
                    joint.zMotion = ConfigurableJointMotion.Free;
                }
                else
                {
                    _isRagdoll = false;
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;
                }
            }
        }

        BlendRagdollValueMemory = value;
    }

    public void ActivatePhysic(bool value)
    {
        for (int i = 0; i < Joints.Count; i++)
        {
            var joint = Joints[i].GetComponent<AnimatePhysicJoint>();

            if (joint == null)
                continue;

            joint.ActivatePhysic(value);
        }
    }

    Transform FindTransform(Transform parent, string name)
    {
        if (parent.name.Equals(name))
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindTransform(child, name);

            if (result != null)
                return result;
        }

        return null;
    }
}