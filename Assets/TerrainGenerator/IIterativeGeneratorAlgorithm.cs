public interface IIterativeGeneratorAlgorithm : IGeneratorAlgorithm
{
    int CurrentIteration { get; }
    int IntermediateResolution { get; }
    float[,] IntermediateHeights { get; }

    void Iterate();
}
