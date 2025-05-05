using UnityEngine;

public class SimpleIK : MonoBehaviour
{
    public Transform[] _bones;
    public Transform _target;
    public Transform _rotationTarget;
    public Vector3 _offset;

    float[] _lengths = new float[2];
    float _totalLength;

    void Start()
    {
        _lengths[0] = Vector3.Distance(_bones[0].position, _bones[1].position);
        _lengths[1] = Vector3.Distance(_bones[1].position, _bones[2].position);
        _totalLength = _lengths[0] + _lengths[1];
    }

    void LateUpdate()
    {
        SolveIK();
    }

    void SolveIK()
    {
        if (_bones[0] == null || _bones[1] == null || _bones[2] == null || _target == null || _rotationTarget == null)
            return;

        Vector3 shoulderToTarget = _target.position - _bones[0].position;
        float targetDistance = Mathf.Min(shoulderToTarget.magnitude, _totalLength - 0.001f);
        Vector3 shoulderToElbowTarget = (_rotationTarget.position - _bones[0].position).normalized;
        Vector3 axis = Vector3.Cross(shoulderToTarget, shoulderToElbowTarget).normalized;
        if (axis.sqrMagnitude < 0.001f)
            axis = Vector3.up; // fallback security

        //Cosinus law
        float cosAngle0 = Mathf.Clamp((_lengths[0] * _lengths[0] + targetDistance * targetDistance - _lengths[1] * _lengths[1]) / (2 * _lengths[0] * targetDistance), -1.0f, 1.0f);
        float shoulderAngle = Mathf.Acos(cosAngle0) * Mathf.Rad2Deg;

        //Shoulder rot
        Quaternion shoulderToTargetRotation = Quaternion.LookRotation(shoulderToTarget, axis) * Quaternion.Euler(_offset);
        _bones[0].rotation = shoulderToTargetRotation;

        //Shoulder rot target
        _bones[0].Rotate(axis, shoulderAngle, Space.World);

        //Elbow rot
        Vector3 elbowToHand = _target.position - _bones[1].position;
        _bones[1].rotation = Quaternion.LookRotation(elbowToHand, axis) * Quaternion.Euler(_offset);
    }
}