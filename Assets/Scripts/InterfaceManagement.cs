using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManagement : MonoBehaviour
{
    int buttonKey = -1;

    public GameObject centerButton;
    public GameObject buttonOne;
    public GameObject buttonTwo;
    public GameObject buttonThree;
    public GameObject buttonBackdrop;

    public ARInteraction ARController;

    void Start()
    {
        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);
        buttonThree.SetActive(false);
        buttonBackdrop.SetActive(false);
    }

    public void CenterButtonDown()
    {
        buttonOne.SetActive(true);
        buttonTwo.SetActive(true);
        buttonThree.SetActive(true);
        buttonBackdrop.SetActive(true);
        buttonKey = -1;
    }

    public void CenterButtonUp()
    {
        if (buttonKey == 1)
        {
            Debug.Log("Key: 1");
            ARController.AddObject();
        }
        else if (buttonKey == 2)
        {
            Debug.Log("Key: 2");
            ARController.AddAlert();
        }
        else if (buttonKey == 3)
        {
            Debug.Log("Key: 3");
            ARController.RemoveObject();
        }


        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);
        buttonThree.SetActive(false);
        buttonBackdrop.SetActive(false);
        buttonKey = -1;
    }

    public void SetButtonKey(int value)
    {
        buttonKey = value;
    }
}
