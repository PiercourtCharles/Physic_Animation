using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AngleTestScript : MonoBehaviour
{
    //[SerializeField][Tooltip("Ugly button")] bool _Actualize = false;
    [SerializeField] Transform _target;
    [Header("Targets :")]
    [SerializeField] Transform _boneA;
    [SerializeField] Transform _boneB, _boneC;
    [Header("Objects :")]
    [SerializeField] Transform _objA;
    [SerializeField] Transform _objB, _objC;
    [Header("Distances :")]
    [SerializeField] float _a;
    [SerializeField] float _b, _c;
    [Header("Results :")]
    [SerializeField] float _resultA;
    [SerializeField] float _resultB, _resultC;

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        //if (_Actualize)
        //{
        ResetValues();
        Initialize();
        //    _Actualize = false;
        //}
    }

    void Initialize()
    {
        // Vecteur AC && normalisation de AC
        Vector3 AC = _objC.position - _objA.position;
        float distanceAC = AC.magnitude;
        distanceAC = Mathf.Clamp(distanceAC, 0.0001f, _a + _b - 0.0001f); // Clamp la distance pour éviter des erreurs si la cible est trop loin
        Vector3 AC_normalized = AC.normalized;

        // Loi des cosinus pour trouver l'angle au coude
        float cosTheta = (_a * _a + distanceAC * distanceAC - _b * _b) / (2 * _a * distanceAC);
        float theta = Mathf.Acos(cosTheta); // Angle en radians

        // Trouver un vecteur perpendiculaire à AC
        Vector3 axis = Vector3.Cross(Vector3.up, AC_normalized); // Utilise "up" par défaut
        if (axis.magnitude < 0.001f) // Si "up" est parallèle à AC, prend un autre axe
            axis = Vector3.Cross(Vector3.right, AC_normalized);
        axis.Normalize();

        // Rotation de AC pour positionner B
        Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis);
        Vector3 AB = rotation * AC_normalized * _a;

        _target.position = _objA.position + AB;

        _boneA.LookAt(_objB.position);
        _boneB.LookAt(_objC.position);
    }

    void ResetValues()
    {
        _boneA.rotation = Quaternion.Euler(0, 0, 0);
        _boneB.rotation = Quaternion.Euler(0, 0, 0);
        _boneC.rotation = Quaternion.Euler(0, 0, 0);
    }
}
