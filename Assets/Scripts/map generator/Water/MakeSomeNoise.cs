using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSomeNoise : MonoBehaviour
{

    public float power = 3;
    public float scale = 1;
    public float timeScale = 1;

    private float offsetX;
    private float offsetY;
    private MeshFilter mf;

    // Use this for initialization
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        MakeNoise();
    }

    void Update()
    {
        MakeNoise();

        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }

    void MakeNoise()
    {
        Vector3[] vertices = mf.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * power;
        }

        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateNormals();

    }

    private float CalculateHeight(float x, float y)
    {
        float xCoord = x * scale + offsetX;
        float yCoord = y * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);

    }
}