using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private ProceduralGenerator _generator;
    
    private bool _chunkHasResource;

    private HeightMapAndColorArray _heightMapAndColorArray;

    //se usará para poder solicitar la malla una vez que se tenga el mapa de altua y de color
    private bool _heightMapAndColorArrayHaveStored;
    
    //posicion real del chunk
    private Vector2 _position;
    //posicion respecto a los demas chunks
    private Vector2 _coordinates;

    private Bounds _bounds;

    //objeto en unity y sus respectivos componentes
    private GameObject _gameObject;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    //malla que su usará como collider
    private MeshWithLOD _meshForCollider;
    //info y mallas de los diferentes niveles de detalle
    private InfoLOD[] _infoLOD_Array;
    private MeshWithLOD[] _meshLOD_Array;

    private int _indexPreviousLOD = -1;
    
    /// <summary>
    /// contructor de la clase Chunk
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="size"></param>
    /// <param name="infoLOD_Array"></param>
    /// <param name="transformOfParent"></param>
    /// <param name="material"></param>
    /// <param name="generator"></param>
    public Chunk(Vector2 coord, int size, InfoLOD[] infoLOD_Array, Transform transformOfParent, Material material, ProceduralGenerator generator)
    {
        _position = coord * size;
        _coordinates = coord;


        _infoLOD_Array = infoLOD_Array;
        _generator = generator;
        _chunkHasResource = false;

        //creación del objeto de Unity
        Vector3 posisiton3D = new Vector3(_position.x, 0, _position.y);
        _gameObject = new GameObject("Chunk");
        _meshCollider = _gameObject.AddComponent<MeshCollider>();
        _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _gameObject.AddComponent<MeshFilter>();

        _gameObject.transform.position = posisiton3D * _generator.ScaleChunkSize;
        _gameObject.transform.parent = transformOfParent;
        _gameObject.transform.localScale = new Vector3(1, 1, 1) * _generator.ScaleChunkSize;

        //no se quiere que se muestre desde el principio. más a delante se establece se se muestra o no.
        _gameObject.SetActive(false);


        _bounds = new Bounds(_position, new Vector2(1,1) * size);
        

        _meshLOD_Array = new MeshWithLOD[infoLOD_Array.Length];
        for (int i = 0; i < infoLOD_Array.Length; i++)
        {
            _meshLOD_Array[i] = new MeshWithLOD(infoLOD_Array[i].LOD, UpdateThisChunk);
            if (infoLOD_Array[i].UseForCollide)
            {
                _meshForCollider = _meshLOD_Array[i];
            }
        }

        //Los dos primeros parametros son necesario spara el calculo de los mapas
        //El ultimo es la función que se va a ejecutar cuando se desencole dentro del hilo principal de Unity
        _generator.SolicitHeightMapAndColorArray(_coordinates, _position, applyTheHeightMapAndColorArrayToTheObject);
    }

    /// <summary>
    /// Cuando se reciba la información del mapa de color y de altura se ejecutará este metodo
    /// </summary>
    /// <param name="heightMapAndColorArray"></param>
    private void applyTheHeightMapAndColorArrayToTheObject(HeightMapAndColorArray heightMapAndColorArray)
    {
        _heightMapAndColorArray = heightMapAndColorArray;
        _heightMapAndColorArrayHaveStored = true;
        
        Texture2D texture = new Texture2D(_generator.SizeOfChunk, _generator.SizeOfChunk);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(heightMapAndColorArray.ColorArray);
        texture.Apply();

        _meshRenderer.material.mainTexture = texture;
       
        UpdateThisChunk();
    }

    /// <summary>
    /// Cuando se reciba la información de la malla ya calculada en un hilo externo se ejecutará este metodo
    /// </summary>
    /// <param name="dataOfMesh"></param>
    private void applyTheMeshToTheObject(DataOfMesh dataOfMesh)
    {
        _meshFilter.mesh = dataOfMesh.MakeMesh();
    }

    /// <summary>
    /// actualiza el chunk: malla renderizada y malla del colisionador
    /// </summary>
    public void UpdateThisChunk()
    {
        if (_heightMapAndColorArrayHaveStored)
        {
            // distancia entre el jugador y el borde más cercano (al jugador) <= maxima distancia establecida como visible --- > visible = true
            bool visible = Mathf.Sqrt(_bounds.SqrDistance(_generator.ActualPositionOfPlayer)) <= _generator.MaxDistanceVisibleOfPlayer;

            if (visible)
            {

                //Busqueda el LOD adecuado
                int actualLOD = locateActualLOD();

                //Si el LOD actual es de mayor calidad 
                if (actualLOD == 0)
                {
                    UpdateColliderOfGameObject();
                }

                //Si el LOD actual cambiar resepcto al lod que se esta mostrando actualmente
                if (actualLOD != _indexPreviousLOD)
                {
                    UpdateMeshOfGameObject(actualLOD);
                }

                _generator.AddChunkToListOfChunkVisiblesAfterLastUpdate(this);

            }

            _gameObject.SetActive(visible);

        }
    }

    /// <summary>
    /// Acutaliza la malla del game object de este chunk en función del LOD inficado como parametro
    /// </summary>
    /// <param name="lod"></param>
    private void UpdateMeshOfGameObject(int lod)
    {
        //obtengo la malla de ese LOD
        MeshWithLOD lodMesh = _meshLOD_Array[lod];

        if (lodMesh.ThereIsDataOfMeshStored)//Si ya tiene la malla almacenada se actualiza en el objeto de unity
        {
            _indexPreviousLOD = lod;
            _meshFilter.mesh = lodMesh.DataOfMesh;
        }
        else if (!lodMesh.DataOfMeshHasBeenSolicit) //Si no tiene la malla almacenada se solitita la malla (Y tampoco se ha solicitado anteriormente) 
        {
            lodMesh.SolicitDataOfMeshInExternalThread(_heightMapAndColorArray);
        }
    }

    /// <summary>
    /// Acutaliza la malla del collider del game object de este chunk
    /// </summary>
    private void UpdateColliderOfGameObject()
    {
        if (_meshForCollider.ThereIsDataOfMeshStored) //Si ya tiene la malla almacenada se usa esta malla como colisionador
        {
            _meshCollider.sharedMesh = _meshForCollider.DataOfMesh;

            //Si es la primera vez que se ha usado como colisionador (es decir que es la primera vez que esta a mayor resolución el chunk) ---> se añaden recursos aletatorios
            if (!_chunkHasResource)
            {
                _chunkHasResource = true;
                _generator.AñadirRecursos(_gameObject.transform.position, _heightMapAndColorArray);
            }

        }
        else if (!_meshForCollider.DataOfMeshHasBeenSolicit) //Si no tiene la malla almacenada se solitita la malla (Y tampoco se ha solicitado anteriormente) 
        {
            _meshForCollider.SolicitDataOfMeshInExternalThread(_heightMapAndColorArray);
        }
    }

    /// <summary>
    /// busqueda del LOd actual
    /// </summary>
    /// <returns></returns>
    private int locateActualLOD()
    {
        int LOD = 0;

        for (int i = 0; i < _infoLOD_Array.Length - 1; i++)
        {
            //Si --> distancia entre el jugador y el chunk > Limite donde están los chunk con un LOD determinado
            if (Mathf.Sqrt(_bounds.SqrDistance(_generator.ActualPositionOfPlayer)) > _infoLOD_Array[i].LimitDistanceOfLOD)
            {
                LOD = i + 1;
            }
            else
            {
                break;
            }
            
        }

        return LOD;
    }

    //getters y setters
    public bool Visibility
    {
        get
        {
            return _gameObject.activeSelf;
        }
        set
        {
            bool visible = value;
            if(_gameObject != null)
            {
                _gameObject.SetActive(visible);
            }
        }
    }

}
