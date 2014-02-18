using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
public class CenterTerrain : MonoBehaviour
{
    private Terrain _terrain;

    private void Awake()
    {
        _terrain = GetComponent<Terrain>();
    }

    private void Update()
    {
        // Keep the terrain centered
        transform.position = -_terrain.terrainData.size / 2;
    }
}
