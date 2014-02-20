using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class TerrainEditorGUI : EditorWindow
{
    private IterativeTerrainGenerator _generator;
    private DiamondSquareParameters _parameters = new DiamondSquareParameters();

    private TerrainData _selection;
    private TerrainData _Selection
    {
        get
        {
            return _selection;
        }
        set
        {
            if (_selection == value)
            {
                return;
            }

            _selection = value;

            if (_selection)
            {
                _generator = new IterativeTerrainGenerator(_selection, new DiamondSquareAlgorithm(_parameters));
            }
        }
    }

    [MenuItem("Window/TerrainGenerator")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(TerrainEditorGUI));
    }

    private void Update()
    {
        if (Terrain.activeTerrain != null)
        {
            _Selection = Terrain.activeTerrain.terrainData;
        }
        else
        {
            _Selection = null;
        }
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (!_Selection)
        {
            return;
        }

        EditorGUILayout.LabelField("Terrain parameters");
        _parameters.variation = EditorGUILayout.Slider("Variation", (float)Math.Round(_parameters.variation, 2), 0f, 1.0f);
        _parameters.smoothness = EditorGUILayout.Slider("Smoothness", (float)Math.Round(_parameters.smoothness, 2), 0.7f, 1.0f);
        _parameters.heightScaling = EditorGUILayout.Slider("Height scaling", (float)Math.Round(_parameters.heightScaling, 2), 0f, 1.0f);
        _parameters.outsideHeight = EditorGUILayout.Slider("Outside height", (float)Math.Round(_parameters.outsideHeight, 2), 0f, 1.0f);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Seeds");
        _parameters.seeds[0] = EditorGUILayout.Slider("North-west", (float)Math.Round(_parameters.seeds[0], 2), 0f, 1.0f);
        _parameters.seeds[1] = EditorGUILayout.Slider("North-east", (float)Math.Round(_parameters.seeds[1], 2), 0f, 1.0f);
        _parameters.seeds[2] = EditorGUILayout.Slider("South-west", (float)Math.Round(_parameters.seeds[2], 2), 0f, 1.0f);
        _parameters.seeds[3] = EditorGUILayout.Slider("South-east", (float)Math.Round(_parameters.seeds[3], 2), 0f, 1.0f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions");
        _parameters.nrIterations = EditorGUILayout.IntSlider("Number of iterations", _parameters.nrIterations, 1, 12);

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.LookLikeControls(60, 20);
        if (GUILayout.Button("Generate"))
        {
            _generator.Generate();
        }

        if (GUILayout.Button("Iterate"))
        {
            _generator.Iterate();
        }
        EditorGUILayout.LabelField("Iteration: " + _generator.CurrentIteration);
        EditorGUILayout.EndHorizontal();

        this.Repaint();
    }
}
