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
    Vector3 worldPosition;


    public void SetData(string alertText, Vector3 worldPosition)
    {
        GetComponent<PhotonView>().RPC("SetDataRPC", RpcTarget.All, alertText, worldPosition);
    }

    void Awake()
    {
        transform.parent = GameObject.Find("Sprites").transform;
    }

    void Start()
    {
        ARCamera = Camera.main;

        distance = (worldPosition - ARCamera.transform.position).sqrMagnitude;
    }

    void LateUpdate()
    {
        distance = (worldPosition - ARCamera.transform.position).sqrMagnitude;
        transform.position = ARCamera.WorldToScreenPoint(worldPosition);
        ttl -= Time.deltaTime;
        if (ttl <= 0.0f)
            Destroy(gameObject);
    }

    void SetDataRPC(string text, Vector3 worldPos)
    {
        worldPosition = worldPos;
        alertText = text;
    }
}
