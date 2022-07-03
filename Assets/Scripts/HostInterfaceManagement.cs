using System.IO;

using UnityEngine;

enum ButtonStateHost
{
    None,
    //Add a 3D ping in scene
    AddFlag,
    //Add a 2D Alert
    AddBomb,
    //Remove a 3D Ping if crosshair is on the ping
    RemoveObjective,
}

public class HostInterfaceManagement : MonoBehaviour
{
    ButtonStateHost buttonKey = ButtonStateHost.None;

    //Button GameObjects
    public GameObject centerButton;
    public GameObject buttonBomb;
    public GameObject buttonFlag;
    public GameObject buttonDestroy;
    public GameObject buttonBackdrop;
    public GameObject scenarioNameObject;

    public ARInteraction ARController;

    void Start()
    {
        if (!Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("HostButtons").SetActive(false);
            enabled = false;
        }


        buttonBomb.SetActive(false);
        buttonFlag.SetActive(false);
        buttonDestroy.SetActive(false);
        buttonBackdrop.SetActive(false);
    }

    public void CenterButtonDown()
    {
        buttonBomb.SetActive(true);
        buttonFlag.SetActive(true);
        buttonDestroy.SetActive(true);
        buttonBackdrop.SetActive(true);
        buttonKey = ButtonStateHost.None;
        Debug.Log("Button Clicked");
    }

    public void CenterButtonUp()
    {
        switch (buttonKey)
        {
            case ButtonStateHost.AddBomb:
                Debug.Log("Add Bomb Pushed");
                ARController.AddMine();
                break;
            case ButtonStateHost.AddFlag:
                Debug.Log("Add Flag Pushed");
                ARController.AddTargetFlag();
                break;
            case ButtonStateHost.RemoveObjective:
                Debug.Log("Remove Object Pushed");
                ARController.RemoveHostObjects();
                break;
        }

        buttonBomb.SetActive(false);
        buttonFlag.SetActive(false);
        buttonDestroy.SetActive(false);
        buttonBackdrop.SetActive(false);
        buttonKey = ButtonStateHost.None;
    }

    public void SetButtonKey(int value)
    {
        Debug.Log("Value is : " + value.ToString());
        switch (value)
        {
            case 1:
                buttonKey = ButtonStateHost.AddFlag;
                break;
            case 2:
                buttonKey = ButtonStateHost.AddBomb;
                break;
            case 3:
                buttonKey = ButtonStateHost.RemoveObjective;
                break;
        }
    }

    public void OpenSaveScenarioInput()
    {
        scenarioNameObject.SetActive(true);
    }

    public void SaveScene()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        var scenarioName = scenarioNameObject.GetComponent<TMPro.TMP_InputField>().text;
        ARController.SaveScenarioObjects(scenarioName);
        scenarioNameObject.SetActive(false);
    }
    public void LoadScene()
    {
        //TODO: Take Scenario Name as input from host
        var scenarioName = "temp";
        ARController.LoadScenarioObjects(scenarioName);
    }
}
