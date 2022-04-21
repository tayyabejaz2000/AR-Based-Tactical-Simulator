using UnityEngine;
using Photon.Pun;
public class PlacementMine : MonoBehaviour
{
    public bool isPinged = false;
    public void SetPose(Vector3 position, Quaternion rotation)
    {
        GetComponent<PhotonView>().RPC("SetPoseRPC", RpcTarget.All, position, rotation);
    }
    [PunRPC]
    void SetPoseRPC(Vector3 position, Quaternion rotation)
    {
        transform.parent = GameObject.Find("ScenarioObjects").transform;
        transform.localPosition = position;
        transform.localRotation = rotation;
    }
}
