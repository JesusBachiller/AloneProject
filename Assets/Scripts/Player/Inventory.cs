using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Inventory : MonoBehaviour
{
    private int[] _inventoryValues;
    private int[] _inventoryIdEnum;
    [SerializeField]
    private int[] _inventoryIsStructure;
    [SerializeField]
    private int[] _inventoryOpeningMode;

    [SerializeField]
    private int _actualOpeningMode;
    private string _nameOfActualStructure;

    [SerializeField]
    private GameObject[] _inventoryPrefabs;
    
    [SerializeField]
    private GameObject _cSCanvas;
    [SerializeField]
    private GameObject _inventoryCanvas;
    [SerializeField]
    private GameObject _inventoryHUD;
    [SerializeField]
    private GameObject _pressECanvas;

    private void Start()
    {
        _inventoryValues = new int[ConstantAssistant.INVENTORY_SLOT];
        _inventoryIdEnum = new int[ConstantAssistant.INVENTORY_SLOT];
        _inventoryIsStructure = new int[ConstantAssistant.INVENTORY_SLOT];
        _inventoryOpeningMode = new int[ConstantAssistant.INVENTORY_SLOT];

        _inventoryPrefabs = new GameObject[ConstantAssistant.INVENTORY_SLOT];

        //inicializa inventorio
        for(int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            _inventoryValues[i] = 0;
            _inventoryIdEnum[i] = (int)ConstantAssistant.EnumNoNaturalResources.empty;
            _inventoryIsStructure[i] = -1;
            _inventoryOpeningMode[i] = -1;
            _inventoryPrefabs[i] = null;
        }

        _cSCanvas = GameObject.FindGameObjectWithTag("CSCanvas");
        _inventoryCanvas = GameObject.FindGameObjectWithTag("InventoryCanvas");
        _inventoryHUD = GameObject.FindGameObjectWithTag("InventoryHUD");

        _pressECanvas = _cSCanvas.transform.GetChild(2).gameObject;
        _pressECanvas.SetActive(false);

        _inventoryCanvas.SetActive(false);

    }

    /// <summary>
    /// instancia el recurso con id = id en el mundo. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="player"></param>
    /// <param name="camera"></param>
    /// <param name="seeDirection"></param>
    public void instanciate(int id, GameObject player, Transform camera, Vector3 seeDirection)
    {
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            if (_inventoryIdEnum[i] == id)
            {
                GameObject inst = Instantiate(_inventoryPrefabs[i],
                                              camera.position + seeDirection.normalized*7,
                                              Quaternion.identity,
                                              GameObject.FindGameObjectWithTag("Terrain").transform);

                inst.GetComponent<NoNaturalResource>().InitNoNaturalResource(id, player.GetComponent<Player>(), _inventoryPrefabs[i], (_inventoryIsStructure[i] == 1), _inventoryOpeningMode[i]);
                break;
            }
        }
    }

    /// <summary>
    /// Añade un recurso al inventaro.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="numberOfResources"></param>
    /// <param name="prefabsResource"></param>
    /// <returns>
    /// Devuelve TRUE si se ha podido añadir el recurso al inventario. FALSE si no hay hueco en el inventario.
    /// </returns>
    public bool addResources(int id, int numberOfResources, GameObject prefab, bool isStructure, int openingMode)
    {
        //busca si hay en el inventario un recurso con su mismo id para almacenarlo tambien en ese slot
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            if (_inventoryIdEnum[i] == id)
            {
                _inventoryValues[i] += numberOfResources;

                debugInventory();

                updateInventoryHUD();

                return true;
            }
        }
        //si no hay un recurso del mismo tipo en el inventario, busca un espacio vacio en el inventario
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            if (_inventoryIdEnum[i] == (int)ConstantAssistant.EnumNoNaturalResources.empty)
            {
                _inventoryIdEnum[i] = id;
                _inventoryValues[i] = numberOfResources;
                _inventoryOpeningMode[i] = openingMode;
                _inventoryPrefabs[i] = prefab;
                _inventoryIsStructure[i] = isStructure ? 1 : 0;

                debugInventory();

                updateInventoryHUD();

                return true;
            }
        }

        //si tampoco hay espacio vacio debuelve false
        debugInventory();
        updateInventoryHUD();
        return false;
    }

    /// <summary>
    /// Actualiza el HUD del inventario
    /// </summary>
    private void updateInventoryHUD()
    {
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            _inventoryHUD.transform.GetChild(i).GetComponent<ButtonInventoryHUD>().UpdateButtonInventoryHUD(_inventoryIdEnum[i], _inventoryValues[i], _inventoryIsStructure[i], _inventoryOpeningMode[i]);
        }
    }

    /// <summary>
    /// Quita recursos del inventario
    /// </summary>
    /// <param name="id"></param>
    /// <param name="numberOfResources"></param>
    /// <returns>
    /// Devulve TRUE si se puede eliminar el recurso del inventario. FALSE si no se puede
    /// </returns>
    public bool removeResource(int id, int numberOfResources)
    {
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            if(_inventoryIdEnum[i] == id)
            {
                //si hay menos numero de un recurso que los que se quieren quitar
                if (_inventoryValues[i]<numberOfResources)
                {
                    Debug.Log("no tengo tantos recursos");
                    debugInventory();
                    updateInventoryHUD();
                    return false;
                }

                _inventoryValues[i] -= numberOfResources;

                //Si se ha vaciado el slot del inventario. Se resetea ese slot
                if(_inventoryValues[i] == 0)
                {
                    _inventoryIdEnum[i] = -1;
                    _inventoryPrefabs[i] = null;
                    _inventoryIsStructure[i] = -1;
                    _inventoryOpeningMode[i] = -1;
                }
                debugInventory();
                updateInventoryHUD();
                return true;
            }
        }
        debugInventory();
        updateInventoryHUD();
        return false;
    }

    /// <summary>
    /// Debug en consola
    /// </summary>
    public void debugInventory()
    {
        string debug = "";
        for (int i = 0; i < ConstantAssistant.INVENTORY_SLOT; i++)
        {
            debug += "SLOT " + (i+1) + " - id: " + _inventoryIdEnum[i] + " -- Num: " + _inventoryValues[i] + "\n";
        }
        //Debug.Log(debug);
    }

    private void Update()
    {

        if(_actualOpeningMode != ConstantAssistant.MODE_INVENTORY_PLAYER)
        {
            //Muestra mensaje en pantalla del qué inventario se va a abrir
            _pressECanvas.GetComponentInChildren<Text>().text = "Press E to open " + _nameOfActualStructure + " inventory";
            _pressECanvas.SetActive(true);
        }
        else
        { 
            //Se oculta el mensaje anterior
            _pressECanvas.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Se abre el inventario segun el modo que se le pase a la función. Por ejemplo: mode = 0 --> inventario del jugador
            openInventory(_actualOpeningMode);
        }
    }

    /// <summary>
    /// Detiene el juego y muestra el inventario en el HUD
    /// </summary>
    /// <param name="mode"></param>
    public void openInventory(int mode)
    {
        RigidbodyFirstPersonController RFPC = transform.GetComponent<RigidbodyFirstPersonController>();

        GameController.InventoryIsOpen = !GameController.InventoryIsOpen;

        RFPC.mouseLook.SetCursorLock(!GameController.InventoryIsOpen);
        RFPC.enabled = !GameController.InventoryIsOpen;

        _cSCanvas.SetActive(!GameController.InventoryIsOpen);
        _inventoryCanvas.SetActive(GameController.InventoryIsOpen);
        _inventoryCanvas.GetComponent<InventoryCanvasController>().Activate(_inventoryIdEnum, _inventoryValues, mode);
    }

    //GET AND SET
    public string NameOfActualStructure
    {
        get
        {
            return _nameOfActualStructure;
        }
        set
        {
            _nameOfActualStructure = value;
        }
    }
    
    public int ActualOpeningMode
    {
        get
        {
            return _actualOpeningMode;
        }
        set
        {
            _actualOpeningMode = value;
        }
    }

    public int[] InventoryId
    {
        get
        {
            return _inventoryIdEnum;
        }
        set
        {
            _inventoryIdEnum = value;
        }
    }
    public int[] InventoryValues
    {
        get
        {
            return _inventoryValues;
        }
        set
        {
            _inventoryValues = value;
        }
    }
}