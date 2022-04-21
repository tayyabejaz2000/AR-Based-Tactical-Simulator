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

    void Awake()
    {
        transform.parent = GameObject.Find("Sprites").transform;
    }

    void Start()
    {
        scenarioParent = GameObject.Find("ScenarioObjects").transform;
        //transform.parent = scenarioParent;
        ARCamera = Camera.main;

        //PhotonNetwork.Instantiate("Prefabs/PlayerMarker", localPosition, Quaternion.identity);
    }

    void LateUpdate()
    {
        transform.position = ARCamera.WorldToScreenPoint(scenarioParent.TransformPoint(localPosition));

        var player = Camera.main.gameObject;
        var distance = (transform.position - player.transform.position).sqrMagnitude;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = distance.ToString("0.0") + "m";
    }
}
