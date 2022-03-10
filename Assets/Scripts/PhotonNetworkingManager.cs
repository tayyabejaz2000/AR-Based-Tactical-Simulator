using UnityEngine;
using Photon.Pun;
public class PhotonNetworkingManager : MonoBehaviourPunCallbacks
{
    GameObject ARPlayerObject;
    void Awake()
    {
        PhotonNetwork.Instantiate("Prefabs/AR Camera", Vector3.zero, Quaternion.identity);
    }
}
