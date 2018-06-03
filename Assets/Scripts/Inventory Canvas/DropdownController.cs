using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour {
    [SerializeField]
    private GameObject[] Material;
    [SerializeField]
    private GameObject[] Structures;
    [SerializeField]
    private GameObject[] Tools;
    [SerializeField]
    private GameObject[] Food;
    [SerializeField]
    private int _actualMode;

    // Use this for initialization
    private void OnEnable()
    {
        if (Material.Length == 0 || Structures.Length == 0 || Tools.Length == 0 || Food.Length == 0)
        {
            Material = GameObject.FindGameObjectsWithTag("RCanvasMaterial");
            Structures = GameObject.FindGameObjectsWithTag("RCanvasStrutures");
            Tools = GameObject.FindGameObjectsWithTag("RCanvasTools");
            Food = GameObject.FindGameObjectsWithTag("RCanvasFood");
        }
        
    }

    /// <summary>
    /// Esconde todos los botones
    /// </summary>
    private void clearCanvas()
    {
        foreach (GameObject go in Material)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in Structures)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in Tools)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in Food)
        {
            go.SetActive(false);
        }
    }

    public void UpdateValue()
    {
        UpdateValue(GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().InventoryId,
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().InventoryValues,
                    transform.parent.GetComponent<InventoryCanvasController>().RecipeContainer,
                    _actualMode);
    }

    public void UpdateValue(int mode)
    {
        _actualMode = mode;
        UpdateValue(GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().InventoryId,
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().InventoryValues,
                    transform.parent.GetComponent<InventoryCanvasController>().RecipeContainer, 
                    mode);
    }


    /// <summary>
    /// actualiza para mostrar los botones disponibles. MODE: Constant Assist
    /// </summary>
    public void UpdateValue(int[] inventoryId, int[] inventoryValues, RecipeContainer recipeContainer, int mode)
    {
        _actualMode = mode;
        clearCanvas();
        switch (GetComponent<Dropdown>().value)
        {
            case 0:
                foreach (GameObject go in Material)
                {
                    go.GetComponent<ButtonInventoryCanvas>().CheckAvailability(inventoryId, inventoryValues, recipeContainer, mode);
                    go.SetActive(true);
                }
                break;
            case 1:
                foreach (GameObject go in Structures)
                {
                    go.GetComponent<ButtonInventoryCanvas>().CheckAvailability(inventoryId, inventoryValues, recipeContainer, mode);
                    go.SetActive(true);
                }
                break;
            case 2:
                foreach (GameObject go in Tools)
                {
                    go.GetComponent<ButtonInventoryCanvas>().CheckAvailability(inventoryId, inventoryValues, recipeContainer, mode);
                    go.SetActive(true);
                }
                break;
            case 3:
                foreach (GameObject go in Food) 
                {
                    go.GetComponent<ButtonInventoryCanvas>().CheckAvailability(inventoryId, inventoryValues, recipeContainer, mode);
                    go.SetActive(true);
                }
                break;

        }
    }
}
