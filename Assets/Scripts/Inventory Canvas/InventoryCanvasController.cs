using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InventoryCanvasController : MonoBehaviour
{
    [SerializeField]
    private GameObject _dropdown;

    private RecipeContainer _recipeContainer;
    

    private void OnEnable()
    {
        _dropdown = GameObject.FindGameObjectWithTag("DropdownInventoryCanvas");

        _recipeContainer = RecipeContainer.Load();
    }

    /// <summary>
    /// Actualiza los botones del canvas segun el modo en el que se haya abierto.
    /// MODE: 0=Inventory; 1=WoodTable
    /// </summary>
    /// <param name="inventoryId"></param>
    /// <param name="inventoryValue"></param>
    /// <param name="mode"></param>
    public void Activate(int[] inventoryId, int[] inventoryValue, int mode)
    {
        _dropdown.GetComponent<DropdownController>().UpdateValue(inventoryId, inventoryValue, _recipeContainer, mode);
    }

    public RecipeContainer RecipeContainer
    {
        get
        {
            return _recipeContainer;
        }
    }
}
