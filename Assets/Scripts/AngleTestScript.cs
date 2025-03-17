using UnityEngine;
using UnityEngine.UIElements;

public class AngleTestScript : MonoBehaviour
{
    [SerializeField][Tooltip("Ugly button")] bool _Actualize = false;
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
        _c = (_objC.position - _objA.position).magnitude;

        var difference = _objC.position - _objA.position;
        var pythagore = Mathf.Sqrt(Mathf.Pow(_a, 2)) + Mathf.Sqrt(Mathf.Pow(_c, 2));
        Vector3 posB = _objA.position + (difference * pythagore).normalized * difference.magnitude / 2; //Aled a revoir

        var a = CalculAngle(_a, _b, _c);
        var b = CalculAngle(_b, _c, _a);
        var c = CalculAngle(_c, _a, _b);

        _resultA = ToDegree(a);
        _resultB = ToDegree(b);
        _resultC = ToDegree(c);

        _boneA.Rotate(Vector3.back, 180 - _resultA);
        _boneB.Rotate(Vector3.back, 180 - _resultB);
        //_boneC.Rotate(Vector3.back, 180 - _resultC);
        _target.position = posB;
    }

    void ResetValues()
    {
        _boneA.rotation = Quaternion.Euler(0, 0, 0);
        _boneB.rotation = Quaternion.Euler(0, 0, 0);
        _boneC.rotation = Quaternion.Euler(0, 0, 0);
    }

    float CalculAngle(float a, float b, float c)
    {
        return Mathf.Round((Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(c, 2)) / (2 * b * c) * 10) / 10;
    }

    float ToDegree(float number)
    {
        return Mathf.Round(((180 * Mathf.Acos(number)) / Mathf.PI) * 10) / 10;
    }
}
