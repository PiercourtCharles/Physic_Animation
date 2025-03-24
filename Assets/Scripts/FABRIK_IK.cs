using UnityEngine;

public class FABRIK_IK : MonoBehaviour
{
    public Transform target; // Cible à atteindre
    public Transform[] bones; // Liste des bones (de la base vers l’effector)
    public int maxIterations = 10; // Nombre d’itérations max
    public float threshold = 0.001f; // Tolérance d’arrêt

    private float[] boneLengths; // Stocke les longueurs des segments
    private float totalLength; // Longueur totale de la chaîne

    void Start()
    {
        InitBones();
    }

    void Update()
    {
        SolveIK();
    }

    void InitBones()
    {
        int numBones = bones.Length;
        boneLengths = new float[numBones - 1]; // N-1 segments
        totalLength = 0f;

        for (int i = 0; i < numBones - 1; i++)
        {
            boneLengths[i] = Vector3.Distance(bones[i].position, bones[i + 1].position);
            totalLength += boneLengths[i];
        }
    }

    void SolveIK()
    {
        if (bones.Length == 0 || target == null)
            return;

        // Vérifier si la cible est atteignable
        float targetDistance = Vector3.Distance(bones[0].position, target.position);
        if (targetDistance > totalLength)
        {
            // Étirement maximal : aligner tous les bones vers la cible
            for (int i = 0; i < bones.Length - 1; i++)
            {
                Vector3 direction = (target.position - bones[i].position).normalized;
                bones[i + 1].position = bones[i].position + direction * boneLengths[i];
            }
        }
        else
        {
            // Sinon, exécuter FABRIK

            Vector3 basePosition = bones[0].position;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // 1️⃣ Backward Pass : On commence par mettre l’effector sur la cible
                bones[bones.Length - 1].position = target.position;

                for (int i = bones.Length - 2; i >= 0; i--)
                {
                    Vector3 direction = (bones[i].position - bones[i + 1].position).normalized;
                    bones[i].position = bones[i + 1].position + direction * boneLengths[i];
                }

                // 2️⃣ Forward Pass : On replace la base et ajuste les bones
                bones[0].position = basePosition;

                for (int i = 1; i < bones.Length; i++)
                {
                    Vector3 direction = (bones[i].position - bones[i - 1].position).normalized;
                    bones[i].position = bones[i - 1].position + direction * boneLengths[i];
                }

                // Vérification de la convergence
                if (Vector3.Distance(bones[bones.Length - 1].position, target.position) < threshold)
                    break;
            }
        }

        // Mise à jour des rotations pour aligner les bones
        for (int i = 0; i < bones.Length - 1; i++)
        {
            Vector3 direction = (bones[i + 1].position - bones[i].position).normalized;
            bones[i].rotation = Quaternion.LookRotation(direction);
        }
    }
}
