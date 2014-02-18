using UnityEngine;
using System.Collections;

[System.Serializable]
public class DiamondSquareParameters
{
    public float variation = 0.5f;
    public float smoothness = 0.9f;
    public float outsideHeight = 0.0f;
    public float heightScaling = 0.8f;
    public float[] seeds = { 0.0f, 0.0f, 0.0f, 0.0f };
    public int nrIterations = 9;
}
