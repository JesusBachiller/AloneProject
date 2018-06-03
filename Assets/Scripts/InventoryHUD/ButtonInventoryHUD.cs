using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInventoryHUD : MonoBehaviour {

    [SerializeField]
    private bool mouseDown = false;
    [SerializeField]
    private bool mouseExit = true;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private int _idActual;
    private int _previousId;
    [SerializeField]
    private int _valueActual;
    [SerializeField]
    private int _isStructure;
    [SerializeField]
    private int _openingMode;

    public GameObject Anvil;
    public GameObject Box;
    public GameObject Coal;
    public GameObject CoockedSeed;
    public GameObject Iron;
    public GameObject IronFurnace;
    public GameObject IronPlate;
    public GameObject IronTable;
    public GameObject Stone;
    public GameObject StoneFurnace;
    public GameObject Rope;
    public GameObject Seed;
    public GameObject Steel;
    public GameObject TreeTrunk;
    public GameObject WoodenTable;
    public GameObject WoodenPlate;
    public GameObject WoodenStick;
    
    public GameObject NumItems;

    [SerializeField]
    private GameObject myImage;
    [SerializeField]
    private GameObject myNumItem;

    /// <summary>
    /// Actualiza el inventario con las imagenes que le corresponda y con el numero que le corresponda.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <param name="isStructure"></param>
    /// <param name="openingMode"></param>
    public void UpdateButtonInventoryHUD(int id, int value, int isStructure, int openingMode)
    {
        _previousId = _idActual;
        _idActual = id;
        _valueActual = value;
        _isStructure = isStructure;
        _openingMode = openingMode;

        if (_idActual == -1)
        {
            if(myImage != null && myNumItem != null)
            {
                Destroy(myImage);
                Destroy(myNumItem);
            }
        }
        else
        {
            if (_idActual != _previousId)
            {
                if (myImage != null && myNumItem != null)
                {
                    Destroy(myImage);
                    Destroy(myNumItem);
                }

                switch (_idActual)
                {
                    case (int)ConstantAssistant.EnumNoNaturalResources.Anvil:
                        myImage = Instantiate(Anvil, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Coal:
                        myImage = Instantiate(Coal, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Box:
                        myImage = Instantiate(Box, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.CookedSeed:
                        myImage = Instantiate(CoockedSeed, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Iron:
                        myImage = Instantiate(Iron, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.IronFurnace:
                        myImage = Instantiate(IronFurnace, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.IronPlate:
                        myImage = Instantiate(IronPlate, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.IronTable:
                        myImage = Instantiate(IronTable, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Steel:
                        myImage = Instantiate(Steel, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Stone:
                        myImage = Instantiate(Stone, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.StoneFurnace:
                        myImage = Instantiate(StoneFurnace, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Rope:
                        myImage = Instantiate(Rope, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.Seed:
                        myImage = Instantiate(Seed, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.TreeTrunk:
                        myImage = Instantiate(TreeTrunk, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.WoodenPlate:
                        myImage = Instantiate(WoodenPlate, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.WoodenStick:
                        myImage = Instantiate(WoodenStick, transform);
                        break;
                    case (int)ConstantAssistant.EnumNoNaturalResources.WoodTable:
                        myImage = Instantiate(WoodenTable, transform);
                        break;

                }

                myNumItem = Instantiate(NumItems, transform);
                myNumItem.GetComponent<Text>().text = "" + value;
            }
            else
            {
                if (myImage == null)
                {
                    switch (_idActual)
                    {
                        case (int)ConstantAssistant.EnumNoNaturalResources.Anvil:
                            myImage = Instantiate(Anvil, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Coal:
                            myImage = Instantiate(Coal, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Box:
                            myImage = Instantiate(Box, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.CookedSeed:
                            myImage = Instantiate(CoockedSeed, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Iron:
                            myImage = Instantiate(Iron, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.IronFurnace:
                            myImage = Instantiate(IronFurnace, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.IronPlate:
                            myImage = Instantiate(IronPlate, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.IronTable:
                            myImage = Instantiate(IronTable, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Steel:
                            myImage = Instantiate(Steel, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Stone:
                            myImage = Instantiate(Stone, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.StoneFurnace:
                            myImage = Instantiate(StoneFurnace, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Rope:
                            myImage = Instantiate(Rope, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.Seed:
                            myImage = Instantiate(Seed, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.TreeTrunk:
                            myImage = Instantiate(TreeTrunk, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.WoodenPlate:
                            myImage = Instantiate(WoodenPlate, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.WoodenStick:
                            myImage = Instantiate(WoodenStick, transform);
                            break;
                        case (int)ConstantAssistant.EnumNoNaturalResources.WoodTable:
                            myImage = Instantiate(WoodenTable, transform);
                            break;

                    }

                }
                if (myNumItem != null)
                {
                    myNumItem.GetComponent<Text>().text = "" + value;
                }
                else
                {

                    myNumItem = Instantiate(NumItems, transform);
                    myNumItem.GetComponent<Text>().text = "" + value;
                }
            }
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _idActual = -1;
        _valueActual = 0;
        _isStructure = -1;
        _openingMode = -1;
    }


    public void MouseDown()
    {
        mouseDown = true;
    }

    /// <summary>
    /// Fundo que hace que se suelten los recursos desde el inventario al suelo
    /// </summary>
    public void MouseUp()
    {
        //Si se ha hecho click en un slot y se ha sacado fuera de ese slot -> suelta el recurso en el suelo
        if(mouseDown && mouseExit && _idActual != -1)
        {
            Ray ray = _player.transform.GetChild(0).GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            _player.GetComponent<Inventory>().instanciate(_idActual, _player, _player.transform.GetChild(0), ray.direction);
            _player.GetComponent<Inventory>().removeResource(_idActual, 1);

            if (_valueActual == 0)
            {
                _idActual = -1;
                _valueActual = 0;

                
            }
        }

        mouseDown = false;
    }

    public void MouseExit()
    {
        mouseExit = true;
    }

    public void MouseEnter()
    {
        mouseExit = false;
    } 
}
