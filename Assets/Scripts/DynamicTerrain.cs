using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class DynamicTerrain : MonoBehaviour
{
    private const float SlowIterateTime = 1f;

    private DiamondSquareParameters _parameters = new DiamondSquareParameters();
    public DiamondSquareParameters Parameters
    {
        get { return _parameters; }
    }

    private int _currentIteration = 0;
    public int CurrentIteration
    {
        get { return _currentIteration; }
    }
    
    private TerrainData _terrain;
    private DiamondSquare _generator;
    private float _resolutionError = 0f;

    private void Awake()
    {
        _terrain = GetComponent<Terrain>().terrainData;
    }

    private void Update()
    {
        Vector3 offset = _terrain.size;
        if (_resolutionError != 0)
        {
            float errorCorrection = 1f - 1f / _resolutionError;
            offset.x -= _terrain.size.x * errorCorrection;
            offset.z -= _terrain.size.z * errorCorrection;
        }
        transform.position = -offset / 2;
    }

    public void Generate()
    {
        _currentIteration = 0;
        _resolutionError = 0f;
        _generator = new DiamondSquare(_parameters);
        _terrain.heightmapResolution = _generator.Resolution;
        _generator.Generate();
        _terrain.SetHeights(0, 0, _generator.Heights);
        float resolutionScale = (_terrain.heightmapResolution - 1) / 32.0f;
        _terrain.size = new Vector3(125.0f * (resolutionScale), 600.0f, 125.0f * (resolutionScale));
    }

    public void Iterate()
    {
        if (_currentIteration == 0)
        {
            _generator = new DiamondSquare(_parameters);
        }
        _generator.Iterate();
        _terrain.heightmapResolution = _generator.IntermediateResolution;
        _terrain.SetHeights(0, 0, _generator.IntermediateHeights);

        float resolutionScale = (_generator.Resolution - 1) / 32.0f;
        Vector3 scaleVector = new Vector3(125.0f * (resolutionScale), 600.0f, 125.0f * (resolutionScale));

        _resolutionError = (_terrain.heightmapResolution - 1f) / (_generator.IntermediateResolution - 1f);
        if (_resolutionError != 0)
        {
            scaleVector.x *= _resolutionError;
            scaleVector.z *= _resolutionError;
        }

        _terrain.size = scaleVector;

        _currentIteration = (_currentIteration + 1) % _parameters.nrIterations;
    }

    public void SlowIterate()
    {
        StartCoroutine(IterateCoroutine());
    }

    private IEnumerator IterateCoroutine()
    {
        do
        {
            Iterate();
            yield return new WaitForSeconds(SlowIterateTime);
        } while (_currentIteration != 0);
    }
}
