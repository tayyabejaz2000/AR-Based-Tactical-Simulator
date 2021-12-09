using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAPI : MonoBehaviour
{
    public GameObject debugText;

    // Start is called before the first frame update
    void Start()
    {
        debugText.SetActive(false);
    }

    public void EnableText()
    {
        debugText.SetActive(true);
        Debug.Log("Enabling Button");
    }

    public void DisableText()
    {
        debugText.SetActive(false);
        Debug.Log("Disabling Button");
    }
}
