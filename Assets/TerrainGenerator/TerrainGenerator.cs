using UnityEngine;
using System.Collections;

public class TerrainGenerator
{
    public float ResolutionError
    {
        get;
        protected set;
    }

    protected TerrainData terrain;
    protected IGeneratorAlgorithm generator;

    public TerrainGenerator(TerrainData terrain, IGeneratorAlgorithm generator)
    {
        this.terrain = terrain;
        this.generator = generator;
    }

    public void Generate()
    {
        generator.Generate();
        ScaleTerrain(generator.Resolution, generator.Heights);
    }

    protected void ScaleTerrain(int resolution, float[,] heights)
    {
        terrain.heightmapResolution = resolution;
        // TerrainData keeps a minimum resolution of 33 even though we are setting it to a smaller number
        // We are taking this into account when scaling and centering the terrain so that we maintain a constant scale
        ResolutionError = (terrain.heightmapResolution - 1f) / (resolution - 1f);
        
        terrain.SetHeights(0, 0, heights);

        float resolutionScale = (generator.Resolution - 1) / 32.0f;
        Vector3 scaleVector = new Vector3(125.0f * resolutionScale, 600.0f, 125.0f * resolutionScale);

        if (ResolutionError != 0)
        {
            scaleVector.x *= ResolutionError;
            scaleVector.z *= ResolutionError;
        }

        terrain.size = scaleVector;
    }
}
