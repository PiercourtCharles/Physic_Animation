using System.Collections.Generic;
using UnityEngine;

public class AngleTestScript : MonoBehaviour
{
    //[SerializeField][Tooltip("Ugly button")] bool _Actualize = false;
    [SerializeField] Transform _target;
    [SerializeField] Transform _targetB;
    Vector3 _bPoint;
    float _maxBonesDistance = 0;

    [Header("Targets :")]
    [SerializeField] Transform[] _bones;

    [Header("Distances :")]
    [SerializeField] List<float> _distances = new();

    private void Start()
    {
        GetDistance();
    }

    private void FixedUpdate()
    {
        if (_maxBonesDistance >= (_target.position - _bones[0].position).magnitude)
        {
            ResetValues();

            float _bpointDistance = ((_target.position - _bones[0].position) / 2).magnitude;
            _bPoint = (_target.position - _bones[0].position) / 2 + _bones[0].position;
            _targetB.position = _bPoint;

            for (int i = 0; i < _bones.Length - 1; i++)
            {





                //Find empty value of the triangle
                float value2 = 0;




                Process(_bones[i], _bones[i + 1], _bPoint, _distances[i], value2);
                _bones[i].LookAt(_bones[i + 1]);
            }

            float value3 = Mathf.Pow(_distances[_bones.Length - 1], 2) - Mathf.Pow(_bpointDistance, 2);
            float value4 = Mathf.Sqrt(Mathf.Abs(value3));
            Process(_bones[_bones.Length - 2], _bones[_bones.Length - 1], _bPoint, _distances[_bones.Length - 1], value4);
            _bones[_bones.Length - 1].LookAt(_target);
        }
        else
        {
            _bones[0].LookAt(_target);
        }
    }

    void Process(Transform point0, Transform point1, Vector3 pointB, float value1, float value2)
    {
        point1.position = point0.position + Get3Point(point0, pointB, value1, value2);
    }

    Vector3 Get3Point(Transform tf1, Vector3 tf3, float value1, float value2)
    {
        // Vecteur AC && normalisation de AC
        Vector3 AC = tf3 - tf1.position;
        float distanceAC = AC.magnitude;
        distanceAC = Mathf.Clamp(distanceAC, 0.0001f, value1 + value2 - 0.0001f); // Clamp la distance pour éviter des erreurs si la cible est trop loin

        Vector3 AC_normalized = AC.normalized;

        // Loi des cosinus pour trouver l'angle au coude
        float cosTheta = CosinusLaw(value1, value2, distanceAC);
        float theta = Mathf.Acos(cosTheta); // Angle en radians

        // Trouver un vecteur perpendiculaire à AC
        Vector3 axis = Vector3.Cross(Vector3.up, AC_normalized).normalized; // Utilise "up" par défaut

        if (axis.magnitude < 0.001f) // Si "up" est parallèle à AC, prend un autre axe
            axis = Vector3.Cross(Vector3.right, AC_normalized).normalized;

        // Rotation de AC pour positionner B
        Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis);
        return rotation * AC_normalized * value1;
    }

    float CosinusLaw(float distance1, float distance2, float maxDistance)
    {
        return (distance1 * distance1 + maxDistance * maxDistance - distance2 * distance2) / (2 * distance1 * maxDistance);
    }

    void GetDistance()
    {
        for (int i = 0; i < _bones.Length - 1; i++)
            _distances.Add((_bones[i].position - _bones[i + 1].position).magnitude);

        _distances.Add(_distances[_distances.Count - 1]);

        for (int i = 0; i < _distances.Count; i++)
            _maxBonesDistance += _distances[i];
    }

    void ResetValues()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}