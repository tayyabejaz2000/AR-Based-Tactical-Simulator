using UnityEngine;
using Photon.Pun;
public class ARPlayerMarker : MonoBehaviour
{
    Vector3 localPosition;
    Camera ARCamera;
    Transform scenarioParent;
    public void SetPosition(Vector3 position)
    {
        GetComponent<PhotonView>().RPC("SetPositionRPC", RpcTarget.Others, position);
    }

    [PunRPC]
    void SetPositionRPC(Vector3 position)
    {
        localPosition = position;
    }

    void Start()
    {
        scenarioParent = GameObject.Find("ScenarioObjects").transform;
        transform.parent = scenarioParent;
        ARCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.position = ARCamera.WorldToScreenPoint(scenarioParent.transform.TransformPoint(localPosition));

        var player = Camera.main.gameObject;
        var distance = (transform.position - player.transform.position).magnitude;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = distance.ToString("0.0") + "m";
    }
}
