yusing System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCityGenerator : MonoBehaviour
{
    public GameObject[] largeBuildings; // Liste des grands bâtiments
    public GameObject[] smallBuildings; // Liste des petits bâtiments

    public ParticleSystem smokeEffect; // Effet de fumée (Prefab Particle System)

    public float perlinScaleLarge = 20f; // Échelle du Perlin Noise pour grands bâtiments
    public float perlinScaleSmall = 10f; // Échelle du Perlin Noise pour petits bâtiments

    public float heightThresholdLarge = 0.8f; // Seuil pour grands bâtiments
    public float heightThresholdSmall = 0.5f; // Seuil pour petits bâtiments

    public float tiltAngle = 15f; // Angle maximum d'inclinaison des grands bâtiments

    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        // Récupérer le maillage de l'objet Plane
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

            // Déterminer si un grand bâtiment doit être placé
            if (perlinValueLarge > heightThresholdLarge)
            {
                // Sélectionner un grand bâtiment aléatoire
                int buildingIndex = Random.Range(0, largeBuildings.Length);
                Vector3 spawnPos = new Vector3(worldPos.x, planePosition.y, worldPos.z);

                // Incliner aléatoirement les grands bâtiments
                Quaternion randomTilt = Quaternion.Euler(Random.Range(-tiltAngle, tiltAngle), Random.Range(0, 360), Random.Range(-tiltAngle, tiltAngle));
                GameObject building = Instantiate(largeBuildings[buildingIndex], spawnPos, randomTilt);

                // Ajouter l'effet de fumée uniquement sur les grands bâtiments
                AddSmokeEffect(building);
            }
            // Sinon, placer un petit bâtiment
            else if (perlinValueSmall > heightThresholdSmall)
            {
                int buildingIndex = Random.Range(0, smallBuildings.Length);
                Vector3 spawnPos = new Vector3(worldPos.x, planePosition.y, worldPos.z);
                Instantiate(smallBuildings[buildingIndex], spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
    }

    // Fonction pour ajouter un effet de fumée uniquement sur les grands bâtiments
    void AddSmokeEffect(GameObject building)
    {
        if (smokeEffect != null)
        {
            // Crée une instance du système de particules
            ParticleSystem smokeInstance = Instantiate(smokeEffect, building.transform);

            // Positionner la fumée en haut du bâtiment
            Renderer renderer = building.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                Vector3 smokePosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                smokeInstance.transform.position = smokePosition;
            }

            // Activer la fumée
            smokeInstance.Play();
        }
    }
}
