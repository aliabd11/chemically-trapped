using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour {

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.instance.Restart();
            }
        }
    }
}
