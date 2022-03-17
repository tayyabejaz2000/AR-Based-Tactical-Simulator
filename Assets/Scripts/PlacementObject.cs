using UnityEngine;

public class PlacementObject : MonoBehaviour
{
    void Start()
    {
        transform.parent = GameObject.FindGameObjectWithTag("ObjectsParent").transform;
    }
}
