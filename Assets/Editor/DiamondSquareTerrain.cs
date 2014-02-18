using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class DiamondSquareTerrain : EditorWindow
{
    DiamondSquareParameters parameters = new DiamondSquareParameters();
    private DiamondSquare generator;
    private int currentIteration = 0;

    [MenuItem("Window/TerrainGenerator")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(DiamondSquareTerrain));
    }

    TerrainData currentSelection;

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {

        //Rect nw = GUILayoutUtility.GetLastRect();
        //Rect areaRect = new Rect(nw.x,nw.yMax +30,nw.width,nw.width);

        parameters.variation = EditorGUILayout.Slider("Variation", (float)Math.Round(parameters.variation, 2), 0f, 1.0f);
        parameters.smoothness = EditorGUILayout.Slider("Smoothness", (float)Math.Round(parameters.smoothness, 2), 0.7f, 1.0f);
        parameters.heightScaling = EditorGUILayout.Slider("Height scaling", (float)Math.Round(parameters.heightScaling, 2), 0f, 1.0f);
        parameters.outsideHeight = EditorGUILayout.Slider("Outside height", (float)Math.Round(parameters.outsideHeight, 2), 0f, 1.0f);

        EditorGUILayout.LabelField("Seeds");
        parameters.seeds[0] = EditorGUILayout.Slider("North-west", (float)Math.Round(parameters.seeds[0], 2), 0f, 1.0f);
        parameters.seeds[1] = EditorGUILayout.Slider("North-east", (float)Math.Round(parameters.seeds[1], 2), 0f, 1.0f);
        parameters.seeds[2] = EditorGUILayout.Slider("South-west", (float)Math.Round(parameters.seeds[2], 2), 0f, 1.0f);
        parameters.seeds[3] = EditorGUILayout.Slider("South-east", (float)Math.Round(parameters.seeds[3], 2), 0f, 1.0f);

        parameters.nrIterations = EditorGUILayout.IntSlider("Number of iterations", parameters.nrIterations, 1, 12);

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.LookLikeControls(60, 20);
        if (GUILayout.Button("Generate"))
            Generate();

        if (GUILayout.Button("Iterate"))
            Iterate();
        EditorGUILayout.LabelField("Iteration", currentIteration.ToString());
        EditorGUILayout.EndHorizontal();

        this.Repaint();
    }

    void Update()
    {
        if (Terrain.activeTerrain != null)
        {
            currentSelection = Terrain.activeTerrain.terrainData;
        }
    }

    void Generate()
    {
        if (currentSelection == null)
            return;

        generator = new DiamondSquare(parameters);
        currentSelection.heightmapResolution = generator.Resolution;
        generator.Generate();
        currentSelection.SetHeights(0, 0, generator.GetHeights());
        float resolutionScale = (currentSelection.heightmapResolution - 1) / 32.0f;
        currentSelection.size = new Vector3(125.0f * (resolutionScale), 600.0f, 125.0f * (resolutionScale));
        currentIteration = 0;
    }

    void Iterate()
    {
        if (currentIteration == 0)
        {
            generator = new DiamondSquare(parameters);
        }
        generator.Iterate();
        currentSelection.heightmapResolution = generator.IntermediateResolution;
        currentSelection.SetHeights(0, 0, generator.GetIntermediateHeights());

        int iResolution = generator.IntermediateResolution - 1;
        float resolutionScale = (generator.Resolution - 1) / 32.0f;
        Vector3 scaleVector = new Vector3(125.0f * (resolutionScale), 600.0f, 125.0f * (resolutionScale));
        if (iResolution < 33)
        {
            float scaleRatio = 32.0f / iResolution;
            scaleVector.x *= scaleRatio;
            scaleVector.z *= scaleRatio;
        }
        currentSelection.size = scaleVector;



        currentIteration = (currentIteration + 1) % parameters.nrIterations;
    }
}
