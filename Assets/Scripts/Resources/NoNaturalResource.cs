using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NoNaturalResource : MonoBehaviour
{
    private int _id;
    [SerializeField]
    private bool _isStructure;
    [SerializeField]
    private int _openingMode;

    private Player _player;
    private Inventory _playerInventory;
    [SerializeField]
    private GameObject _myPrefab;

    /// <summary>
    /// Inicializador de recursos no naturales.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="player"></param>
    /// <param name="myPrefab"></param>
    /// <param name="isStructure"></param>
    /// <param name="openingMode"></param>
    public void InitNoNaturalResource(int id, Player player, GameObject myPrefab, bool isStructure, int openingMode)
    {
        _id = id;
        _player = player;
        _playerInventory = _player.transform.GetComponent<Inventory>();
        _myPrefab = myPrefab;

        _openingMode = openingMode;

        _isStructure = isStructure;
    }

    /// <summary>
    /// Habre el inventario del jugador segun su modo de apertura
    /// </summary>
    public void OpenInventory()
    {
         _player.transform.GetComponent<Inventory>().openInventory(_openingMode);
    }

    /// <summary>
    /// Se llama a esta fución cuando el raton pasa por encima del este objeto
    /// </summary>
    private void OnMouseEnter()
    {
        _playerInventory.ActualOpeningMode = _openingMode;
        _playerInventory.NameOfActualStructure = name.Split('(')[0];
    }

    /// <summary>
    /// Se llama a esta fución cuando el raton deja de esta encima del este objeto
    /// </summary>
    private void OnMouseExit()
    {
        //reseteo de modo de apertura al default
        _playerInventory.ActualOpeningMode = ConstantAssistant.MODE_INVENTORY_PLAYER;
        _playerInventory.NameOfActualStructure = "";

        _player.GetComponentInChildren<Animator>().SetBool("Recollect", false);
    }

    /// <summary>
    /// Se llama a esta fución cuando se hace click en este objeto
    /// </summary>
    private void OnMouseDown()
    {
        //inici animacion de la mano
        _player.GetComponentInChildren<Animator>().SetBool("Recollect", true);

        if (!_isStructure)
        {
            if (_playerInventory.addResources(_id, 1, _myPrefab, _isStructure, _openingMode))
            {
                Destroy(gameObject);
            }
        }else if(!GameController.InventoryIsOpen){
            if (_playerInventory.addResources(_id, 1, _myPrefab, _isStructure, _openingMode))
            {
                OnMouseExit();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Se llama cuando se destruye el objeto
    /// </summary>
    private void OnDestroy()
    {
        if(_player !=null)
        _player.GetComponentInChildren<Animator>().SetBool("Recollect", false);
    }

    public int OpeningMode
    {
        get
        {
            return _openingMode;
        }
    }
}
