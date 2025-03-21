using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoPhysicRig : MonoBehaviour
{
    [Tooltip("Actualize joints values on model in play mode")] public bool Actualize = false;

    [Header("End branches :")]
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;

    [Header("Anim scripts config :")]
    [SerializeField] bool _isLocalRotationBonesEnable = true;
    [SerializeField] bool _isOffsetedBonesEnable = true;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField][Range(0f, 1f)] float _connectedMassScale = 0.3f; //Scale spring short/long
    [SerializeField] bool _useAcceleration = true;
    [SerializeField] bool _configuredInWorldSpace = false; //Get world position/rotation

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    List<ConfigurableJoint> _joints = new List<ConfigurableJoint>();

    private void Start()
    {
        Initialize(); //If Rigidbody not set correctly

        for (int i = 0; i < _endChains.Length; i++)
        {
            Physicate(_endChains[i], _endChainsAnim[i]);
        }
    }

    private void Update()
    {
        if (Actualize)
        {
            for (int i = 0; i < _joints.Count; i++)
            {
                JointSetUp(_joints[i]);
            }

            Actualize = false;
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

        joint.angularXDrive = drive;   //Apply motion parameter on axis
        joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;  //Rotation mod
        joint.configuredInWorldSpace = _configuredInWorldSpace;    //Can recieve worldspace values
        joint.connectedMassScale = _connectedMassScale;

        //Liberty of movements for articulations
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
    }
}
