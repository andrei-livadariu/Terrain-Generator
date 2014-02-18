using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class DiamondSquareTerrain : EditorWindow {
	public float variation = 0.5f;
	public float smoothness = 0.9f;
	public float outsideHeight = 0.0f;
	public float heightScaling = 0.8f;
	public float[] seeds = { 0.0f, 0.0f, 0.0f, 0.0f };
	public int nrIterations = 9;

	private int currentIteration = 0;
	private DiamondSquare generator;
	
	[MenuItem ("Window/TerrainGenerator")]
    private static void Init () {
        EditorWindow.GetWindow (typeof (DiamondSquareTerrain));
    }
	
	TerrainData currentSelection;

	void OnInspectorUpdate() {
        Repaint();
    }
	
	void OnGUI() {
		
		//Rect nw = GUILayoutUtility.GetLastRect();
		//Rect areaRect = new Rect(nw.x,nw.yMax +30,nw.width,nw.width);
		
		variation = EditorGUILayout.Slider( "Variation", (float) Math.Round( variation, 2), 0f, 1.0f );
		smoothness = EditorGUILayout.Slider( "Smoothness", (float) Math.Round( smoothness, 2), 0.7f, 1.0f );
		heightScaling = EditorGUILayout.Slider( "Height scaling", (float) Math.Round( heightScaling, 2), 0f, 1.0f );
		outsideHeight = EditorGUILayout.Slider( "Outside height", (float) Math.Round( outsideHeight, 2), 0f, 1.0f );
		
		EditorGUILayout.LabelField( "Seeds" );
		seeds[0] = EditorGUILayout.Slider( "North-west", (float) Math.Round( seeds[0], 2), 0f, 1.0f );
		seeds[1] = EditorGUILayout.Slider( "North-east", (float) Math.Round( seeds[1], 2), 0f, 1.0f );
		seeds[2] = EditorGUILayout.Slider( "South-west", (float) Math.Round( seeds[2], 2), 0f, 1.0f );
		seeds[3] = EditorGUILayout.Slider( "South-east", (float) Math.Round( seeds[3], 2), 0f, 1.0f );
		
		nrIterations = EditorGUILayout.IntSlider( "Number of iterations", nrIterations, 1, 12 );
		
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.LookLikeControls( 60, 20 );
		if (GUILayout.Button( "Generate")) 
			Generate();

		if (GUILayout.Button( "Iterate")) 
			Iterate();
		EditorGUILayout.LabelField( "Iteration", currentIteration.ToString() );
		EditorGUILayout.EndHorizontal();
		
		this.Repaint();
	}

	void Update() {
		if( Terrain.activeTerrain != null ) {
			currentSelection = Terrain.activeTerrain.terrainData;
		}
	}
	
	void Generate() {
		if( currentSelection== null ) 
			return;
		
		generator = new DiamondSquare( nrIterations, seeds, variation, smoothness, outsideHeight, heightScaling );
		currentSelection.heightmapResolution = generator.GetResolution();
		generator.Generate();
		currentSelection.SetHeights( 0, 0, generator.GetHeights() );
		float resolutionScale = ( currentSelection.heightmapResolution - 1 ) / 32.0f;
		currentSelection.size = new Vector3( 125.0f * ( resolutionScale ), 600.0f, 125.0f * ( resolutionScale ) );
		currentIteration = 0;
	}
	
	void Iterate() {
		if( currentIteration == 0 ) {
			generator = new DiamondSquare( nrIterations, seeds, variation, smoothness, outsideHeight, heightScaling );
		}
		generator.Iterate();
		currentSelection.heightmapResolution = generator.GetIntermediateResolution();
		currentSelection.SetHeights( 0, 0, generator.GetIntermediateHeights() );
		
		int iResolution = generator.GetIntermediateResolution() - 1;
		float resolutionScale = ( generator.GetResolution() - 1 ) / 32.0f;
		Vector3 scaleVector = new Vector3( 125.0f * ( resolutionScale ), 600.0f, 125.0f * ( resolutionScale ) );
		if( iResolution < 33 ) {
			float scaleRatio = 32.0f / iResolution;
			scaleVector.x *= scaleRatio;
			scaleVector.z *= scaleRatio;
		}
		currentSelection.size = scaleVector;
		
	
		
		currentIteration = ( currentIteration + 1 ) % nrIterations;
	}
}

public class Point {
	public int x;
	public int y;
	public float height;
	
	public Point( Point p ) {
		x = p.x;
		y = p.y;
		height = p.height;
	}
	
	public Point( int x, int y, float height ) {
		this.x = x;
		this.y = y;
		this.height = height;
	}
	
	public bool isInside( int res ) {
		return x >= 0 && x < res && y >=0 && y < res;
	}
}

public class DiamondSquare {
	private System.Random _rng = new System.Random();
	private int _res;
	private float[,] _heights;
	private int _iterationStep;
	
	private float[] _seeds;
	private double _variation;
	private double _H;
	private float _outsideHeight;
	private double _heightScaling;
	
	public DiamondSquare( int iterations, float[] seeds, double variation, double roughness, float outsideHeight, double heightScaling ) {		
		_res = (int)Math.Pow( 2, iterations ) + 1;
		_heights = new float[ _res , _res ];
		
		_seeds = seeds;
		_variation = variation;
		_H = roughness;
		_outsideHeight = outsideHeight;
		_heightScaling = heightScaling;
		
		_iterationStep = _res - 1;

		InitializeHeights();
	}
	
	public void InitializeHeights() {
		int i, j;
		for( i = 0; i < _res; ++i ) {
			for( j = 0; j < _res; ++j ) {
				_heights[ i, j ] = 0f;
			}
		}
		// Initializing the corners
		_heights[0 , 0] = _seeds[0];
		_heights[0 , _res-1] = _seeds[1];
		_heights[_res-1 , 0] = _seeds[2];
		_heights[_res-1 , _res-1] = _seeds[3];
	}
	
	public void Generate()
	{		
		while( _iterationStep > 1 ) {
			Iterate();
		}
	}
	
	public int GetResolution() {
		return _res;
	}
	
	public int GetIntermediateResolution() {
		return ( ( _res - 1 ) / _iterationStep ) + 1;
	}
	
	public float[,] GetHeights() {
		float[,] heights = new float[ _res, _res ];
		int i, j;
		for( i = 0; i < _res; ++i ) {
			for( j = 0; j < _res; ++j ) {
				heights[i,j] = _heights[i,j] * (float)_heightScaling;
			}
		}
		return heights;
	}
	
	public float [,] GetIntermediateHeights() {
		int iRes = GetIntermediateResolution();
		int iStep = _iterationStep;
		float [,] heights = new float[ iRes, iRes ];
		int i, j;
		/*Debug.Log( "iRes: " + iRes );
		Debug.Log( "_res: " + _res );
		Debug.Log( "iStep: " + iStep );
		Debug.Log( "_iterationStep: " + _iterationStep );*/
		for( i = 0; i < iRes; ++i ) {
			for( j = 0; j < iRes; ++j ) {
				heights[i,j] = _heights[i * iStep, j * iStep] * (float)_heightScaling;
			}
		}
		return heights;
	}
	
	private static float Average( float x1, float x2, float x3, float x4 ) {
		return ( x1 + x2 + x3 + x4 ) / 4;
	}
	
	public void Iterate() {
		int i, j;
		for( i = 0; i < _res - 1; i += _iterationStep ) {
			for( j = 0; j < _res - 1; j += _iterationStep ) {
				Process(
					new Point( i, j, _heights[i , j] ),
					new Point( i + _iterationStep, j, _heights[i + _iterationStep , j] ),
					new Point( i, j + _iterationStep, _heights[i , j + _iterationStep] ),
					new Point( i + _iterationStep, j + _iterationStep, _heights[i , j + _iterationStep] )
				);
			}
		}
		_variation *= Math.Pow( 2, -_H );
		_iterationStep /= 2;
	}
	
	private void Process( Point p1, Point p2, Point p3, Point p4  ) {
		// Diamond step
		Point dMid = DiamondStep( p1, p2, p3, p4 );
		
		// Square step
		SquareStep( null, p1, p2, dMid );
		SquareStep( p1, null, dMid, p3 );
		SquareStep( p2, dMid, null, p4 );
		SquareStep( dMid, p3, p4, null );
	}
	
	private Point DiamondStep( Point p1, Point p2, Point p3, Point p4 ) {
		Point dMid = new Point( (int)( ( p1.x + p2.x ) / 2 ), (int)( ( p1.y + p3.y ) / 2 ), Average( p1.height, p2.height, p3.height, p4.height ) );
		_heights[ dMid.x , dMid.y ] = dMid.height = (float)( dMid.height + ( _rng.NextDouble() * 2 - 1.0f ) * _variation);
		return dMid;
	}
	
	private void SquareStep( Point p1, Point p2, Point p3, Point p4 ) {
		Point nullPoint;
		if( p1 == null ) {
			nullPoint = p1 = new Point( p4.x, (int)( p4.y - ( p4.y - p2.y ) * 2 ), 0.0f );
		} else if( p2 == null ) {
			nullPoint = p2 = new Point( (int)( p3.x - ( p3.x - p1.x ) * 2 ), p3.y, 0.0f );
		} else if( p3 == null ) {
			nullPoint = p3 = new Point( (int)( p2.x + ( p1.x - p2.x ) * 2 ), p2.y, 0.0f );
		} else {
			nullPoint = p4 = new Point( p1.x, (int)( p1.y + ( p2.y - p1.y ) * 2 ), 0.0f );
		}
		if( nullPoint.isInside( _res ) ) {
			nullPoint.height = _heights[ nullPoint.x, nullPoint.y ];
		} else {
			nullPoint.height = _outsideHeight;
		}
		Point sqMid = new Point( (int)( ( p2.x + p3.x ) / 2 ), (int)( ( p1.y + p4.y ) / 2 ), Average( p1.height, p2.height, p3.height, p4.height ) );
		_heights[ sqMid.x , sqMid.y ] = sqMid.height = (float)( sqMid.height + ( _rng.NextDouble() * 2 - 1.0f ) * _variation );
	}
}