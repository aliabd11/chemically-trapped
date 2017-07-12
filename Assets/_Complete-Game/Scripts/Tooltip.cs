// based on this tutorial: https://www.youtube.com/playlist?list=PLivfKP2ufIK78r7nzfpIEH89Nlnb__RRG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private Item item;
    private string data;
    private GameObject tooltip;

    private void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);

    }

    private void Update()
    {
        if (tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    public void Activate(Item item)
    {
        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        data = "<color=#3C1392FF><b>" + item.Title + "</b></color>\n\n" + item.Description;
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
