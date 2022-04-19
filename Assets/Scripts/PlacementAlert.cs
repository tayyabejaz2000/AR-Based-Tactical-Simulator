using TMPro;

using UnityEngine;

using Photon.Pun;

public class PlacementAlert : MonoBehaviour
{
    public float ttl = 5.0f;

    [SerializeField]
    TextMeshProUGUI targetText;

    [SerializeField]
    TextMeshProUGUI nameText;

    PhotonView photonView;

    float _distanceFromCamera;
    float distance
    {
        get { return _distanceFromCamera; }
        set
        {
            _distanceFromCamera = value;
            if (targetText != null)
                targetText.text = value.ToString("0.0") + "m";
        }
    }
    string _alertText;
    string alertText
    {
        get { return _alertText; }
        set
        {
            _alertText = value;
            if (nameText != null)
                nameText.text = value;
        }
    }

    Camera ARCamera;
    Vector3 localPosition;
    GameObject scenarioParent;


    public void SetData(string alertText, Vector3 position)
    {
        GetComponent<PhotonView>().RPC("SetDataRPC", RpcTarget.All, alertText, position);
    }

    void Awake()
    {
        transform.parent = GameObject.Find("Sprites").transform;
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        ARCamera = Camera.main;

        scenarioParent = GameObject.Find("ScenarioObjects");

        distance = (scenarioParent.transform.TransformPoint(localPosition) - ARCamera.transform.position).sqrMagnitude;
    }

    void LateUpdate()
    {
        distance = (scenarioParent.transform.TransformPoint(localPosition) - ARCamera.transform.position).sqrMagnitude;
        transform.position = ARCamera.WorldToScreenPoint(scenarioParent.transform.TransformPoint(localPosition));
        if (photonView.IsMine)
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0.0f)
                PhotonNetwork.Destroy(photonView);
        }
    }

    [PunRPC]
    void SetDataRPC(string text, Vector3 localPos)
    {
        localPosition = localPos;
        alertText = text;
    }
}
