using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour {
    public GameObject Tree;
    public GameObject CoalMine;
    public GameObject IronMine;
    public GameObject Rock;
    public GameObject Herb;
    public GameObject Stone;

    public GameObject TreeTrunk;
    public GameObject Seed;
    public GameObject Iron;
    public GameObject Coal;
    public GameObject Rope;

    public Transform ParentOfResources;
    
    private List<DataOfResources> _listOfResources = new List<DataOfResources>();

    private int _heightOfRaycast = 300;
    
    private const int _sizeOfChunk = 97;
    private const int _numberOfChunkInTerrain = 30;

    private float _persistance = 0.5f;
    private float _lacunarity = 2;
    private float _scale = 200;
    private int _seed;
    private int _octaves = 8;
    private float _multiplier = 90;
    public AnimationCurve curva;

    public Biomes[] biomes;

    private const float _scaleChunkSize = 4f;

    private float[,] _falloffMap;

    private Queue<ThreadInfo<HeightMapAndColorArray>> _queueHeightMapAndColorArray = new Queue<ThreadInfo<HeightMapAndColorArray>>();
    private Queue<ThreadInfo<DataOfMesh>> _queueDataOfMesh = new Queue<ThreadInfo<DataOfMesh>>();

    public InfoLOD[] LevelOfDetail;
    private static float _maxDistanceVisibleOfPlayer;

    private bool _playerPositionIsStablished = false;

    public Transform Player;
    public Material Material;
    public Transform TransformOfParent;

    private static Vector2 _actualPositionOfPlayer;
    private Vector2 _previousPositionÓfPlayer;
    private int _chunksVisibles;

    private Dictionary<Vector2, Chunk> _dicChunks = new Dictionary<Vector2, Chunk>();
    private static List<Chunk> _chunksVisiblesAfterLastUpdate = new List<Chunk>();
    
    private void Start()
    {
        _seed = UnityEngine.Random.Range(0, 1000);
        _maxDistanceVisibleOfPlayer = LevelOfDetail[LevelOfDetail.Length - 1].LimitDistanceOfLOD;
        _chunksVisibles = Mathf.RoundToInt(_maxDistanceVisibleOfPlayer / (_sizeOfChunk - 1));

        updateAllChunkVisibles();
    }

    private void Update()
    {
        _actualPositionOfPlayer = new Vector2(Player.position.x, Player.position.z) / _scaleChunkSize;

        //Si el jugador se ha movido más de 650 desde el ultimo ActualizaChunksVisibles() ---> se vuelve a actualizar
        if ((_previousPositionÓfPlayer - _actualPositionOfPlayer).sqrMagnitude > 650f)
        {
            _previousPositionÓfPlayer = _actualPositionOfPlayer;
            updateAllChunkVisibles();
        }

        int index;
        //Ya dentro del hilo principal de unity se evalua las dos colas que se actualizan en hilos diferentes.
        if (_queueHeightMapAndColorArray.Count > 0)
        {
            index = 0;
            while (index < _queueHeightMapAndColorArray.Count)
            {
                //Obtengo el primer objeto que esta en la lista. Al mismo tiempo lo quito de la cola
                ThreadInfo<HeightMapAndColorArray> threadInfoHeightMapAndColorArray = _queueHeightMapAndColorArray.Dequeue();
                //Ejecuto el metodo guardado en el estruc ThreadInfoMapa con el parametro que también esta guardado en ese mismo struct
                threadInfoHeightMapAndColorArray.Method(threadInfoHeightMapAndColorArray.ParameterOfMethod);

                index++;
            }
        }

        if (_queueDataOfMesh.Count > 0)
        {
            index = 0;
            while (index < _queueDataOfMesh.Count)
            {
                //Obtengo el primer objeto que esta en la lista. Al mismo tiempo lo quito de la cola
                ThreadInfo<DataOfMesh> threadInfoDataOfMesh = _queueDataOfMesh.Dequeue();
                //Ejecuto el metodo guardado en el estruc ThreadInfoMapa con el parametro que también esta guardado en ese mismo struct
                threadInfoDataOfMesh.Method(threadInfoDataOfMesh.ParameterOfMethod);

                index++;
            }
        }

        if (!_playerPositionIsStablished)
        {
            _playerPositionIsStablished = Player.GetComponent<Player>().InitPosition();
        }
    }

    /// <summary>
    /// Metodo para añadir recursos al mapa
    /// </summary>
    public void AñadirRecursos(Vector3 position, HeightMapAndColorArray heightMapAndColoArray)
    {
        for (int i = 0; i < 25; i++)
        {
            RaycastHit raycastHit;

            //situa en un plano por encima del mapa objetos py hace ray cast para conocer la altura de ese punto.
            if (Physics.Raycast(position + Vector3.up * _heightOfRaycast + Vector3.right * UnityEngine.Random.Range(-180f, 180f) + Vector3.forward * UnityEngine.Random.Range(-180f, 180f), -Vector3.up, out raycastHit))
            {
                if (raycastHit.transform.name == "Chunk")
                {
                    //altura del mapa de ruido, NO altura global de la escena.
                    float heightNoiseMap = heightMapAndColoArray.HeightMap[Mathf.RoundToInt(raycastHit.textureCoord.x * heightMapAndColoArray.HeightMap.GetLength(1)), Mathf.RoundToInt(raycastHit.textureCoord.y * heightMapAndColoArray.HeightMap.GetLength(1))];
                    int index = 0;
                    //obtenemos el bioma donde esta situado ese punto
                    while (index < biomes.Length)
                    {
                        if (heightNoiseMap >= biomes[index].Height)
                        {
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    DataOfResources dataOfResource = new DataOfResources(biomes[index - 1].Name, raycastHit.point);

                    switch (dataOfResource.MyBiome)
                    {
                        case "Agua":
                            break;
                        case "Arena":
                            addResourceInBeach(dataOfResource);
                            break;
                        case "Pradera":
                            addResourceInMeadow(dataOfResource);
                            break;
                        case "Bosque":
                            addResourceInForest(dataOfResource);
                            break;
                        case "BajaM":
                            addResourceInLMountain(dataOfResource);
                            break;
                        case "MediaM":
                            addResourceInMMountain(dataOfResource);
                            break;
                        case "AltaM":
                            addResourceInHMountain(dataOfResource);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// añade un recurso a alta montaña
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInHMountain(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 40f)//crea Roca
        {
            inst = instanciateRock(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// añade un recurso a media montaña
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInMMountain(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 40f)//crea Roca
        {
            inst = instanciateRock(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else if (percent < 80f)//crea hierro
        {
            inst = instanciateIronMine(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// añade un recurso a baja montaña
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInLMountain(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 10f) //Crea arbol
        {
            inst = instanciateTree(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else if (percent < 60f) //coal
        {
            inst = instanciateCoalMine(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// añade un recurso a pradera
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInMeadow(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 90f) //Crea Hierva
        {
            inst = instanciateHerbs(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
            DataOfResources newResourceData;
            for (int j = 0; j < UnityEngine.Random.Range(15, 20); j++)
            {
                newResourceData = instanciateHerbArroundOfPosition(data.Position);
                if (newResourceData != null) { _listOfResources.Add(newResourceData); }
            }
        }
        else if (percent < 95f)
        {
            inst = instanciateTree(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else if (percent < 99f)
        {
            inst = instanciateCoalMine(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// añade un recurso al bosque
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInForest(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 70f) //Crea arbol
        {
            inst = instanciateTree(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
            DataOfResources newResourceData;
            for (int j = 0; j < UnityEngine.Random.Range(6, 10); j++)
            {
                newResourceData = instanciateTreeArroundOfPosition(data.Position);
                if (newResourceData != null) { _listOfResources.Add(newResourceData); }
            }
        }
        else if (percent < 101f) //hierba
        {
            inst = instanciateHerbs(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
            DataOfResources newResourceData;
            for (int j = 0; j < UnityEngine.Random.Range(7, 12); j++)
            {
                newResourceData = instanciateHerbArroundOfPosition(data.Position);
                if (newResourceData != null) { _listOfResources.Add(newResourceData); }
            }
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// añade un recurso a la playa
    /// </summary>
    /// <param name="data"></param>
    private void addResourceInBeach(DataOfResources data)
    {
        GameObject inst;
        float percent = UnityEngine.Random.Range(0f, 100f);

        if (percent < 20f) //Crea Mina de hierro
        {
            inst = instanciateIronMine(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
            DataOfResources newResourceData;
            for (int j = 0; j < UnityEngine.Random.Range(2, 5); j++)
            {
                newResourceData = instanciateStoneArroundOfPosition(data.Position);
                if (newResourceData != null) { _listOfResources.Add(newResourceData); }
            }
        }
        else if (percent < 35f)//crea Roca
        {
            inst = instanciateRock(data.Position);
            data.GameObject = inst;
            _listOfResources.Add(data);
        }
        else
        {
            return;
        }
    }
    
    /// <summary>
    /// intancia una hierva en una pasicion al rededor de pos
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private DataOfResources instanciateHerbArroundOfPosition(Vector3 pos)
    {
        GameObject inst;

        RaycastHit raycastHit;

        int ratio = 3;
        float xRandom = UnityEngine.Random.Range(-15f, 15f);
        float yRandom = UnityEngine.Random.Range(-15f, 15f);
        while (xRandom * xRandom + yRandom * yRandom < ratio * ratio)
        {
            xRandom = UnityEngine.Random.Range(-9f, 9f);
            yRandom = UnityEngine.Random.Range(-9f, 9f);
        }
        Vector3 arroundPosition = new Vector3(pos.x + xRandom, pos.y, pos.z + yRandom);
        //situa en un plano por encima del mapa objetos py hace ray cast para conocer la altura de ese punto.
        if (Physics.Raycast(arroundPosition + Vector3.up * _heightOfRaycast, -Vector3.up, out raycastHit))
        {
            if (raycastHit.transform.name == "Chunk")
            {
                inst = Instantiate(Herb, raycastHit.point, Quaternion.identity, ParentOfResources);
                inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
                inst.transform.localScale *= UnityEngine.Random.Range(0.75f, 1.25f);
                inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_HERB,
                                                                           ConstantAssistant.NUMBER_OF_EXTRACTIONS_HERB,
                                                                           Player.GetComponent<Player>(),
                                                                           new GameObject[] { Seed, Rope },
                                                                           new int[] { (int)ConstantAssistant.EnumNoNaturalResources.Seed, (int)ConstantAssistant.EnumNoNaturalResources.Rope });
                return new DataOfResources("Herb", raycastHit.point, inst);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// intancia un arbol en una pasicion al rededor de pos
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private DataOfResources instanciateTreeArroundOfPosition(Vector3 pos)
    {
        GameObject inst;

        RaycastHit hit;

        int ratio = 10;
        float xRandom = UnityEngine.Random.Range(-20f, 20f);
        float yRandom = UnityEngine.Random.Range(-20f, 20f);
        while (xRandom * xRandom + yRandom * yRandom < ratio * ratio)
        {
            xRandom = UnityEngine.Random.Range(-9f, 9f);
            yRandom = UnityEngine.Random.Range(-9f, 9f);
        }
        Vector3 posAlrededor = new Vector3(pos.x + xRandom, pos.y, pos.z + yRandom);
        //situa en un plano por encima del mapa objetos py hace ray cast para conocer la altura de ese punto.
        if (Physics.Raycast(posAlrededor + Vector3.up * _heightOfRaycast, -Vector3.up, out hit))
        {
            if (hit.transform.name == "Chunk")
            {
                inst = Instantiate(Tree, hit.point, Quaternion.identity, ParentOfResources);
                inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
                inst.transform.localScale *= UnityEngine.Random.Range(0.75f, 1.75f);
                inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_TREE,
                                                                           ConstantAssistant.NUMBER_OF_EXTRACTIONS_TREE,
                                                                           Player.GetComponent<Player>(),
                                                                           new GameObject[] { TreeTrunk, Seed },
                                                                           new int[] { (int)ConstantAssistant.EnumNoNaturalResources.TreeTrunk, (int)ConstantAssistant.EnumNoNaturalResources.Seed });
                return new DataOfResources("Tree", hit.point, inst);
            }
            else
            {
                return null;
            }

        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// intancia una piedra en una pasicion al rededor de pos
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private DataOfResources instanciateStoneArroundOfPosition(Vector3 pos)
    {
        GameObject inst;

        RaycastHit hit;

        int ratio = 9;
        float xRand = UnityEngine.Random.Range(-9f, 9f);
        float yRand = UnityEngine.Random.Range(-9f, 9f);
        while (xRand * xRand + yRand * yRand < ratio * ratio)
        {
            xRand = UnityEngine.Random.Range(-9f, 9f);
            yRand = UnityEngine.Random.Range(-9f, 9f);
        }
        Vector3 posAlrededor = new Vector3(pos.x + xRand, pos.y, pos.z + yRand);
        //situa en un plano por encima del mapa objetos py hace ray cast para conocer la altura de ese punto.
        if (Physics.Raycast(posAlrededor + Vector3.up * _heightOfRaycast, -Vector3.up, out hit))
        {
            if (hit.transform.name == "Chunk")
            {
                inst = Instantiate(Stone, hit.point, Quaternion.identity, ParentOfResources);
                inst.GetComponent<NoNaturalResource>().InitNoNaturalResource((int)ConstantAssistant.EnumNoNaturalResources.Stone, Player.GetComponent<Player>(), Stone, false, 0);
                inst.GetComponent<Rigidbody>().isKinematic = true;
                inst.transform.localScale *= UnityEngine.Random.Range(0.75f, 1.3f);
                return new DataOfResources("Stone", hit.point, inst);
            }
            else
            {
                return null;
            }

        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// intacia mina de carbon
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GameObject instanciateCoalMine(Vector3 pos)
    {
        GameObject inst;

        inst = Instantiate(CoalMine, pos, Quaternion.identity, ParentOfResources);
        inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_COAL_MINE,
                                                                   ConstantAssistant.NUMBER_OF_EXTRACTIONS_COAL_MINE,
                                                                   Player.GetComponent<Player>(),
                                                                   new GameObject[] { Coal },
                                                                   new int[] { (int)ConstantAssistant.EnumNoNaturalResources.Coal });

        return inst;
    }
    
    /// <summary>
    /// instancia Arbol
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GameObject instanciateTree(Vector3 pos)
    {
        GameObject inst;

        inst = Instantiate(Tree, pos, Quaternion.identity, ParentOfResources);
        inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_TREE,
                                                                   ConstantAssistant.NUMBER_OF_EXTRACTIONS_TREE,
                                                                   Player.GetComponent<Player>(),
                                                                   new GameObject[] { TreeTrunk, Seed },
                                                                   new int[] { (int)ConstantAssistant.EnumNoNaturalResources.TreeTrunk, (int)ConstantAssistant.EnumNoNaturalResources.Seed });

        return inst;
    }

    /// <summary>
    /// instancia hierbas
    /// </summary>
    /// <param name="posicion"></param>
    /// <returns></returns>
    private GameObject instanciateHerbs(Vector3 posicion)
    {
        GameObject inst;

        inst = Instantiate(Herb, posicion, Quaternion.identity, ParentOfResources);
        inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_HERB,
                                                                   ConstantAssistant.NUMBER_OF_EXTRACTIONS_HERB,
                                                                   Player.GetComponent<Player>(),
                                                                   new GameObject[] { Seed, Rope },
                                                                   new int[] { (int)ConstantAssistant.EnumNoNaturalResources.Seed, (int)ConstantAssistant.EnumNoNaturalResources.Rope });

        return inst;
    }

    /// <summary>
    /// instancia roca
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GameObject instanciateRock(Vector3 pos)
    {
        GameObject inst;

        inst = Instantiate(Rock, pos, Quaternion.identity, ParentOfResources);
        inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_ROCK,
                                                                   ConstantAssistant.NUMBER_OF_EXTRACTIONS_ROCK,
                                                                   Player.GetComponent<Player>(),
                                                                   new GameObject[] { Stone },
                                                                   new int[] { (int)ConstantAssistant.EnumNoNaturalResources.Stone });
        return inst;
    }

    /// <summary>
    /// instancia mina de herro
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GameObject instanciateIronMine(Vector3 pos)
    {
        GameObject inst;

        inst = Instantiate(IronMine, pos, Quaternion.identity, ParentOfResources);
        inst.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        inst.GetComponent<NaturalResources>().InitNaturalResources(ConstantAssistant.COLLECTION_TIME_IRON_MINE,
                                                                   ConstantAssistant.NUMBER_OF_EXTRACTIONS_IRON_MINE,
                                                                   Player.GetComponent<Player>(),
                                                                   new GameObject[] { Iron },
                                                                   new int[] { (int)ConstantAssistant.EnumNoNaturalResources.Iron });
        return inst;
    }

    /// <summary>
    /// Funcion que actualiza todos los chunk que se vana mostrar.
    /// </summary>
    private void updateAllChunkVisibles()
    {
        for (int i = 0; i < _chunksVisiblesAfterLastUpdate.Count; i++)
        {
            _chunksVisiblesAfterLastUpdate[i].Visibility = false;
        }
        _chunksVisiblesAfterLastUpdate.Clear();

        //obtener la coordenada del chunk donde está le jugador
        
        int coordX_ActualChunk = Mathf.RoundToInt(_actualPositionOfPlayer.x / (_sizeOfChunk - 1));
        int coordY_ActualChunk = Mathf.RoundToInt(_actualPositionOfPlayer.y / (_sizeOfChunk - 1));

        for (int i = -_chunksVisibles; i <= _chunksVisibles; i++)
        {
            for (int j = -_chunksVisibles; j <= _chunksVisibles; j++)
            {
                Vector2 coordChunk = new Vector2(coordX_ActualChunk + j, coordY_ActualChunk + i);

                updateChunk(coordChunk);
                
            }
        }
    }

    /// <summary>
    /// Actualiza el chunk de la posicion = coord
    /// </summary>
    /// <param name="coord"></param>
    private void updateChunk(Vector2 coord)
    {
        if (_dicChunks.ContainsKey(coord))
        {
            _dicChunks[coord].UpdateThisChunk();
        }
        else
        {
            _dicChunks.Add(coord, new Chunk(coord, _sizeOfChunk - 1, LevelOfDetail, TransformOfParent, Material, this));
        }
    }

    /// <summary>
    /// Añade el chunk a la lista de chunks visibles
    /// </summary>
    /// <param name="chunk"></param>
    public void AddChunkToListOfChunkVisiblesAfterLastUpdate(Chunk chunk)
    {
        _chunksVisiblesAfterLastUpdate.Add(chunk);
    }

    //Metodos que crean un hilo para generar los datos requeridos
    public void SolicitHeightMapAndColorArray(Vector2 coord, Vector2 center, Action<HeightMapAndColorArray> method)
    {
        //establece el metodo que se ejecutara cuando se inicie el hilo: MapaDeAlturasYColorThread()
        ThreadStart threadStart = delegate { generateHeightMapAndColorArrayInThread(coord, center, method); };
        //empieza el hilo
        new Thread(threadStart).Start();
    }
    public void SolicitMesh(HeightMapAndColorArray heightMapAndColorArray, int LOD, Action<DataOfMesh> method)
    {
        //establece el metodo que se ejecutara cuando se inicie el hilo: mallaThread()
        ThreadStart threadStart = delegate { generateMeshInThread(heightMapAndColorArray, LOD, method); };
        //inicio del hilo
        new Thread(threadStart).Start();
    }

    /*Metodos que se ejecutan en hilos secundarios*/
    /// <summary>
    /// crea el falloff map del tamaña deseado
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private float[,] makeFalloffMap(int size)
    {
        float[,] falloff = new float[size, size];

        float x, y, gradualness, distOfCenter;
        float result, signoide;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                x = i / (float)size * 2 - 1;
                y = j / (float)size * 2 - 1;

                result = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                gradualness = 2.2f; //0->muy gradual; 10->muy brusco
                distOfCenter = 2f;//1->cerca del centro; 10->lejos del centro

                //función signoide.
                signoide = Mathf.Pow(result, gradualness) / (Mathf.Pow(result, gradualness) + Mathf.Pow(distOfCenter - distOfCenter * result, gradualness));

                //limitamos el resultado a 2 decimales
                signoide = (float)Mathf.Round(signoide * 100f) / 100;

                falloff[i, j] = signoide;
            }
        }

        return falloff;
    }

    /// <summary>
    /// Crea un mapa interpolando 4 puntos
    /// </summary>
    /// <param name="size"></param>
    /// <param name="SD"></param>
    /// <param name="SI"></param>
    /// <param name="ID"></param>
    /// <param name="II"></param>
    /// <returns></returns>
    private float[,] bilinialInterpolation(int size, float SD, float SI, float ID, float II)
    {
        float[,] map = new float[size, size];

        float supInterpolation, infInterpoation, valueInterpolation;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //interpolación de la parte superior e inferior
                supInterpolation = lienalInterpolation(0f, SI, size, SD, i);
                infInterpoation = lienalInterpolation(0f, II, size, ID, i);

                //interpolación de los dos valores anteriores
                valueInterpolation = lienalInterpolation(0f, supInterpolation, size, infInterpoation, j);

                map[i, j] = valueInterpolation;


            }
        }

        return map;
    }

    /// <summary>
    /// interpolacion lineal
    /// </summary>
    /// <param name="x"></param>
    /// <param name="xValue"></param>
    /// <param name="y"></param>
    /// <param name="yValue"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private float lienalInterpolation(float x, float xValue, float y, float yValue, float t)
    {
        return xValue * (y - t) / (y - x) + yValue * (t - x) / (y - x);
    }

    /// <summary>
    /// aplica el mapa falloff al mapaRuido segun las coordenadas del chuck
    /// </summary>
    /// <param name="noiseMap"></param>
    /// <param name="coordinatesOfChunk"></param>
    /// <returns></returns>
    private float[,] applyFalloffMapToNoiseMap(float[,] noiseMap, Vector2 coordinatesOfChunk)
    {
        //Se obtiene el falloff map  del tamaño indicado (tamañoGlobalTerreno)
        _falloffMap = makeFalloffMap(_numberOfChunkInTerrain);

        float[,] interpolatedMap = new float[0, 0];

        //se obtiene cuatro valores contiguos del FalloffMap
        float falloffMapValue_SupLef    = _falloffMap[Mathf.RoundToInt(coordinatesOfChunk.x), Mathf.RoundToInt(coordinatesOfChunk.y) + 1];
        float falloffMapValue_SupRight  = _falloffMap[Mathf.RoundToInt(coordinatesOfChunk.x) + 1, Mathf.RoundToInt(coordinatesOfChunk.y) + 1];
        float falloffMapValue_InfLeft   = _falloffMap[Mathf.RoundToInt(coordinatesOfChunk.x), Mathf.RoundToInt(coordinatesOfChunk.y)];
        float falloffMapValue_InfRight  = _falloffMap[Mathf.RoundToInt(coordinatesOfChunk.x) + 1, Mathf.RoundToInt(coordinatesOfChunk.y)];

        //se interpolan los 4 valores anteriores.
        interpolatedMap = bilinialInterpolation(_sizeOfChunk, falloffMapValue_SupRight, falloffMapValue_SupLef, falloffMapValue_InfRight, falloffMapValue_InfLeft);

        for (int y = 0; y < _sizeOfChunk; y++)
        {
            for (int x = 0; x < _sizeOfChunk; x++)
            {
                //mapa interpolado con 2 decimales unicamente
                //mapaRuido[x, y] = (float)Mathf.Round(mapaInterpolado[x, y] * 100f) / 100f;
                noiseMap[x, y] = noiseMap[x, y] - (float)Mathf.Round(interpolatedMap[x, y] * 100f) / 100f;
                //para asegurar que esté entre 0 y 1 los valores despues de aplicar el falloffmap
                noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y], 0f, 1f);
            }
        }

       /*for (int y = 0; y < _tamañoChunk; y++)
        {
            for (int x = 0; x < _tamañoChunk; x++)
            {
                mapaRuido[x, y] = mapaRuido[x, y] - _falloffMap[Mathf.RoundToInt(coordenadasChunk.x), Mathf.RoundToInt(coordenadasChunk.y)];
                mapaRuido[x, y] = Mathf.Clamp(mapaRuido[x, y], 0f, 1f);
            }
        }*/
        
        return noiseMap;

    }

    /// <summary>
    /// Algoritmo para generar valores  del mapa de ruido con Perlin Noise
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    private float[,] noiseMapGenerator(Vector2 offset)
    {
        float[,] map = new float[_sizeOfChunk, _sizeOfChunk];

        //Aseguramos de no dividir por cero en escala
        if (_scale <= 0) { _scale = float.MinValue; }

        //A partir de la Seed encontramos los offset
        //Uno para cada octava
        System.Random rnd = new System.Random(_seed);
        Vector2[] octavesOffsets = new Vector2[_octaves];
        int i = 0;
        while (i < _octaves)
        {
            octavesOffsets[i] = new Vector2(rnd.Next(-1000, 1000) + offset.x, rnd.Next(-1000, 1000) - offset.y);
            i++;
        }


        float amplitude, frequency, heightOfMap, _x, _y, perlin;
        for (int y = 0; y < _sizeOfChunk; y++)
        {
            for (int x = 0; x < _sizeOfChunk; x++)
            {
                amplitude = 1;
                frequency = 1;
                heightOfMap = 0;

                i = 0;
                while (i < _octaves)
                {
                    _x = (x + octavesOffsets[i].x) / _scale * frequency; //Para conocer el rango de valores del noiseHeigth
                    _y = (y + octavesOffsets[i].y) / _scale * frequency;  //Para conocer el rango de valores del noiseHeigth

                    perlin = Mathf.PerlinNoise(_x, _y); //valor entre [0,1]
                    perlin = perlin * 2 - 1; //valor entre [-1,1]

                    heightOfMap += perlin * amplitude;

                    //modificamos los valores de amplitud y frecuencia con una relacion 2:1
                    amplitude *= _persistance;
                    frequency *= _lacunarity;

                    i++;
                }

                map[x, y] = heightOfMap;
            }
        }

        //Buscamos la posible mayor altura del cualquier mapa posible
        float maxHeight = 0;
        amplitude = 1;
        i = 0;
        while (i < _octaves)
        {
            maxHeight += amplitude;
            amplitude *= _persistance;

            i++;
        }

        map = normalizeMap(map, maxHeight);
        
        return map;
    }

    /// <summary>
    /// normaliza el mapa
    /// </summary>
    /// <param name="map"></param>
    /// <param name="heightMaxOfTheMap"></param>
    /// <returns></returns>
    private float[,] normalizeMap(float[,] map, float heightMaxOfTheMap)
    {
        float normalizedHeight;
        float[,] _map = map;

        for (int y = 0; y < _sizeOfChunk; y++)
        {
            for (int x = 0; x < _sizeOfChunk; x++)
            {
                //Normalizar mapa de ruido -> [0,1]
                normalizedHeight = (_map[x, y] + 1) / heightMaxOfTheMap; //Lo contrario a la hora de sacar el valor de perlin.
                _map[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
            }
        }
        return _map;
    }

    /// <summary>
    /// obtener el array de colores
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    private Color[] makeColorArray(float[,] map)
    {
        Color[] colors = new Color[_sizeOfChunk * _sizeOfChunk]; ;
        float actualHeight;
        int i;

        for (int y = 0; y < _sizeOfChunk; y++)
        {
            for (int x = 0; x < _sizeOfChunk; x++)
            {
                actualHeight = map[x, y];
                i = 0;
                while (i < biomes.Length)
                {
                    //almacena por orden en el array el color de cada pixel segun la altura del mapa de ruido
                    if (actualHeight >= biomes[i].Height)
                    {
                        colors[y * _sizeOfChunk + x] = biomes[i].Color;
                    }
                    else //Si la altura actual es más pequeña que la evaluada en el bioma
                    {
                        break;
                    }
                    i++;
                }
            }
        }
        return colors;
    }

    /// <summary>
    /// obtiene el array de colores en escala de grises
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    private Color[] makeColorArrayWithGrayscale(float[,] map)
    {
        Color[] color = new Color[_sizeOfChunk * _sizeOfChunk]; ;
        float actualHeight;
        int i;

        for (int y = 0; y < _sizeOfChunk; y++)
        {
            for (int x = 0; x < _sizeOfChunk; x++)
            {
                actualHeight = map[x, y];

                color[y * _sizeOfChunk + x] = Color.Lerp(Color.black, Color.white, actualHeight);
            }
        }
        return color;
    }

    /// <summary>
    /// Genera el mapa de altura y el array de color
    /// </summary>
    /// <param name="coordOfChunk"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    private HeightMapAndColorArray generateHeightMapAndColorArray(Vector2 coordOfChunk, Vector2 center)
    {
        float[,] noiseMap = noiseMapGenerator(center);

        
        noiseMap = applyFalloffMapToNoiseMap(noiseMap, coordOfChunk);


        //Color[] es el parametro que tiene que recibir la textura para aplicarle el color a cada pixel
        Color[] colorArray;
        colorArray = makeColorArray(noiseMap);
        //mapaDeColor = obtenerMapaColorEstalaGrises(mapaRuido);

        //Devuelvo un objeto que contiene el mapa de ruido y el mapa de color
        return new HeightMapAndColorArray(noiseMap, colorArray);
    }

    /// <summary>
    /// Algoritmo para generar dos datos de la malla. despues estos datos en el hilo principal se asociaran a un objeto de Unity
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="LOD"></param>
    /// <returns></returns>
    private DataOfMesh makeMesh(float[,] heightMap, int LOD)
    {
        //Calculo del incremento (salto entre vertices) para construir la malla con el LOD adecuado
        int increaseOfLOD;
        if (LOD != 0)
        {
            increaseOfLOD = LOD * 2;
        }
        else
        {
            increaseOfLOD = 1;
        }

        //calculo de los vértices que habrá en fila de la malla én funcion del LOD que se establezca
        int vPerLine = (_sizeOfChunk - 1) / increaseOfLOD + 1;

        //inicializamos la malla con el numero de vertices que tendrá, despues de calcular el LOD
        DataOfMesh dataOfMesh = new DataOfMesh(vPerLine);

        //1º parte del algoritmo para generar la malla 3D: con vertices compartidos.
        dataOfMesh = makeMeshWithSharedVertices(dataOfMesh, heightMap, increaseOfLOD, vPerLine);
        
        //2º parte del algoritmo para generar la malla 3D: caras indepentdientes.
        dataOfMesh = transformMeshFromSharedVerticesToIndependentFaces(dataOfMesh);

        return dataOfMesh;
    }

    /// <summary>
    /// 1º parte del algoritmo para la generacion de la malla 3D del terreno.
    /// </summary>
    /// <param name="dataOfMesh"></param>
    /// <param name="heightMap"></param>
    /// <param name="increaseOdLOD"></param>
    /// <param name="vPerLine"></param>
    /// <returns></returns>
    private DataOfMesh makeMeshWithSharedVertices(DataOfMesh dataOfMesh, float[,] heightMap, int increaseOdLOD, int vPerLine)
    {
        float xSupLeft = (_sizeOfChunk - 1) / -2f;
        float zSupLeft = xSupLeft * (-1);

        int indexOfvertex = 0;
        //Al acceder desde diferentes hilos a la curva genera valores erroneos, por lo que simplemente se crea una copia de la curva.
        AnimationCurve curveOfHeight = new AnimationCurve(curva.keys);

        for (int i = 0; i < _sizeOfChunk; i += increaseOdLOD)
        {
            for (int j = 0; j < _sizeOfChunk; j += increaseOdLOD)
            {
                //crea vertices vector3 y accede al mapa de alturas para situarlo a la altura que le corresponde. usa la curva y el multiplicador para acentuar las montañas.
                //Vector3 vertice = new Vector3(xSupIzq + j, mapaAlturas[j, i], zSupIzq - i);
                Vector3 vertex = new Vector3(xSupLeft + j, curveOfHeight.Evaluate(heightMap[j, i]) * _multiplier, zSupLeft - i);
                dataOfMesh.Vertices[indexOfvertex] = vertex;
                //actualiza el vertor de Uvs, tiene que ser entre 0 y 1.
                Vector2 uv = new Vector2(j / (float)_sizeOfChunk, i / (float)_sizeOfChunk);
                dataOfMesh.Uvs[indexOfvertex] = uv;

                //para evitar acceder a la ultima fila y ultima columna ya que para un vértice se añade los dos triangulos que entán debajo y a la derecha.
                if (j < _sizeOfChunk - 1 && i < _sizeOfChunk - 1)
                {
                    //Añadir los 2 trianglos de abajo y derecha.
                    dataOfMesh.AddTriangleToMesh(indexOfvertex, indexOfvertex + vPerLine + 1, indexOfvertex + vPerLine);
                    dataOfMesh.AddTriangleToMesh(indexOfvertex + vPerLine + 1, indexOfvertex, indexOfvertex + 1);
                }

                //siguiente vertice.
                indexOfvertex++;
            }
        }

        return dataOfMesh;
    }

    /// <summary>
    /// 2º parte del algoritmo para la generacion de la malla 3D del terreno
    /// </summary>
    /// <param name="dataOfMesh"></param>
    /// <returns></returns>
    private DataOfMesh transformMeshFromSharedVerticesToIndependentFaces(DataOfMesh dataOfMesh)
    {
        Vector3[] flatVertices = new Vector3[dataOfMesh.Triangles.Length];
        Vector2[] flatUvs = new Vector2[dataOfMesh.Triangles.Length];

        int index = 0;
        while (index < dataOfMesh.Triangles.Length)
        {
            flatVertices[index] = dataOfMesh.Vertices[dataOfMesh.Triangles[index]];
            flatUvs[index] = dataOfMesh.Uvs[dataOfMesh.Triangles[index]];

            dataOfMesh.Triangles[index] = index;

            index++;
        }

        dataOfMesh.Vertices = flatVertices;
        dataOfMesh.Uvs = flatUvs;

        return dataOfMesh;
    }

    /// <summary>
    /// Metodo para obtener los datos del mapa altura y color dentro del hilo
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="center"></param>
    /// <param name="method"></param>
    void generateHeightMapAndColorArrayInThread(Vector2 coord, Vector2 center, Action<HeightMapAndColorArray> method)
    {
        //genera los datos: mapa de alturas y mapa de color
        HeightMapAndColorArray heightMapAndColorArray = generateHeightMapAndColorArray(coord, center);

        //Bloqueo si algún otro hilo esta usando la cola para evitar fallos de ejecución. añade a la cola un objeto de la clase ThreadInfoMapa el metodo (metodo) y el parametro que necesita (mapaAyC)
        lock (_queueHeightMapAndColorArray)
        {
            _queueHeightMapAndColorArray.Enqueue(new ThreadInfo<HeightMapAndColorArray>(method, heightMapAndColorArray));
        }
    }

    /// <summary>
    /// Metodo para obtener los datos de la malla dentro del hilo
    /// </summary>
    /// <param name="heightMapAndColorArray"></param>
    /// <param name="increaseOfLOD"></param>
    /// <param name="Method"></param>
    void generateMeshInThread(HeightMapAndColorArray heightMapAndColorArray, int increaseOfLOD, Action<DataOfMesh> Method)
    {
        //genera la los datos de la malla
        DataOfMesh dataOfMesh = makeMesh(heightMapAndColorArray.HeightMap, increaseOfLOD);
        
        //Bloqueo si algún otro hilo esta usando la cola para evitar fallos de ejecución. añade a la cola un objeto de la clase ThreadInfoMapa el metodo (metodo) y el parametro que necesita (mapaAyC)
        lock (_queueDataOfMesh)
        {
            _queueDataOfMesh.Enqueue(new ThreadInfo<DataOfMesh>(Method, dataOfMesh));
        }
    }

    //Getters 
    public float ScaleChunkSize
    {
        get
        {
            return _scaleChunkSize;
        }
    }

    public float Multiplier
    {
        get
        {
            return _multiplier;
        }
    }

    public Vector2 ActualPositionOfPlayer
    {
        get
        {
            return _actualPositionOfPlayer;
        }
    }

    public  float MaxDistanceVisibleOfPlayer
    {
        get
        {
            return _maxDistanceVisibleOfPlayer;
        }
    }

    public int SizeOfChunk
    {
        get
        {
            return _sizeOfChunk;
        }
    }

    public int NumberOfChunkInTerrain
    {
        get
        {
            return _numberOfChunkInTerrain;
        }
    }
}