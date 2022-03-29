using UnityEngine;

public class PlacementObject : MonoBehaviour
{
    void Awake()
    {
        transform.parent = GameObject.Find("ScenarioObjects").transform;
    }
}
