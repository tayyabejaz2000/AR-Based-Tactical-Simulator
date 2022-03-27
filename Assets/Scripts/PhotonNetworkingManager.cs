using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.ARFoundation;
public class PhotonNetworkingManager : MonoBehaviourPunCallbacks
{
    GameObject ARPlayerObject;
    void Awake()
    {
        PhotonNetwork.Instantiate("Prefabs/AR Camera", Vector3.zero, Quaternion.identity);
    }
}
