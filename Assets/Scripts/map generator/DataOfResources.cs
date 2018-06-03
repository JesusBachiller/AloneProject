using UnityEngine;

[System.Serializable]
public class DataOfResources{

    [SerializeField]
    private string _myBiome;

    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private GameObject _gameObject;

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="biome"></param>
    /// <param name="pos"></param>
    public DataOfResources(string biome, Vector3 pos)
    {
        _myBiome = biome;
        _position = pos;
    }

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="biome"></param>
    /// <param name="pos"></param>
    /// <param name="gameObject"></param>
    public DataOfResources(string biome, Vector3 pos, GameObject gameObject)
    {
        _myBiome = biome;
        _position = pos;
        _gameObject = gameObject;
    }

    //getters y setters
    public GameObject GameObject
    {
        get
        {
            return _gameObject;
        }

        set
        {
            _gameObject = value;
        }
    }

    public string MyBiome
    {
        get
        {
            return _myBiome;
        }
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }
    }
}
