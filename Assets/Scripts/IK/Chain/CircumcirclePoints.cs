using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircumcirclePoints : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform C;

    public Vector2 Center;
    public float Radius;
    public int numberOfPoints = 10;

    [Header("Targets :")]
    [SerializeField] Transform[] _bones;

    [Header("Distances :")]
    [SerializeField] List<float> _distances = new();

    [SerializeField] List<Vector2> _points = new();
    [SerializeField] float _maxBonesDistance;
    [SerializeField] float AB;
    [SerializeField] float BC;
    [SerializeField] float AC;

    private void FixedUpdate()
    {
        Initialize();
    }

    void Initialize()
    {
        GetDistance();

        AB = 0;
        BC = 0;
        AC = 0;

        for (int i = 0; i < _distances.Count; i++)
        {
            if (i < _distances.Count / 2)
            {
                AB += _distances[i];
                BC += _distances[i];
            }

        }

        AC = (A.position - C.position).magnitude;
        B.position = A.position + Get3Point(A, C.position, AB, BC);

        ComputeCircumcircle(A.position, B.position, C.position);
        //Debug.Log($"Center: {Center}, Radius: {Radius}");

        GeneratePointsOnCircle(numberOfPoints);
        BonesAngles();
    }

    void ComputeCircumcircle(Vector2 A, Vector2 B, Vector2 C)
    {
        float D = 2 * (A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y));

        float Xc = ((A.x * A.x + A.y * A.y) * (B.y - C.y) +
                    (B.x * B.x + B.y * B.y) * (C.y - A.y) +
                    (C.x * C.x + C.y * C.y) * (A.y - B.y)) / D;

        float Yc = ((A.x * A.x + A.y * A.y) * (C.x - B.x) +
                    (B.x * B.x + B.y * B.y) * (A.x - C.x) +
                    (C.x * C.x + C.y * C.y) * (B.x - A.x)) / D;

        Center = new Vector2(Xc, Yc);
        Radius = Vector2.Distance(Center, A);
    }

    void GeneratePointsOnCircle(int n)
    {
        _points.Clear();

        for (int i = 0; i <= n + 1; i++)
        {
            //float angle = (2 * Mathf.PI / n) * i; // Répartition uniforme des angles
            float angleAction = CosinusLaw((A.position - (Vector3)Center).magnitude, (B.position - (Vector3)Center).magnitude, (C.position - A.position).magnitude);
            float angle = (angleAction * Mathf.PI / n) * i;

            float originOffset = CosinusLaw((A.position - (Vector3)Center).magnitude, (new Vector3(Center.x + Radius, Center.y, 0) - (Vector3)Center).magnitude, (C.position - A.position).magnitude);

            Vector2 point = new Vector2(
                Center.x + Radius * Mathf.Cos(angle - originOffset/2),
                Center.y + Radius * Mathf.Sin(angle - originOffset/2)
            );

            _points.Add(point);
            //Debug.Log($"Point {i}: {point}");
        }
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

        // Rotation de AC pour positionner _b
        Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis);
        return rotation * AC_normalized * value1;
    }

    float CosinusLaw(float distance1, float distance2, float maxDistance)
    {
        return (distance1 * distance1 + maxDistance * maxDistance - distance2 * distance2) / (2 * distance1 * maxDistance);
    }

    void BonesAngles()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].rotation = Quaternion.AngleAxis(Vector3.Angle(_points[i], _points[i + 1]), Vector3.forward);
        }
    }

    void GetDistance()
    {
        _distances.Clear();

        for (int i = 0; i < _bones.Length - 1; i++)
        {
            _distances.Add((_bones[i].position - _bones[i + 1].position).magnitude);
        }

        _distances.Add(_distances[_distances.Count - 1]);

        for (int i = 0; i < _distances.Count; i++)
        {
            _maxBonesDistance += _distances[i];
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(A.position, 0.1f);
    //    Gizmos.DrawWireSphere(_b.position, 0.1f);
    //    Gizmos.DrawWireSphere(C.position, 0.1f);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(Center, Radius);

    //    Gizmos.color = Color.green;
    //    for (int i = 0; i < _points.Count - 1; i++)
    //    {
    //        Gizmos.DrawWireSphere(_points[i], 0.1f);
    //    }
    //}
}
