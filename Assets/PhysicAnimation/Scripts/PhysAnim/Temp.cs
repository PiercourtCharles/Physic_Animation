using UnityEngine;

public class Temp : MonoBehaviour
{
    public AutoPhysicRig Auto;
    public float Value = 1;

    void Blend()
    {
        if (0 < Value && Value < 1)
            Auto.BlendRagdoll(Value);
    }

    public void ValueChange(float value)
    {
        Value += value;
        Blend();
    }
}
