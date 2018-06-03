using UnityEngine;
using UnityEngine.UI;

public class ButtonInventoryCanvas : MonoBehaviour
{
    [SerializeField]
    private int _id;

    RecipeContainer rc;
    Recipe _myRecipe;

    [SerializeField]
    private int[] _myOpenModes;
    [SerializeField]
    private int _actualMode;

    public GameObject GameResource;

    /// <summary>
    /// Actualiza la descripcion de la receta del objeto
    /// </summary>
    public void MouseEnter()
    {
        GameObject description = GameObject.FindGameObjectWithTag("DescriptionTextInventoryCanvas");
        if (GetComponent<Button>().interactable)
        {
            string recipeResource = "";
            //if (_myRecipe == null) { Debug.Log("null"); return; }
            foreach(ResourceXML r in _myRecipe.Resources)
            {
                recipeResource += r.Name + " x" + r.Number + "\n";
            }
            description.GetComponent<Text>().text = recipeResource;
            
        }else
        {
            description.GetComponent<Text>().text = "No disponible";
        }
        
    }

    public void MouseExit()
    {
        GameObject description = GameObject.FindGameObjectWithTag("DescriptionTextInventoryCanvas");
        description.GetComponent<Text>().text = "";
    }
    public void MouseDown()
    {
        if (GetComponent<Button>().interactable)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            foreach (ResourceXML resource in _myRecipe.Resources)
            {
                player.GetComponent<Inventory>().removeResource(resource.Id, resource.Number);
            }
            
            if(!player.GetComponent<Inventory>().addResources(_myRecipe.Id, _myRecipe.Number, GameResource, (_myRecipe.IsStructure == 1), _myRecipe.OpeningMode)){
                for(int i = 0; i < _myRecipe.Number; i++)
                {
                    Camera camera = player.transform.GetChild(0).GetComponent<Camera>();
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                    GameObject inst = Instantiate(GameResource,
                                                  camera.transform.position + new Vector3(UnityEngine.Random.Range(2.0f, 4.5f), UnityEngine.Random.Range(2.0f, 4.5f), UnityEngine.Random.Range(2.0f, 4.5f)) + ray.direction.normalized * 7,
                                                  Quaternion.identity,
                                                  GameObject.FindGameObjectWithTag("Terrain").transform);
                    inst.GetComponent<NoNaturalResource>().InitNoNaturalResource(_myRecipe.Id, player.GetComponent<Player>(), GameResource, (_myRecipe.IsStructure == 1), _myRecipe.OpeningMode);
                }     
            }

            GameObject.FindGameObjectWithTag("DropdownInventoryCanvas").GetComponent<DropdownController>().UpdateValue(_actualMode);

        }
    }

    /// <summary>
    /// mira la disponibilidad de este boton según los objetos del inventario y el modo de abertura. MODE Const Assist
    /// </summary>
    /// <param name="inventoryId"></param>
    /// <param name="inventoryValues"></param>
    /// <param name="recipeContainer"></param>
    /// <param name="mode"></param>
    public void CheckAvailability(int[] inventoryId, int[] inventoryValues, RecipeContainer recipeContainer, int mode)
    {
        _actualMode = mode;
        rc = recipeContainer;

        if (_myRecipe == null)
        {
            foreach (Recipe recipe in rc.Recipes)
            {
                if (recipe.Id == _id)
                {
                    _myRecipe = recipe;
                    break;
                }
            }
        }

        if (_myRecipe != null)
        {
            bool haveAll = true;
            foreach (ResourceXML resource in _myRecipe.Resources)
            {
                bool isInInventory = false;
                for(int i = 0; i < inventoryId.Length; i++)
                {
                    if (inventoryId[i] == resource.Id && inventoryValues[i] >= resource.Number)
                    {
                        isInInventory = true;
                    }
                }

                if (!isInInventory)
                {
                    haveAll = false;
                }
            }

            if (haveAll)
            {
                GetComponent<Button>().interactable = true;
            } else
            {
                GetComponent<Button>().interactable = false;
            }
        }else{
            GetComponent<Button>().interactable = false;
        }

        //Comprobacion de que se pueda activar el boton en el actual mode

        if (GetComponent<Button>().interactable)
        {
            foreach (int m in _myOpenModes)
            {
                //Debug.Log("openmode: " + m + "---actual: " + _actualMode);
                if (m == _actualMode)
                {
                    return;
                }
            }
            GetComponent<Button>().interactable = false;
        }
    }
}
