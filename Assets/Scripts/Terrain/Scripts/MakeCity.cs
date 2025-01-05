yusing System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCityGenerator : MonoBehaviour
{
    public GameObject[] largeBuildings; // Liste des grands b�timents
    public GameObject[] smallBuildings; // Liste des petits b�timents

    public ParticleSystem smokeEffect; // Effet de fum�e (Prefab Particle System)

    public float perlinScaleLarge = 20f; // �chelle du Perlin Noise pour grands b�timents
    public float perlinScaleSmall = 10f; // �chelle du Perlin Noise pour petits b�timents

    public float heightThresholdLarge = 0.8f; // Seuil pour grands b�timents
    public float heightThresholdSmall = 0.5f; // Seuil pour petits b�timents

    public float tiltAngle = 15f; // Angle maximum d'inclinaison des grands b�timents

    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        // R�cup�rer le maillage de l'objet Plane
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // Calculer la position et la taille
        Vector3 planePosition = transform.position;
        Vector3 planeScale = transform.localScale;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Convertir la position locale en position globale
            Vector3 worldPos = transform.TransformPoint(vertices[i]);

            // Calculer les valeurs de Perlin Noise
            float perlinValueLarge = Mathf.PerlinNoise(worldPos.x / perlinScaleLarge, worldPos.z / perlinScaleLarge);
            float perlinValueSmall = Mathf.PerlinNoise(worldPos.x / perlinScaleSmall, worldPos.z / perlinScaleSmall);

            // D�terminer si un grand b�timent doit �tre plac�
            if (perlinValueLarge > heightThresholdLarge)
            {
                // S�lectionner un grand b�timent al�atoire
                int buildingIndex = Random.Range(0, largeBuildings.Length);
                Vector3 spawnPos = new Vector3(worldPos.x, planePosition.y, worldPos.z);

                // Incliner al�atoirement les grands b�timents
                Quaternion randomTilt = Quaternion.Euler(Random.Range(-tiltAngle, tiltAngle), Random.Range(0, 360), Random.Range(-tiltAngle, tiltAngle));
                GameObject building = Instantiate(largeBuildings[buildingIndex], spawnPos, randomTilt);

                // Ajouter l'effet de fum�e uniquement sur les grands b�timents
                AddSmokeEffect(building);
            }
            // Sinon, placer un petit b�timent
            else if (perlinValueSmall > heightThresholdSmall)
            {
                int buildingIndex = Random.Range(0, smallBuildings.Length);
                Vector3 spawnPos = new Vector3(worldPos.x, planePosition.y, worldPos.z);
                Instantiate(smallBuildings[buildingIndex], spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
    }

    // Fonction pour ajouter un effet de fum�e uniquement sur les grands b�timents
    void AddSmokeEffect(GameObject building)
    {
        if (smokeEffect != null)
        {
            // Cr�e une instance du syst�me de particules
            ParticleSystem smokeInstance = Instantiate(smokeEffect, building.transform);

            // Positionner la fum�e en haut du b�timent
            Renderer renderer = building.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                Vector3 smokePosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                smokeInstance.transform.position = smokePosition;
            }

            // Activer la fum�e
            smokeInstance.Play();
        }
    }
}
