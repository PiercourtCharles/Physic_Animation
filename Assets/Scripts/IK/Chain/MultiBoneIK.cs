using UnityEngine;

public class MultiBoneIK : MonoBehaviour
{
    [SerializeField] Transform[] bones;   // Les bones qui forment la chaîne de l'IK
    [SerializeField] Transform target;    // La cible que les bones doivent atteindre
    [SerializeField] float tolerance = 0.1f;  // La tolérance pour la fin de l'IK

    void Update()
    {
        SolveIK();
    }

    void SolveIK()
    {
        // Résoudre l'IK avec l'algorithme CCD
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            // Calculer la direction vers la cible
            Vector3 directionToTarget = target.position - bones[i].position;

            // Si c'est le dernier bone, on calcule la direction directe
            if (i == bones.Length - 1)
            {
                bones[i].LookAt(target);
            }
            else
            {
                // Calculer la direction inverse pour le reste des bones
                Vector3 directionToNextBone = bones[i + 1].position - bones[i].position;
                float angle = Vector3.SignedAngle(directionToNextBone, directionToTarget, Vector3.up);

                // Appliquer la rotation du bone pour orienter dans la direction de la cible
                bones[i].rotation = Quaternion.Euler(0, angle, 0) * bones[i].rotation;
            }

            // Vérifier si la distance entre le dernier bone et la cible est suffisante
            if (Vector3.Distance(bones[bones.Length - 1].position, target.position) < tolerance)
            {
                break;
            }
        }
    }
}
