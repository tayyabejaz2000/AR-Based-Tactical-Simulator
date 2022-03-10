using UnityEngine;
using TMPro;

public class PlacementAlert : MonoBehaviour
{
    public float ttl = 2.5f;

    [SerializeField]
    TextMeshProUGUI targetText;

    [SerializeField]
    TextMeshProUGUI nameText;

    private float _distanceFromCamera;
    public float distance
    {
        get { return _distanceFromCamera; }
        set
        {
            _distanceFromCamera = value;
            if (targetText != null)
                targetText.text = value.ToString("0.0") + "m";
        }
    }
    private string _alertText;
    public string alertText
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
    public Vector3 worldPosition;

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
}
