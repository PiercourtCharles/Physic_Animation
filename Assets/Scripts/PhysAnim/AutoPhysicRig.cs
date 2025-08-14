using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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
                autoRig.ResetPhysic();
                autoRig.Initialize();
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
                Debug.Log("Reset components");
            }

            GUILayout.Label("Other stuffs :");

            //if (GUILayout.Button("Rig search"))
            //{
            //    List<Transform> tf = new();
            //    tf = autoRig.ChildSearch(autoRig.transform, tf);
            //    autoRig._endChains = new Transform[tf.Count];

            //    for (int i = 0; i < tf.Count; i++)
            //    {
            //        autoRig._endChains[i] = tf[i];
            //    }
            //}

            if (GUILayout.Button("SubGroups joints search"))
            {
                autoRig.InitializeSubGroups();
            }
        }
    }
#endif

    [Header("End branches :")]
    [SerializeField] Transform _root;
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;

    [Header("Anim scripts config :")]
    [HideInInspector] public bool IsFollowingRotation = true;
    public bool IsFreeMotion = false;
    public bool UsePhysic = true;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField] float _maxForce = 500f;
    [SerializeField] bool _useAcceleration = true;
    [SerializeField][Range(0.1f, 1f)] float _connectedMassScale = 0.5f; //Scale spring short/long

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    List<Transform> _joints = new();
    float _maxForceValueMemory = 0;

    [SerializeField][Range(0f, 1f)] float _blendRagdollValue = 1f;
    float _blendRagdollValueMemory = 0;
    public SubParts[] SubParts;

    #region Start
    private void Start()
    {
        _maxForceValueMemory = _maxForce;

        Initialize();
    }

    public void Initialize()
    {
        Rigidbody rb = _root.GetComponent<Rigidbody>();

        if (rb == null)
            rb = _root.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        for (int i = 0; i < _endChains.Length; i++)
        {
            if (!CheckJointList(_endChains[i]))
                Physicate(_endChains[i], _endChainsAnim[i]);
        }

        InitializeSubGroups();
    }

    public void InitializeSubGroups()
    {
        for (int i = 0; i < SubParts.Length; i++)
        {
            SubParts[i].AutoRig = this;
            SubParts[i].Initialize();
        }
    }

    bool CheckJointList(Transform tf)
    {
        var anim = tf.GetComponent<AnimatePhysicJoint>();

        if (tf == _root)
            return true;

        if (anim != null && !_joints.Contains(tf))
        {
            _joints.Add(tf);
            return CheckJointList(tf.parent);
        }

        return false;
    }

    Rigidbody Physicate(Transform tf, Transform tfAnim)
    {
        var anim = tf.GetComponent<AnimatePhysicJoint>();
        var joint = tf.GetComponent<ConfigurableJoint>();

        if (anim != null)
            return null;
        if (anim == null)
            anim = tf.AddComponent<AnimatePhysicJoint>();
        if (joint == null)
            joint = tf.AddComponent<ConfigurableJoint>();

        var rb = tf.GetComponent<Rigidbody>();
        var parentJointScript = tf.parent.GetComponent<AnimatePhysicJoint>();

        if (tf.parent == _root)
            anim.IsRoot = true;

        _joints.Add(tf);
        JointSetUp(joint);

        //Rigidbody
        rb.useGravity = _useGravity;
        rb.interpolation = _interpolation;
        rb.collisionDetectionMode = _collisionDetectionMode;

        anim.Initialize();

        //AnimeScript
        anim.TargetBone = tfAnim;
        anim.PositionSpring = _positionSpring;
        anim.PositionDamper = _positionDamper;
        anim.ConnectedMassScale = _connectedMassScale;
        anim.MaxForceValueMemory = _maxForce;
        anim.UseAcceleration = _useAcceleration;
        anim.IsFollowing = IsFollowingRotation;
        anim.UsePhysic = UsePhysic;
        anim.ActivatePhysic(anim.UsePhysic);

        if (parentJointScript != null || tf.parent == _root)
            joint.connectedBody = tf.parent.GetComponent<Rigidbody>();
        else
            joint.connectedBody = Physicate(tf.parent, tfAnim.parent);

        return rb;
    }

    void JointSetUp(ConfigurableJoint joint)
    {
        if (!joint.transform.GetComponent<AnimatePhysicJoint>().UsePhysic)
            return;

        //Joint config
        JointDrive drive = new JointDrive();
        drive.positionSpring = _positionSpring;
        drive.positionDamper = _positionDamper;
        drive.maximumForce = Mathf.Infinity;
        drive.useAcceleration = _useAcceleration;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
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
    #endregion

    #region Update
    private void Update()
    {
        for (int i = 0; i < _joints.Count; i++)
        {
            _joints[i].GetComponent<AnimatePhysicJoint>().FreeMotionRoot(IsFreeMotion);
        }

        if (_blendRagdollValue != _blendRagdollValueMemory)
            BlendRagdoll(_blendRagdollValue);

        for (int i = 0; i < SubParts.Length; i++)
        {
            if (SubParts[i].BlendRagdollValue != SubParts[i].BlendRagdollValueMemory)
                SubParts[i].BlendRagdoll(SubParts[i].BlendRagdollValue);

            SubParts[i].A.GetComponent<AnimatePhysicJoint>().FreeMotion(SubParts[i].IsFreeMotion);
        }
    }

    public void UpdateValues()
    {
        _maxForceValueMemory = _maxForce;

        for (int i = 0; i < _joints.Count; i++)
        {
            JointSetUp(_joints[i].GetComponent<ConfigurableJoint>());

            //Rigidbody
            var rb = _joints[i].GetComponent<Rigidbody>();
            rb.useGravity = _useGravity;
            rb.interpolation = _interpolation;
            rb.collisionDetectionMode = _collisionDetectionMode;

            //AnimeScript
            bool isInSub = false;

            for (int j = 0; j < SubParts.Length; j++)
            {
                if (SubParts[j].Joints.Contains(_joints[i]))
                    isInSub = true;
            }

            if (isInSub)
                continue;

            var anim = _joints[i].GetComponent<AnimatePhysicJoint>();
            anim.PositionSpring = _positionSpring;
            anim.PositionDamper = _positionDamper;
            anim.ConnectedMassScale = _connectedMassScale;
            anim.MaxForceValueMemory = _maxForce;
            anim.IsFollowing = IsFollowingRotation;
            anim.UseAcceleration = _useAcceleration;
            anim.UsePhysic = UsePhysic;
            anim.ActivatePhysic(anim.UsePhysic);

            var jointScript = _joints[i].GetComponent<AnimatePhysicJoint>();
            jointScript.ActivatePhysic(jointScript.UsePhysic);
        }

        for (int i = 0; i < SubParts.Length; i++)
        {
            SubParts[i].UpdateValues();
        }
    }

    public void BlendRagdoll(float value)
    {
        if (value < 0) value = 0;
        if (value > 1) value = 1;


        bool ragdoll = true;

        if (value > 0)
            ragdoll = false;

        for (int i = 0; i < _joints.Count; i++)
        {
            _joints[i].GetComponent<AnimatePhysicJoint>().RagdollJoint(_joints[i].transform, value, ragdoll);
        }

        _blendRagdollValueMemory = value;
    }
    #endregion

    #region Clean
    public void ResetPhysic()
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

            DestroyImmediate(_root.GetComponent<Rigidbody>());
            _joints.Clear();
        }
        else
        {
            for (int i = 0; i < _endChains.Length; i++)
            {
                ExtremeClean(_endChains[i]);
            }
            ExtremeClean(_root);
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
    [SerializeField] Transform[] _b;

    [Header("Anim scripts config :")]
    [HideInInspector] public bool IsFollowingRotation = true;
    public bool IsFreeMotion = false;
    public bool UsePhysic = true;

    [Header("Joints config :")]
    public float PositionSpring = 20000f;
    public float PositionDamper = 1000f;
    public float MaxForce = 0;
    [HideInInspector] public float MaxForceValueMemory = 0;
    public bool UseAcceleration = true;
    [Range(0.1f, 1f)]
    public float ConnectedMassScale = 0.5f;

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    [Header("Debug :")]
    [Range(0f, 1f)]
    public float BlendRagdollValue = 1f;
    [HideInInspector] public float BlendRagdollValueMemory = 0;
    [HideInInspector] public AutoPhysicRig AutoRig;

    public List<Transform> Joints = new List<Transform>();

    public void Initialize()
    {
        IsFollowingRotation = AutoRig.IsFollowingRotation;
        MaxForceValueMemory = MaxForce;

        Joints.Clear();

        //Check order
        for (int i = 0; i < _b.Length; i++)
        {
            if (A != null && _b != null && FindTransform(A, _b[i].name) == null)
            {
                Debug.LogError("A/B not set correctly");
                return;
            }
        }

        for (int i = 0; i < _b.Length; i++)
        {
            ListSub(_b[i]);
        }

        Joints.Add(A);
        ActivatePhysic(UsePhysic);
    }

    void ListSub(Transform tf)
    {
        if (tf == A)
            return;

        if (!Joints.Contains(tf))
            Joints.Add(tf);

        ListSub(tf.parent);
    }

    public void UpdateValues()
    {
        MaxForceValueMemory = MaxForce;

        for (int i = 0; i < Joints.Count; i++)
        {
            JointSetUp(Joints[i].GetComponent<ConfigurableJoint>());

            //Rigidbody
            var rb = Joints[i].GetComponent<Rigidbody>();
            rb.useGravity = _useGravity;
            rb.interpolation = _interpolation;
            rb.collisionDetectionMode = _collisionDetectionMode;

            ActivatePhysic(UsePhysic);
        }
    }

    void JointSetUp(ConfigurableJoint joint)
    {
        if (!joint.transform.GetComponent<AnimatePhysicJoint>().UsePhysic)
            return;

        //Joint config
        JointDrive drive = new JointDrive();
        drive.positionSpring = PositionSpring;
        drive.positionDamper = PositionDamper;
        drive.maximumForce = Mathf.Infinity;
        drive.useAcceleration = UseAcceleration;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        joint.connectedMassScale = ConnectedMassScale;
        joint.enableCollision = true;

        //Liberty of movements for articulations
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
    }

    public void BlendRagdoll(float value)
    {
        if (value < 0) value = 0;
        if (value > 1) value = 1;


        bool ragdoll = true;

        if (value > 0)
            ragdoll = false;

        for (int i = 0; i < Joints.Count; i++)
        {
            Joints[i].GetComponent<AnimatePhysicJoint>().RagdollJoint(Joints[i].transform, value, ragdoll);
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