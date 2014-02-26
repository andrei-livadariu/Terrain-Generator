using UnityEngine;
using System.Collections;

public class ScaleWater : MonoBehaviour
{
    private TerrainData _terrain;

    private void Awake()
    {
        _terrain = GameObject.FindObjectOfType<Terrain>().terrainData;
    }

    private void Update()
    {
        transform.localScale = new Vector3(_terrain.size[0] * 0.02f, 1f, _terrain.size[2] * 0.01f);
        transform.localPosition = new Vector3(transform.localScale.x * 25f, 40f, transform.localScale.z * 50f);
    }
}
