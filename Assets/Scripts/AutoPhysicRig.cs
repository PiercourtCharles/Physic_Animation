using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoPhysicRig : MonoBehaviour
{
    [SerializeField] Transform[] _endChains;
    [SerializeField] Transform[] _endChainsAnim;
    [SerializeField] Transform _animModel;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.linearVelocity = Vector3.zero;    //linearVelocity because unity 6 else velocity
            rb.angularVelocity = Vector3.zero;
        }

        for (int i = 0; i < _endChains.Length; i++)
        {
            Physicate(_endChains[i], _endChainsAnim[i]);
        }
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
        drive.positionSpring = 20000f;
        drive.positionDamper = 1000f;
        drive.maximumForce = Mathf.Infinity;
        drive.useAcceleration = true;

        ph.angularXDrive = drive;   //Apply motion parameter on axis
        ph.angularYZDrive = drive;
        ph.rotationDriveMode = RotationDriveMode.XYAndZ;  //Rotation mod
        ph.configuredInWorldSpace = true;    //Can recieve worldspace values

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

        if (tf.parent.GetComponent<AnimatePhysicJoint>() != null || tf.parent.GetComponent<AutoPhysicRig>() != null)
            ph.connectedBody = tf.parent.GetComponent<Rigidbody>();
        else
            ph.connectedBody = Physicate(tf.parent, tfAnim.parent);

        return rb;
    }
}
