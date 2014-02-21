using UnityEngine;
using System.Collections;

public interface IGeneratorAlgorithm
{
    int Resolution { get; }
    float[,] Heights { get; }

    void Generate();
}
