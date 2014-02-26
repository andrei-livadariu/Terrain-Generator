using UnityEngine;

public class ScaleWater : MonoBehaviour
{
    private void Start()
    {
        GameObject.FindObjectOfType<TerrainGUI>().Generator.OnTerrainGenerated += Rescale;
    }

    private void Rescale(TerrainData terrain, float resolutionError)
    {
        transform.localScale = new Vector3(terrain.size[0] * 0.02f, 1f, terrain.size[2] * 0.01f);
        transform.localPosition = new Vector3(transform.localScale.x * 25f, transform.localPosition.y, transform.localScale.z * 50f);
    }
}
