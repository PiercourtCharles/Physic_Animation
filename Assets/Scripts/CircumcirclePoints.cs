using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircumcirclePoints : MonoBehaviour
{
    public Vector2 A = new Vector2(-3, 1);
    public Vector2 B = new Vector2(2, 4);
    public Vector2 C = new Vector2(4, -1);

    public Vector2 Center;
    public float Radius;
    public int numberOfPoints = 10; // Nombre de points à générer sur le cercle
    List<Vector2> _points = new();

    void Start()
    {
        ComputeCircumcircle(A, B, C);
        Debug.Log($"Center: {Center}, Radius: {Radius}");

        GeneratePointsOnCircle(numberOfPoints);
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
        for (int i = 0; i <= n+1; i++)
        {
            //float angle = (2 * Mathf.PI / n) * i; // Répartition uniforme des angles
            float angle = (Mathf.PI / n) * i;

            Vector2 point = new Vector2(
                Center.x + Radius * Mathf.Cos(angle),
                Center.y + Radius * Mathf.Sin(angle)
            );

            _points.Add(point);
            Debug.Log($"Point {i}: {point}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(A, 0.1f);
        Gizmos.DrawWireSphere(B, 0.1f);
        Gizmos.DrawWireSphere(C, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Center, Radius);

        Gizmos.color = Color.green;
        for (int i = 0; i < _points.Count-1; i++)
        {
            Gizmos.DrawWireSphere(_points[i], 0.1f);
        }
    }
}
