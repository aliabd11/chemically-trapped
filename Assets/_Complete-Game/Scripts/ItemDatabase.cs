// based on this tutorial: https://www.youtube.com/playlist?list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    private List<Item> database = new List<Item>();
    private JsonData itemsDatabase;

    private void Start()
    {
        itemsDatabase = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/_Complete-Game/StreamingAssets/Items.json"));
        ConstructItemsDatabase();

        Debug.Log("Databse constructed");
    }

    void ConstructItemsDatabase()
    {
        for (int i = 0; i < itemsDatabase.Count; i++)
        {
            JsonData data = itemsDatabase[i];
            database.Add(new Item((int)data["id"], data["title"].ToString(), (int)data["value"],
                (int)data["stats"]["power"], (int)data["stats"]["defence"], (int)data["stats"]["vitality"],
                data["description"].ToString(), (bool)data["stackable"], (int)data["rarity"], data["slug"].ToString()));
        }
    }

    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
            {
                return database[i];
            }
        }
        return null;
    }
}


[System.Serializable] // allows it to show on the inspector
public class Item
{
    public int ID { get; set; } //property (always starts with capital letter)
    public string Title { get; set; }
    public int Value { get; set; }
    public int Power { get; set; }
    public int Defence { get; set; }
    public int Vitality { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public int Rarity { get; set; }
    public string Slug { get; set; }

    public Sprite Sprite { get; set; }

    public Item()
    {
        this.ID = -1; // will never be used in database so something is wrong if this happens
    }

    public Item(int id, string title, int value, int power, int defence,
        int vitality, string description, bool stackable, int rarity, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Value = value;
        this.Power = power;
        this.Defence = defence;
        this.Vitality = vitality;
        this.Description = description;
        this.Stackable = stackable;
        this.Rarity = rarity;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Sprites/" + slug);
        //this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + slug); // WILL CHANGE
    }
}