using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapAndColorArray
{
    private float[,] _heightMap;
    private Color[] _colorArray;

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="height"></param>
    /// <param name="color"></param>
    public HeightMapAndColorArray(float[,] height, Color[] color)
    {
        _heightMap = height;
        _colorArray = color;
    }

    //getters
    public float[,] HeightMap
    {
        get
        {
            return _heightMap;
        }
    }

    public Color[] ColorArray
    {
        get
        {
            return _colorArray;
        }
    }
}