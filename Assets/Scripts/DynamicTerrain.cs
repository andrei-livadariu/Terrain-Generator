using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class DynamicTerrain : MonoBehaviour
{
    private TerrainData _terrain;
    private IterativeTerrainGenerator _generator;

    public void Awake()
    {
        _terrain = GetComponent<Terrain>().terrainData;
        _generator = GameObject.FindObjectOfType<TerrainGUI>().Generator;
    }

    private void Update()
    {
        // Keep the terrain centered
        Vector3 offset = _terrain.size;
        if (_generator.ResolutionError != 0)
        {
            float errorCorrection = 1f - 1f / _generator.ResolutionError;
            offset.x -= _terrain.size.x * errorCorrection;
            offset.z -= _terrain.size.z * errorCorrection;
        }
        transform.position = -offset / 2;
    }
}
