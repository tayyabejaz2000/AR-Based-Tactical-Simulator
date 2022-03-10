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
}
