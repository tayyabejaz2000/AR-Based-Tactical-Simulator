using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARInteraction : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;
    public GameObject alertToInstantiate1;
    public GameObject alertToInstantiate2;
    public ARAnchorInteraction markerData;

    private List<GameObject> spawnedObject;
    private List<Vector3> spawnedSprites;
    private List<GameObject> UISprites;
    [SerializeField]
    private GameObject spritesAnchor;
    [SerializeField]
    private GameObject UISpritePrefab1;
    [SerializeField]
    private GameObject UISpritePrefab2;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    private Vector2 crosshairPosition;

    [SerializeField]
    private Camera ARCamera;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Logging Functionality
    [SerializeField]
    private TMPro.TextMeshProUGUI LogText;
    private Touch touch;

    public void Log(string message)
    {
        LogText.text += $"{message}";
    }
    public void LogLn(string message)
    {
        LogText.text += $"{message}\n";
    }

    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Start()
    {
        crosshairPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        spawnedObject = new List<GameObject>();
        spawnedSprites = new List<Vector3>();
        UISprites = new List<GameObject>();
    }


    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void Update()
    {
        for (int i = 0; i < spawnedSprites.Count; i++)
        {
            var position = spawnedSprites[i];
            var placementAlert = UISprites[i].GetComponent<PlacementAlert>();
            if (placementAlert != null)
            {
                placementAlert.setDistance(calculateDistance(position, ARCamera.transform.position));
            }

            //Updating UI Sprite Position
            UISprites[i].transform.position = ARCamera.WorldToScreenPoint(position);
        }
    }

    public void AddObject()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            spawnedObject.Add(Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation));
            //Get the spawnedObject position relative to the scanned marker
            if (markerData.isStartingMarkerScanned)
            {
                var relativePosition = hitPose.position - markerData.startingPosition;
                var relativeRotation = hitPose.rotation * Quaternion.Inverse(markerData.startingRotation);
                LogLn("Relative Position: " + relativePosition.ToString());
                LogLn("Relative Rotation: " + relativeRotation.ToString());
            }

            ///TODO:Write it to other user using RPC call
            spawnedObject.Last().transform.position = hitPose.position;
            spawnedObject.Last().transform.rotation = hitPose.rotation;

            if (spawnedObject.Last().TryGetComponent<PlacementObject>(out var placementObject))
            {
                placementObject.index = spawnedObject.Count - 1;
            }
            else
            {
                LogLn("-------[AddObject]: " + "Placement Object is NULL");
            }
        }
    }

    public void AddAlert()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            //Setting up UI Position of the 3D Placement
            var UIPosition = ARCamera.WorldToScreenPoint(hitPose.position);

            LogLn("UI Position: " + UIPosition.ToString());

            //Check Hit condition here
            var ray = ARCamera.ScreenPointToRay(crosshairPosition);
            var mask = 1 << 7;
            if (Physics.Raycast(ray, out var hitObject, float.MaxValue, mask))
            {
                var position = hitObject.transform.position;

                //Updating UI Position
                UIPosition = ARCamera.WorldToScreenPoint(position);
                if (hitObject.transform.TryGetComponent<PlacementFlag>(out var placementFlag) && placementFlag.isPinged)
                {
                    spawnedSprites.Add(position);
                    UISprites.Add(Instantiate(UISpritePrefab1, UIPosition, Quaternion.identity));
                    if (UISprites.Last().TryGetComponent<PlacementAlert>(out var placementAlert))
                        placementAlert.setName(placementFlag.flagName);
                    else
                        LogLn("[AddAlert] Placement Alert is NULL");
                    placementFlag.isPinged = true;
                }
                else
                {
                    spawnedSprites.Add(position);//Instantiate(alertToInstantiate2, hitPose.position, hitPose.rotation));
                    UISprites.Add(Instantiate(UISpritePrefab2, UIPosition, Quaternion.identity));
                }

                //Calculating relative position to the starting marker
                var relativePosition = Vector3.zero;
                if (markerData.isStartingMarkerScanned)
                {
                    relativePosition = hitPose.position - markerData.startingPosition;
                    LogLn("UI Relative Position: " + relativePosition.ToString());
                }
                ///TODO: write the position in RPC call

                //Maybe we have to RPC Call later in the if statement, in case we have to send objectName or identity as parameter
                //since there are different sprites
            }
            else
            {
                spawnedSprites.Add(hitPose.position);
                UISprites.Add(Instantiate(UISpritePrefab2, UIPosition, Quaternion.identity));
                LogLn("Alert should have been placed! " + UIPosition.ToString() + " ");
            }

            UISprites.Last().transform.SetParent(spritesAnchor.transform, false);
            UISprites.Last().transform.position = UIPosition;
            LogLn("Parent have been set... UISprites Count: " + UISprites.Count.ToString());
        }
    }

    public void RemoveObject()
    {
        var ray = ARCamera.ScreenPointToRay(crosshairPosition);
        var mask = 1 << 6;
        if (Physics.Raycast(ray, out var hitObject, float.MaxValue, mask))
            Destroy(hitObject.collider.gameObject);
    }


    //Some Helper functions
    float calculateDistance(Vector3 pointOne, Vector3 pointTwo)
    {
        return (pointOne - pointTwo).magnitude;
    }
}
