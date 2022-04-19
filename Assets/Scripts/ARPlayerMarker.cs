using UnityEngine;
using Photon.Pun;
public class ARPlayerMarker : MonoBehaviour
{
    public void SetPosition(Vector3 position)
    {
        GetComponent<PhotonView>().RPC("SetPositionRPC", RpcTarget.All, position);
    }


    [PunRPC]
    void SetPositionRPC(Vector3 position)
    {
        transform.position = GameObject.Find("ScenarioObjects").transform.TransformPoint(position);
    }
}
