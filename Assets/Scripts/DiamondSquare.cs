using UnityEngine;
using System.Collections;

public class Point
{
    public float x;
    public float y;
    public float z;

    public Point(Point p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
    }

    public Point(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool isInside(int res)
    {
        return x >= 0 && x < res && y >= 0 && y < res;
    }
}

public class DiamondSquare
{
    private int _res;
    private float[,] _heights;
    private int _iterationStep;

    private float[] _seeds;
    private float _variation;
    private float _H;
    private float _outsideHeight;
    private float _heightScaling;

    public int Resolution
    {
        get { return _res; }
    }

    public int IntermediateResolution
    {
        get { return ((_res - 1) / _iterationStep) + 1; }
    }

    public DiamondSquare(DiamondSquareParameters parameters)
    {
        _res = (int)Mathf.Pow(2, parameters.nrIterations) + 1;
        _heights = new float[_res, _res];

        _seeds = parameters.seeds;
        _variation = parameters.variation;
        _H = parameters.smoothness;
        _outsideHeight = parameters.outsideHeight;
        _heightScaling = parameters.heightScaling;

        _iterationStep = _res - 1;

        InitializeHeights();
    }

    public void InitializeHeights()
    {
        int i, j;
        for (i = 0; i < _res; ++i)
        {
            for (j = 0; j < _res; ++j)
            {
                _heights[i, j] = 0f;
            }
        }
        // Initializing the corners
        _heights[0, 0] = _seeds[0];
        _heights[0, _res - 1] = _seeds[1];
        _heights[_res - 1, 0] = _seeds[2];
        _heights[_res - 1, _res - 1] = _seeds[3];
    }

    public void Generate()
    {
        while (_iterationStep > 1)
        {
            Iterate();
        }
    }

    public float[,] GetHeights()
    {
        float[,] heights = new float[_res, _res];
        int i, j;
        for (i = 0; i < _res; ++i)
        {
            for (j = 0; j < _res; ++j)
            {
                heights[i, j] = _heights[i, j] * (float)_heightScaling;
            }
        }
        return heights;
    }

    public float[,] GetIntermediateHeights()
    {
        int iRes = IntermediateResolution;
        int iStep = _iterationStep;
        float[,] heights = new float[iRes, iRes];
        int i, j;
        /*Debug.Log( "iRes: " + iRes );
        Debug.Log( "_res: " + _res );
        Debug.Log( "iStep: " + iStep );
        Debug.Log( "_iterationStep: " + _iterationStep );*/
        for (i = 0; i < iRes; ++i)
        {
            for (j = 0; j < iRes; ++j)
            {
                heights[i, j] = _heights[i * iStep, j * iStep] * (float)_heightScaling;
            }
        }
        return heights;
    }

    public void Iterate()
    {
        int i, j;
        for (i = 0; i < _res - 1; i += _iterationStep)
        {
            for (j = 0; j < _res - 1; j += _iterationStep)
            {
                Process(
                    new Point(i, j, _heights[i, j]),
                    new Point(i + _iterationStep, j, _heights[i + _iterationStep, j]),
                    new Point(i, j + _iterationStep, _heights[i, j + _iterationStep]),
                    new Point(i + _iterationStep, j + _iterationStep, _heights[i, j + _iterationStep])
                );
            }
        }
        _variation *= Mathf.Pow(2, -_H);
        _iterationStep /= 2;
    }

    private void Process(Point p1, Point p2, Point p3, Point p4)
    {
        // Diamond step
        Point dMid = DiamondStep(p1, p2, p3, p4);

        // Square step
        SquareStep(null, p1, p2, dMid, 1);
        SquareStep(p1, null, dMid, p3, 2);
        SquareStep(p2, dMid, null, p4, 3);
        SquareStep(dMid, p3, p4, null, 4);
    }

    private Point DiamondStep(Point p1, Point p2, Point p3, Point p4)
    {
        Point dMid = new Point((int)((p1.x + p2.x) / 2), (int)((p1.y + p3.y) / 2), _average(p1.z, p2.z, p3.z, p4.z));
        _heights[(int)dMid.x, (int)dMid.y] = dMid.z += Random.Range(-_variation, _variation);
        return dMid;
    }

    private void SquareStep(Point p1, Point p2, Point p3, Point p4, int nullIndex)
    {
        Point nullPoint;
        switch (nullIndex)
        {
            case 1:
                nullPoint = p1 = new Point(p4.x, (int)(p4.y - (p4.y - p2.y) * 2), 0.0f);
                break;
            case 2:
                nullPoint = p2 = new Point((int)(p3.x - (p3.x - p1.x) * 2), p3.y, 0.0f);
                break;
            case 3:
                nullPoint = p3 = new Point((int)(p2.x + (p1.x - p2.x) * 2), p2.y, 0.0f);
                break;
            case 4:
            default:
                nullPoint = p4 = new Point(p1.x, (int)(p1.y + (p2.y - p1.y) * 2), 0.0f);
                break;
        }

        if (_isPointInside(nullPoint, _res))
        {
            nullPoint.z = _heights[(int)nullPoint.x, (int)nullPoint.y];
        }
        else
        {
            nullPoint.z = _outsideHeight;
        }
        Point sqMid = new Point((int)((p2.x + p3.x) / 2), (int)((p1.y + p4.y) / 2), _average(p1.z, p2.z, p3.z, p4.z));
        _heights[(int)sqMid.x, (int)sqMid.y] = sqMid.z += Random.Range(-_variation, _variation);
    }

    private static bool _isPointInside(Point point, float res)
    {
        return point.x >= 0 && point.x < res && point.y >= 0 && point.y < res;
    }

    private static float _average(float x1, float x2, float x3, float x4)
    {
        return (x1 + x2 + x3 + x4) / 4;
    }
}
