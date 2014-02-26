using System;
using UnityEngine;

public class TerrainGenerator
{
    public Action<TerrainData, float> OnTerrainGenerated;

    protected TerrainData terrain;
    protected IGeneratorAlgorithm generator;
    protected float resolutionError;

    public TerrainGenerator(TerrainData terrain, IGeneratorAlgorithm generator)
    {
        this.terrain = terrain;
        this.generator = generator;
    }

    public void Generate()
    {
        generator.Generate();
        ScaleTerrain(generator.Resolution, generator.Heights);
        if (OnTerrainGenerated != null)
        {
            OnTerrainGenerated(terrain, resolutionError);
        }
    }

    protected void ScaleTerrain(int resolution, float[,] heights)
    {
        terrain.heightmapResolution = resolution;
        // TerrainData keeps a minimum resolution of 33 even though we are setting it to a smaller number
        // We are taking this into account when scaling and centering the terrain so that we maintain a constant scale
        resolutionError = (terrain.heightmapResolution - 1f) / (resolution - 1f);
        
        terrain.SetHeights(0, 0, heights);

        float resolutionScale = (generator.Resolution - 1) / 32.0f;
        Vector3 scaleVector = new Vector3(125.0f * resolutionScale, 600.0f, 125.0f * resolutionScale);

        if (resolutionError > 1)
        {
            scaleVector.x *= resolutionError;
            scaleVector.z *= resolutionError;
        }

        terrain.size = scaleVector;
        PaintTerrain(heights);
    }

    // Paint the terrain based on height and the textures it has loaded
    // Based on the code from: http://answers.unity3d.com/questions/12835/how-to-automatically-apply-different-textures-on-t.html
    protected void PaintTerrain(float[,] heights)
    {
        // The structure that holds all the splat-related data
        float[, ,] splatmapData = new float[terrain.alphamapWidth, terrain.alphamapHeight, terrain.alphamapLayers];

        // The splat map is always 512 x 512, so we need to scale that to whatever resolution our terrain is
        float xCorrection = (heights.GetLength(0) - 1f) / terrain.alphamapWidth;
        float yCorrection = (heights.GetLength(1) - 1f) / terrain.alphamapHeight;

        // The height interval that each texture represents (alphamapLayers = number of textures)
        float textureInterval = 1f / (terrain.alphamapLayers - 1);

        // A number that represents which textures and which blending weights are present at a particular step
        float textureBlend;

        // The texture ID that corresponds to a particular height
        int textureId;
        
        // Represents (1 - blending weight of the current texture), also blending weight of the next texture
        float textureWeight;
        
        // The array that holds the blend weight for each texture
        float[] splat;

        // Iteration variables
        int z, y, x;
        for (y = 0; y < terrain.alphamapHeight; y++)
        {
            for (x = 0; x < terrain.alphamapWidth; x++)
            {
                splat = new float[terrain.alphamapLayers];

                textureBlend = Mathf.Clamp01(heights[(int)(x * xCorrection), (int)(y * yCorrection)]) / textureInterval;
                textureId = (int)textureBlend;
                textureWeight = textureBlend - textureId;

                if (textureWeight < 0.2f)
                {
                    textureWeight = 0f;
                }
                else
                {
                    textureWeight = Mathf.InverseLerp(0.2f, 1f, textureWeight);
                }

                splat[textureId] = 1.0f - textureWeight;
                if (textureId < terrain.alphamapLayers - 1)
                {
                    splat[textureId + 1] = textureWeight;
                }

                // now assign the values to the correct location in the array
                for (z = 0; z < terrain.alphamapLayers; ++z)
                {
                    splatmapData[(int)(x / resolutionError), (int)(y / resolutionError), z] = splat[z];
                }
            }
        }

        // Assign the splat map to the terrain data
        terrain.SetAlphamaps(0, 0, splatmapData);
    }
}
