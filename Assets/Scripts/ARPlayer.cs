using UnityEngine;
using Photon.Pun;

public class ARPlayer : MonoBehaviour
{
    public Component[] localComponents;
    public Component[] otherComponents;

    private bool _isSet;
    public bool isMine
    {
        set
        {
            if (!_isSet)
            {
                _isSet = true;
                if (value)
                {
                    foreach (var component in otherComponents)
                        Destroy(component);
                }
                else
                {
                    foreach (var component in localComponents)
                        Destroy(component);
                }
            }
        }
    }

    void Start()
    {
        transform.parent = GameObject.Find("AR Session Origin").transform;
        isMine = GetComponent<PhotonView>().IsMine;
    }
}
