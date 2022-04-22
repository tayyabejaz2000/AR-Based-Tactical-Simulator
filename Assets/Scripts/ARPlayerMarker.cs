using UnityEngine;
using Photon.Pun;
public class ARPlayerMarker : MonoBehaviour
{
    Vector3 localPosition;
    Camera ARCamera;
    Transform scenarioParent;
    public TMPro.TextMeshProUGUI playerName;
    public TMPro.TextMeshProUGUI distanceText;
    public void SetData(Vector3 position, string name)
    {
        GetComponent<PhotonView>().RPC("SetDataRPC", RpcTarget.Others, position, name);
    }

    [PunRPC]
    void SetDataRPC(Vector3 position, string name)
    {
        localPosition = position;
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    void Awake()
    {
        transform.parent = GameObject.Find("Sprites").transform;
    }

    void Start()
    {
        scenarioParent = GameObject.Find("ScenarioObjects").transform;
        ARCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.position = ARCamera.WorldToScreenPoint(scenarioParent.TransformPoint(localPosition));
        var player = Camera.main.gameObject;
        var distance = (scenarioParent.TransformPoint(localPosition) - player.transform.position).sqrMagnitude;
        distanceText.text = distance.ToString("0.0") + "m";
    }
}
