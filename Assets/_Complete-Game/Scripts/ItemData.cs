// based on this tutorial: https://www.youtube.com/playlist?list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int amount;
    public int slotID;

    private Transform parentSlot;
    private Transform canvas;

    private Inventory inventory;
    private Tooltip tooltip;

    private void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        tooltip = inventory.GetComponent<Tooltip>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("starting to drag item at slotID: " + slotID);

            parentSlot = this.transform.parent;
            canvas = parentSlot.parent.parent.parent;

            // avoid rendering below other objects
            this.transform.SetParent(canvas);
            this.transform.position = eventData.position;

            GetComponent<CanvasGroup>().blocksRaycasts = false;

            //inventory.items[this.slotID] = new Item();

            //Debug.Log("inventory's items list at index " + slotID + " now has empty item");

            if (item.Stackable && amount > 1)
            {
                Debug.Log("dragged item's amount is more than 1");

                //inventory.items[slotID] = item;

                //Debug.Log("inventory's items list updated at index " + slotID + " with item " + item.Title);

                GameObject itemInSlot = Instantiate(inventory.inventoryItem);
                itemInSlot.transform.SetParent(inventory.slots[slotID].transform);
                itemInSlot.transform.position = itemInSlot.transform.parent.transform.position;
                itemInSlot.GetComponent<Image>().sprite = item.Sprite;
                itemInSlot.name = item.Title;

                ItemData dataOfItemInSlot = itemInSlot.GetComponent<ItemData>();
                dataOfItemInSlot.item = item;
                dataOfItemInSlot.amount = amount - 1;
                dataOfItemInSlot.transform.GetChild(0).GetComponent<Text>().text = dataOfItemInSlot.amount == 1 ? "" : dataOfItemInSlot.amount.ToString();
                dataOfItemInSlot.slotID = slotID;

                // update the amount of the dragged item
                this.amount = 1;
                this.transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            // Debug.Log("Currently dragging item " + item.Title + " with slotID: " + slotID + "\nActivating tooltip");

            this.transform.position = eventData.position;
            tooltip.Activate(item);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("End of dragging");

            //Debug.Log("this itemData's parent is " + this.transform.parent.ToString() + " with slotID " + this.transform.parent.GetComponent<Slot>().slotID);


            this.transform.SetParent(inventory.slots[slotID].transform);
            this.transform.position = inventory.slots[slotID].transform.position;

            //inventory.items[slotID] = item;

            //Debug.Log("inventory's items list at index " + slotID + " updated with " + item.Title);
            //Debug.Log("this itemData's parent is " + this.transform.parent.ToString() + " with slotID " + this.transform.parent.GetComponent<Slot>().slotID);
            Debug.Log("slot " + slotID + " has " + this.transform.parent.childCount + " children");

            if (this.transform.parent.childCount > 1)
            {
                Debug.Log("parent slot has more than one child");

                if (this.transform.parent.GetChild(0).GetComponent<ItemData>().item.ID ==
                    this.transform.parent.GetChild(1).GetComponent<ItemData>().item.ID)
                {
                    Debug.Log("destroy b/c same items");

                    ItemData dataOfItemInParentSlot = this.transform.parent.GetChild(0).GetComponent<ItemData>();
                    dataOfItemInParentSlot.amount++;
                    dataOfItemInParentSlot.transform.GetChild(0).GetComponent<Text>().text = dataOfItemInParentSlot.amount.ToString();

                    Destroy(eventData.pointerDrag);

                    Debug.Log("Dropped item destroyed");
                }
                else
                {
                    // to fix
                    Debug.Log("don't destroy, not same items");

                    for (int i = 0; i < inventory.items.Count; i++)
                    {
                        if (inventory.items[i].ID == -1)
                        {
                            Debug.Log("slot with id " + i + " is empty");

                            this.transform.SetParent(inventory.slots[i].transform);
                            this.transform.position = inventory.slots[i].transform.position;
                            //inventory.items[i] = this.item;
                            break;
                        }
                    }
                }

            }

            if (parentSlot.childCount > 1) // parentSlot is now the item's old parent, NOT the current
            {
                Debug.Log("old parent slot has more than one child");

                //Transform oldParentItemToKeep = parentSlot.GetChild(0);
                Transform oldParentItemToMove = parentSlot.GetChild(1);

                for (int i = 0; i < inventory.items.Count; i++)
                {
                    if (inventory.items[i].ID == -1)
                    {
                        Debug.Log("moving old item in slot " + parentSlot.GetComponent<Slot>().slotID + " to slot with index " + i);

                        oldParentItemToMove.SetParent(inventory.slots[i].transform);
                        oldParentItemToMove.position = inventory.slots[i].transform.position;
                        oldParentItemToMove.GetComponent<ItemData>().slotID = i;
                        //inventory.items[i] = oldParentItemToMove.GetComponent<ItemData>().item;

                        //inventory.items[oldParentItemToKeep.GetComponent<ItemData>().slotID] = oldParentItemToKeep.GetComponent<ItemData>().item;
                        break;
                    }
                }
            }

            GetComponent<CanvasGroup>().blocksRaycasts = true;

            inventory.UpdateInventoryItems();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }


}
