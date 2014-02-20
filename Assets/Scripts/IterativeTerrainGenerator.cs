using UnityEngine;
using System.Collections;

public class IterativeTerrainGenerator : TerrainGenerator
{
    public int CurrentIteration
    {
        get { return generator.CurrentIteration; }
    }

    public IterativeTerrainGenerator(TerrainData terrain, DiamondSquare generator)
        : base(terrain, generator)
    {
    }

    public void Iterate()
    {
        generator.Iterate();
        ScaleTerrain(generator.IntermediateResolution, generator.IntermediateHeights);
    }
}
