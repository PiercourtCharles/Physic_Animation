using UnityEngine;

public class CCD_IK : MonoBehaviour
{
    public Transform target; // Cible à atteindre
    public Transform[] bones; // Bones du bout vers la base
    public int maxIterations = 10; // Nombre max d'itérations
    public float threshold = 0.001f; // Distance minimale avant d’arrêter
    public float damping = 0.8f; // Facteur d’adoucissement (0.1 à 1.0)

    void Update()
    {
        SolveIK();
    }

    void SolveIK()
    {
        if (bones == null || bones.Length == 0 || target == null)
            return;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Transform effector = bones[0];

            // Vérifier si l’effector est proche de la cible
            if (Vector3.Distance(effector.position, target.position) < threshold)
                break;

            // Parcourir les bones du dernier vers le premier
            for (int i = 1; i < bones.Length; i++)
            {
                Transform bone = bones[i];

                // Vecteurs avant/après
                Vector3 toEffector = effector.position - bone.position;
                Vector3 toTarget = target.position - bone.position;

                // Vérifier si les vecteurs sont valides (éviter des erreurs)
                if (toEffector.sqrMagnitude < 0.0001f || toTarget.sqrMagnitude < 0.0001f)
                    continue;

                // Trouver la rotation nécessaire
                Quaternion rotationNeeded = Quaternion.FromToRotation(toEffector, toTarget);
                bone.rotation = Quaternion.Slerp(Quaternion.identity, rotationNeeded, damping) * bone.rotation;
            }
        }
    }
}
