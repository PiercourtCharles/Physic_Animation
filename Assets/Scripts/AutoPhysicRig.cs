using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoPhysicRig : MonoBehaviour
{
    [Header("End branches :")]
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;

    [Header("Anim scripts config :")]
    [SerializeField] bool _isLocalRotationBonesEnable = true;
    [SerializeField] bool _isOffsetedBonesEnable = true;

    [Header("Joints config :")]
    [SerializeField] float _positionSpring = 20000f;
    [SerializeField] float _positionDamper = 1000f;
    [SerializeField] bool _useAcceleration = true;
    [SerializeField] bool _configuredInWorldSpace = false;

    [Header("Rigidbodies config :")]
    [SerializeField] bool _useGravity = true;
    [SerializeField] RigidbodyInterpolation _interpolation = RigidbodyInterpolation.Interpolate;
    [SerializeField] CollisionDetectionMode _collisionDetectionMode = CollisionDetectionMode.Continuous;

    private void Start()
    {
        //Initialize();

        for (int i = 0; i < _endChains.Length; i++)
        {
            Physicate(_endChains[i], _endChainsAnim[i]);
        }
    }

    void Initialize() //Set-up rigidbody trunk model
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;    //linearVelocity because unity 6 else velocity
        rb.angularVelocity = Vector3.zero;
    } 

    Rigidbody Physicate(Transform tf, Transform tfAnim)
    {
        Rigidbody rb;

        if (tf.GetComponent<AnimatePhysicJoint>() != null)
        {
            rb = tf.GetComponent<Rigidbody>();
            return rb;
        }

        var ph = tf.AddComponent<ConfigurableJoint>();
        var anim = tf.AddComponent<AnimatePhysicJoint>();
        rb = tf.GetComponent<Rigidbody>();

        //Joint config
        JointDrive drive = new JointDrive();  //Setup joint drive for motion respond
        drive.positionSpring = _positionSpring;
        drive.positionDamper = _positionDamper;
        drive.maximumForce = Mathf.Infinity;
        drive.useAcceleration = _useAcceleration;

        ph.angularXDrive = drive;   //Apply motion parameter on axis
        ph.angularYZDrive = drive;
        ph.rotationDriveMode = RotationDriveMode.XYAndZ;  //Rotation mod
        ph.configuredInWorldSpace = _configuredInWorldSpace;    //Can recieve worldspace values

        //Liberty of movements for articulations
        ph.angularXMotion = ConfigurableJointMotion.Free;
        ph.angularYMotion = ConfigurableJointMotion.Free;
        ph.angularZMotion = ConfigurableJointMotion.Free;
        ph.xMotion = ConfigurableJointMotion.Locked;
        ph.yMotion = ConfigurableJointMotion.Locked;
        ph.zMotion = ConfigurableJointMotion.Locked;

        //Rigidbody
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //AnimeScript
        anim.TargetBone = tfAnim;
        anim.IsLocal = _isLocalRotationBonesEnable;
        anim.IsOffseted = _isOffsetedBonesEnable;

        if (tf.parent.GetComponent<AnimatePhysicJoint>() != null || tf.parent.GetComponent<AutoPhysicRig>() != null)
            ph.connectedBody = tf.parent.GetComponent<Rigidbody>();
        else
            ph.connectedBody = Physicate(tf.parent, tfAnim.parent);

        return rb;
    }
}
