using UnityEngine;

/// <summary>
/// enum to identify current UI Button State
/// </summary>
enum ButtonState
{
    None,
    //Add a 3D ping in scene
    AddObject,
    //Add a 2D Alert
    AddAlert,
    //Remove a 3D Ping if crosshair is on the ping
    RemoveObject,
}


public class InterfaceManagement : MonoBehaviour
{
    ButtonState buttonKey = ButtonState.None;

    //Button GameObjects
    public GameObject centerButton;
    public GameObject buttonOne;
    public GameObject buttonTwo;
    public GameObject buttonThree;
    public GameObject buttonBackdrop;
    public GameObject originButton;

    //Text GameObject
    public GameObject topLeft;
    public GameObject topRight;
    public GameObject bottomLeft;
    public GameObject bottomRight;

    //Scan Button Variable
    int pointScanned;


    public ARInteraction ARController;


    void Start()
    {
        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);
        buttonThree.SetActive(false);
        buttonBackdrop.SetActive(false);

        topRight.SetActive(false);
        bottomLeft.SetActive(false);
        bottomRight.SetActive(false);

        pointScanned = 1;
    }

    public void CenterButtonDown()
    {
        buttonOne.SetActive(true);
        buttonTwo.SetActive(true);
        buttonThree.SetActive(true);
        buttonBackdrop.SetActive(true);
        buttonKey = ButtonState.None;
    }

    public void CenterButtonUp()
    {
        switch (buttonKey)
        {
            case ButtonState.AddObject:
                ARController.AddObject();
                break;
            case ButtonState.AddAlert:
                ARController.AddAlert();
                break;
            case ButtonState.RemoveObject:
                ARController.RemoveObject();
                break;
        }

        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);
        buttonThree.SetActive(false);
        buttonBackdrop.SetActive(false);
        buttonKey = ButtonState.None;
    }

    public void SetButtonKey(int value)
    {
        switch (value)
        {
            case 1:
                buttonKey = ButtonState.AddObject;
                break;
            case 2:
                buttonKey = ButtonState.AddAlert;
                break;
            case 3:
                buttonKey = ButtonState.RemoveObject;
                break;
        }
    }

    public void SyncOrigin()
    {
        Debug.Log("Syncing Origin");
        originButton.SetActive(false);
        ARController.SyncScenarioObjects();
        ARController.AddObjectiveToScenario();
        Debug.Log("Added Objectives");
    }

    public void PointScanner()
    {
        Debug.Log("Scanned Button Triggered");
        pointScanned = ARController.GetMarkerPoint();
        PointText();
    }
    void PointText()
    {
        if (pointScanned == 1)
        {
            topLeft.SetActive(false);
            topRight.SetActive(true);
        }
        else if (pointScanned == 2)
        {
            topRight.SetActive(false);
            bottomLeft.SetActive(true);
        }
        else if (pointScanned == 3)
        {
            bottomLeft.SetActive(false);
            bottomRight.SetActive(true);
        }
        else if (pointScanned == 4)
        {
            bottomRight.SetActive(false);
            originButton.SetActive(false);
        }
    }
}
