using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject inventoryObject;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseUI(inventoryObject);
        }
    }
    public void OpenCloseUI(GameObject ui)
    {
        if (ui.activeSelf == false)
        {
            GameManager.PauseGame();
            ui.SetActive(true);
        }
        else
        {
            GameManager.UnpauseGame();
            ui.SetActive(false);
        }
    }
}
