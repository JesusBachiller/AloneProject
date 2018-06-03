using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshWithLOD : MonoBehaviour
{
    private int _LOD;
    private Mesh _dataOfMesh;

    private bool _dataOfMeshHasBeenSolicit;
    private bool _thereIsDataOfMeshStored;

    private System.Action _method;

    private ProceduralGenerator _generator;

    /// <summary>
    /// contructor
    /// 
    /// </summary>
    /// <param name="lod"></param>
    /// <param name="method"></param>
    public MeshWithLOD(int lod, System.Action method)
    {
        _LOD = lod;
        _method = method;
    }

    /// <summary>
    /// se llama cuando se recibe los datos de la malla
    /// </summary>
    /// <param name="dataOfMesh"></param>
    public void DataOfMeshHasBeenReceived(DataOfMesh dataOfMesh)
    {
        _dataOfMesh = dataOfMesh.MakeMesh();
        _thereIsDataOfMeshStored = true;

        _method();
    }

    /// <summary>
    /// Solicita los datos de la malla en un hilo externo
    /// </summary>
    /// <param name="heightAndColoMap"></param>
    public void SolicitDataOfMeshInExternalThread(HeightMapAndColorArray heightAndColoMap)
    {
        _dataOfMeshHasBeenSolicit = true;
        if (_generator == null) { _generator = FindObjectOfType<ProceduralGenerator>(); }
        _generator.SolicitMesh(heightAndColoMap, _LOD, DataOfMeshHasBeenReceived);
    }

    //getters y setters
    public Mesh DataOfMesh
    {
        get
        {
            return _dataOfMesh;
        }

        set
        {
            _dataOfMesh = value;
        }
    }

    public bool DataOfMeshHasBeenSolicit
    {
        get
        {
            return _dataOfMeshHasBeenSolicit;
        }

        set
        {
            _dataOfMeshHasBeenSolicit = value;
        }
    }

    public bool ThereIsDataOfMeshStored
    {
        get
        {
            return _thereIsDataOfMeshStored;
        }

        set
        {
            _thereIsDataOfMeshStored = value;
        }
    }
}