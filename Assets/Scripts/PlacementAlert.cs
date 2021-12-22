using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementAlert : MonoBehaviour
{
    float distance;

    [SerializeField]
    TextMesh targetText;

    [SerializeField]
    TextMesh nameText;

    public void setDistance(float dist)
    {
        distance = dist;

        if(targetText != null)
            targetText.text = distance.ToString("0.0")+"m";
    }
    public float getDistance() {return distance;}

    public void setName(string name)
    {
        if(nameText != null)
            nameText.text = name;
    }
}
