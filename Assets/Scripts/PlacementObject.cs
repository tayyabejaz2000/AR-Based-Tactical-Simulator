using UnityEngine;
using Photon.Pun;

public class PlacementObject : MonoBehaviour
{
    public void SetPose(Vector3 position, Quaternion rotation)
    {
        GetComponent<PhotonView>().RPC("SetPoseRPC", RpcTarget.All, position, rotation);
    }

    [PunRPC]
    void SetPoseRPC(Vector3 position, Quaternion rotation)
    {
        transform.parent = GameObject.Find("ScenarioObjects").transform;
        transform.localPosition = position;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180 - Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

}
