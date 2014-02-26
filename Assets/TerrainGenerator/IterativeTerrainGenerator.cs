using UnityEngine;
using System.Collections;

public class IterativeTerrainGenerator : TerrainGenerator
{
    public int CurrentIteration
    {
        get { return (generator as IIterativeGeneratorAlgorithm).CurrentIteration; }
    }

    public IterativeTerrainGenerator(TerrainData terrain, IIterativeGeneratorAlgorithm generator)
        : base(terrain, generator)
    {
    }

    public void Iterate()
    {
        IIterativeGeneratorAlgorithm iterativeGenerator = generator as IIterativeGeneratorAlgorithm;
        iterativeGenerator.Iterate();
        ScaleTerrain(iterativeGenerator.IntermediateResolution, iterativeGenerator.IntermediateHeights);
        if (OnTerrainGenerated != null)
        {
            OnTerrainGenerated(terrain, resolutionError);
        }
    }
}
