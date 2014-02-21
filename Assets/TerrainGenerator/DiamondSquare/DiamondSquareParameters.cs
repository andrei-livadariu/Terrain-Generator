using UnityEngine;
using System.Collections;

[System.Serializable]
public class DiamondSquareParameters
{
    public float variation = 1f;
    public float smoothness = 0.9f;
    public float outsideHeight = 0.5f;
    public float heightScaling = 0.8f;
    public float[] seeds = { 0.5f, 0.5f, 0.5f, 0.5f };
    public int nrIterations = 9;
}
