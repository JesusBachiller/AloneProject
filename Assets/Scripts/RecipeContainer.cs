using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.IO;
using UnityEngine;

public class ResourceXML
{
    [XmlAttribute("id")]
    public int Id;

    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("number")]
    public int Number;
}

public class Recipe
{
    [XmlAttribute("id")]
    public int Id;

    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("number")]
    public int Number;

    [XmlAttribute("isStructure")]
    public int IsStructure;

    [XmlAttribute("openingMode")]
    public int OpeningMode;

    [XmlElement("Resource")]
    public List<ResourceXML> Resources;
}

[XmlRoot("RecipeCollection")]
public class RecipeContainer
{
    [XmlArray("Recipes")]
    [XmlArrayItem("Recipe", typeof(Recipe))]
    private List<Recipe> _recipes = new List<Recipe>();

    public static RecipeContainer Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(RecipeContainer));
        StreamReader reader = new StreamReader("Assets/Scripts/Recipes.xml");
        RecipeContainer recipeContainer = (RecipeContainer)serializer.Deserialize(reader);
        reader.Close();
        

        return recipeContainer;
}

    public List<Recipe> Recipes
    {
        get
        {
            return _recipes;
        }
    }
}
