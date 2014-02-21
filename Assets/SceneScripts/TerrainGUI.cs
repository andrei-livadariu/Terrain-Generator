using UnityEngine;
using System;
using System.Collections;

public class TerrainGUI : MonoBehaviour
{
    private const float ToolbarWidth = 300f;
    private const float SlowIterateTime = 1f;

    public IterativeTerrainGenerator Generator
    {
        get;
        protected set;
    }

    public DiamondSquareParameters Parameters
    {
        get;
        protected set;
    }

    private bool _isInAnimation = false;

    private void Awake()
    {
        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        Parameters = new DiamondSquareParameters();
        Generator = new IterativeTerrainGenerator(terrain.terrainData, new DiamondSquareAlgorithm(Parameters));
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, ToolbarWidth, Screen.height));

        GUILayout.Box("Terrain parameters");
        SliderWithLabel("Randomness", ref Parameters.variation, 0f, 1f);
        SliderWithLabel("Smoothness", ref Parameters.smoothness, 0.7f, 1f);
        SliderWithLabel("Height multiplier", ref Parameters.heightScaling, 0f, 1f);
        
        GUILayout.Space(20f);

        GUILayout.Box("Seeds");
        SliderWithLabel("Outside height", ref Parameters.outsideHeight, 0f, 1f);
        SliderWithLabel("North-west corner", ref Parameters.seeds[0], 0f, 1f);
        SliderWithLabel("North-east corner", ref Parameters.seeds[1], 0f, 1f);
        SliderWithLabel("South-west corner", ref Parameters.seeds[2], 0f, 1f);
        SliderWithLabel("South-east corner", ref Parameters.seeds[3], 0f, 1f);

        GUILayout.Space(20f);

        GUILayout.Box("Actions");
        SliderWithLabel("Iterations", ref Parameters.nrIterations, 1, 12);

        if (GUILayout.Button("Generate"))
        {
            Generator.Generate();
        }

        GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
                if (GUILayout.Button("Iterate"))
                {
                    Generator.Iterate();
                }
                if (GUILayout.Button("Animate") && !_isInAnimation)
                {
                    StartCoroutine(IterateCoroutine());
                }
            GUILayout.EndVertical();
            GUILayout.Box("Current iteration: " + Generator.CurrentIteration);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private IEnumerator IterateCoroutine()
    {
        _isInAnimation = true;
        do
        {
            Generator.Iterate();
            yield return new WaitForSeconds(SlowIterateTime);
        } while (Generator.CurrentIteration < Parameters.nrIterations);
        _isInAnimation = false;
    }

    private void SliderWithLabel(string label, ref float parameter, float minValue, float maxValue)
    {
        GUILayout.BeginHorizontal();
            GUILayout.Box(label + ": " + parameter, GUILayout.Width(ToolbarWidth / 2));
            parameter = (float)Math.Round(GUILayout.HorizontalSlider(parameter, minValue, maxValue), 2);
        GUILayout.EndHorizontal();
    }

    private void SliderWithLabel(string label, ref int parameter, float minValue, float maxValue)
    {
        GUILayout.BeginHorizontal();
            GUILayout.Box(label + ": " + parameter, GUILayout.Width(ToolbarWidth / 2));
            parameter = (int)Math.Floor(GUILayout.HorizontalSlider(parameter, minValue, maxValue));
        GUILayout.EndHorizontal();
    }
}
