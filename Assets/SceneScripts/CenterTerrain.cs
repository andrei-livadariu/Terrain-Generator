using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class CenterTerrain : MonoBehaviour
{
    public void Start()
    {
        GameObject.FindObjectOfType<TerrainGUI>().Generator.OnTerrainGenerated += Recenter;
    }

    private void Recenter(TerrainData terrain, float resolutionError)
    {
        // Keep the terrain centered
        Vector3 offset = terrain.size;
        if (resolutionError != 0)
        {
            float errorCorrection = 1f - 1f / resolutionError;
            offset.x -= terrain.size.x * errorCorrection;
            offset.z -= terrain.size.z * errorCorrection;
        }
        transform.position = -offset / 2;
    }
}
