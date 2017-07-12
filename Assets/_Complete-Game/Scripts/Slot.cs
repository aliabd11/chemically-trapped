// based on this tutorial: https://www.youtube.com/playlist?list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    public int slotID;

    private Inventory inventory;

    private void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        Debug.Log("this slot's slotID: " + slotID + "\ndropped item's slotID (before anything happens): " + droppedItem.slotID);

        if (inventory.items[slotID].ID == -1) // no item in slot
        {
            Debug.Log("OnDrop: no item in slot");

            //inventory.items[droppedItem.slotID] = new Item();

            //inventory.items[slotID] = droppedItem.item;
            droppedItem.slotID = slotID;

            //Debug.Log("inventory's items list at index " + slotID + " updated with " + droppedItem.item.Title);
        }
        else if (droppedItem.slotID != slotID)
        {
            Debug.Log("OnDrop: slotIDs not the same");

            Transform itemInSlot = this.transform.GetChild(0);
            ItemData dataOfItemInSlot = itemInSlot.GetComponent<ItemData>();

            if (dataOfItemInSlot.item.ID != droppedItem.item.ID || !dataOfItemInSlot.item.Stackable)
            {
                Debug.Log("OnDrop: item IDs are different and/or item already in slot is not stackable");

                dataOfItemInSlot.slotID = droppedItem.slotID;
                itemInSlot.transform.SetParent(inventory.slots[droppedItem.slotID].transform);
                itemInSlot.transform.position = inventory.slots[droppedItem.slotID].transform.position;
                //inventory.items[droppedItem.slotID] = dataOfItemInSlot.item;

                droppedItem.slotID = slotID;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;

                //inventory.items[slotID] = droppedItem.item;
            }
            else
            {
                Debug.Log("OnDrop: stack items together");
                Destroy(eventData.pointerDrag);
                dataOfItemInSlot.amount++;
                dataOfItemInSlot.transform.GetChild(0).GetComponent<Text>().text = dataOfItemInSlot.amount.ToString();
            }


            // handle case that drops on different slots but same item

        }
        //else if (droppedItem.slotID == slotID)
        //{
        //    Debug.Log("destroy item");
        //    Destroy(eventData.pointerDrag);
        //    ItemData data = this.transform.GetChild(0).GetComponent<ItemData>();
        //    data.amount++;
        //    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
        //}
    }
}
