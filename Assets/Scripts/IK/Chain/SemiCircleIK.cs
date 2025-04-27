using System.Collections.Generic;
using UnityEngine;

public class SemiCircleIK : MonoBehaviour
{
    [SerializeField] Transform[] bones;   // Bones à manipuler
    [SerializeField] Transform target;    // Cible pour l'IK
    [SerializeField] float radius = 5f;   // Rayon du demi-cercle
    [SerializeField] Vector3 center;      // Centre du demi-cercle

    private void Start()
    {
        // Initialisation ou autres actions nécessaires
    }

    void Update()
    {
        SolveIK();
    }

    void SolveIK()
    {
        // Résoudre l'IK de manière classique
        // (Ceci peut être un système IK CCD ou FABRIK ici, selon ce que tu utilises)

        // Une fois les positions des bones résolues, on ajuste leur orientation
        ApplySemiCircleForm();
    }

    void ApplySemiCircleForm()
    {
        // On ajuste la position des bones sur un demi-cercle
        // Chaque bone sera positionné en utilisant un angle autour du centre pour former un demi-cercle
        int boneCount = bones.Length;
        float angleStep = Mathf.PI / (boneCount - 1);  // Diviser le demi-cercle en fonction du nombre de bones

        for (int i = 0; i < boneCount; i++)
        {
            // Calcul de l'angle correspondant pour chaque bone
            float angle = i * angleStep - Mathf.PI / 2; // Commencer à -90° pour un demi-cercle horizontal

            // Positionner chaque bone le long du demi-cercle
            float x = center.x + radius * Mathf.Cos(angle);
            float z = center.z + radius * Mathf.Sin(angle);

            // Positionner le bone sur l'arc
            bones[i].position = new Vector3(x, bones[i].position.y, z);

            // Optionnel: Assurez-vous que les bones suivent la courbe en regardant le centre du cercle
            Vector3 direction = (new Vector3(x, 0, z) - center).normalized;
            bones[i].rotation = Quaternion.LookRotation(direction);
        }
    }
}
