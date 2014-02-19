using UnityEngine;
using System;
using System.Collections;

public class TerrainGUI : MonoBehaviour
{
    private const float ToolbarWidth = 300f;

    private DynamicTerrain terrain;

    private void Awake()
    {
        terrain = GameObject.FindObjectOfType<DynamicTerrain>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, ToolbarWidth, Screen.height));

        GUILayout.Box("Terrain controls");
        SliderWithLabel("Randomness", ref terrain.Parameters.variation, 0f, 1f);
        SliderWithLabel("Smoothness", ref terrain.Parameters.smoothness, 0.7f, 1f);
        SliderWithLabel("Height multiplier", ref terrain.Parameters.heightScaling, 0f, 1f);
        
        GUILayout.Space(20f);

        GUILayout.Box("Seeds");
        SliderWithLabel("Outside height", ref terrain.Parameters.outsideHeight, 0f, 1f);
        SliderWithLabel("North-west corner", ref terrain.Parameters.seeds[0], 0f, 1f);
        SliderWithLabel("North-east corner", ref terrain.Parameters.seeds[1], 0f, 1f);
        SliderWithLabel("South-west corner", ref terrain.Parameters.seeds[2], 0f, 1f);
        SliderWithLabel("South-east corner", ref terrain.Parameters.seeds[3], 0f, 1f);

        GUILayout.Space(20f);

        GUILayout.Box("Actions");
        SliderWithLabel("Iterations", ref terrain.Parameters.nrIterations, 1, 12);

        if (GUILayout.Button("Generate"))
        {
            terrain.Generate();
        }

        GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
                if (GUILayout.Button("Iterate"))
                {
                    terrain.Iterate();
                }
                if (GUILayout.Button("Animate"))
                {
                    terrain.SlowIterate();
                }
            GUILayout.EndVertical();
            GUILayout.Box("Current iteration: " + terrain.CurrentIteration);
        GUILayout.EndHorizontal();


        GUILayout.EndArea();
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
