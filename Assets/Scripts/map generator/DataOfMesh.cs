using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataOfMesh{

    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector2[] _uvs;

    private int _countOfIndexOfTriangles;

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="size"></param>
    public DataOfMesh(int size)
    {
        _vertices = new Vector3[size * size];
        _uvs = new Vector2[size * size];
        _triangles = new int[(size - 1) * (size - 1) * 6];
    }

    /// <summary>
    /// Añade el triangulo formado por a, b y c
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void AddTriangleToMesh(int a, int b, int c)
    {
        _triangles[_countOfIndexOfTriangles] = a;
        _triangles[_countOfIndexOfTriangles + 1] = b;
        _triangles[_countOfIndexOfTriangles + 2] = c;
        _countOfIndexOfTriangles += 3;
    }
    
    /// <summary>
    /// crea una malla
    /// </summary>
    /// <returns></returns>
    public Mesh MakeMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.uv = _uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    //getters y setters
    public Vector3[] Vertices
    {
        get
        {
            return _vertices;
        }

        set
        {
            _vertices = value;
        }
    }

    public int[] Triangles
    {
        get
        {
            return _triangles;
        }

        set
        {
            _triangles = value;
        }
    }

    public Vector2[] Uvs
    {
        get
        {
            return _uvs;
        }

        set
        {
            _uvs = value;
        }
    }
}
