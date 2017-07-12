// based on this tutorial: https://www.youtube.com/playlist?list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;

    public GameObject inventorySlot;
    public GameObject inventoryItem;

    //public GameObject 

    int slotAmount = 9;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    private void Start()
    {
        database = GetComponent<ItemDatabase>();

        slotAmount = 9;
        inventoryPanel = GameObject.Find("InventoryPanel");
        slotPanel = inventoryPanel.transform.FindChild("SlotPanel").gameObject;

        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].GetComponent<Slot>().slotID = i;
        }

        //AddItem(0);
        //AddItem(1);
        //AddItem(1);
        //AddItem(1);
        //AddItem(1);
        //AddItem(1);
        //AddItem(0);
        //RemoveItem(0);

        Debug.Log(items[1].Title);
    }

    public void AddItem(int id)
    {
        Debug.Log("Adding item to inventory");

        Item itemToAdd = database.FetchItemByID(id);

        int index = GetIndexOfItemInInventory(itemToAdd);

        if (itemToAdd.Stackable && index != -1)
        {
            ItemData data = slots[index].transform.GetChild(0).GetComponent<ItemData>();
            data.amount++;
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    itemObj.transform.SetParent(slots[i].transform);
                    itemObj.transform.position = itemObj.transform.parent.position;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                    itemObj.name = itemToAdd.Title;

                    ItemData data = itemObj.GetComponent<ItemData>();
                    data.amount++;
                    data.item = itemToAdd;
                    data.slotID = i;

                    break;
                }
            }
        }


    }

    int GetIndexOfItemInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
            {
                return i;
            }
        }

        return -1;
    }

    // based on SkaterAdsk6's comment: https://www.youtube.com/watch?v=Rs6-9DpUIo0&index=7&list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG
    public void RemoveItem(int id)
    {
        Item itemToRemove = database.FetchItemByID(id);

        int index = GetIndexOfItemInInventory(itemToRemove);

        if (itemToRemove.Stackable && index != -1)
        {
            ItemData data = slots[index].transform.GetChild(0).GetComponent<ItemData>();
            data.amount--;
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();

            if (data.amount == 0)
            {
                Destroy(slots[index].transform.GetChild(0).gameObject);
                items[index] = new Item();
            }
            else if (data.amount == 1)
            {
                //data amount text
                slots[index].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
        else for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1 && items[i].ID == id)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                    items[i] = new Item();
                    break;
                }
            }
    }

    public void UpdateInventoryItems()
    {
        Debug.Log("Updating the items list in inventory");

        for (int i = 0; i < slotPanel.transform.childCount; i++)
        {
            Transform slot = slotPanel.transform.GetChild(i);
            Item itemInSlot = slot.childCount >= 1 ? slot.GetChild(0).GetComponent<ItemData>().item : new Item();
            items[i] = itemInSlot;
        }
    }
}


//bool CheckIfItemIsInInventory(Item item)
//{
//    for (int i = 0; i < items.Count; i++)
//    {
//        if (items[i].ID == item.ID)
//        {
//            return true;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
//        }
//    }
//    return false;
//}
