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

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

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

    void Start()
    {
        crosshairPosition = new Vector2(1920 / 2f, 1080 / 2f);

        spawnedObject = new List<GameObject>();
        spawnedSprites = new List<Vector3>();
        UISprites = new List<GameObject>();
    }

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
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
            //spawnedSprites[i].transform.LookAt(ARCamera.transform);
            var position = spawnedSprites[i];//.transform.position;
            var placementAlert = UISprites[i].GetComponent<PlacementAlert>();
            if (placementAlert != null)
            {
                placementAlert.setDistance(calculateDistance(position, ARCamera.transform.position));
                //Log("[ALERT " + i.ToString() + "]" + ARCamera.WorldToScreenPoint(position).ToString());
            }

            //Updating UI Sprite Position
            UISprites[i].transform.position = ARCamera.WorldToScreenPoint(position);
        }

        /*if ( Input.touchCount > 0 )
        {
            touch = Input.GetTouch(0);
            //Only a click, if it was slided then consider it cancelled
            if ( touch.phase == TouchPhase.Stationary)
            {
                var worldC_position = ARCamera.ScreenToWorldPoint(touch.position);
                //What do we have to compare aganist??
                //Fix these lines
                //var relativePosition = worldC_position - transform.position;
                //Print it on the screen in the GUI
                //For rotation, we need object not a plane
            }
        }*/
    }

    void LateUpdate()
    {
    }

    public void AddObject()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            //spawnedObject.Add(PhotonNetwork.Instantiate(gameObjectToInstantiate.name, hitPose.position,hitPose.rotation) );
            spawnedObject.Add(Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation));
            //Get the spawnedObject position relative to the scanned marker
            if (markerData.isStartingMarkerScanned)
            {
                var relativePosition = hitPose.position - markerData.startingPosition;
                var relativeRotation = hitPose.rotation * Quaternion.Inverse(markerData.startingRotation);
                Log("Relative Position: " + relativePosition.ToString());
                Log("Relative Rotation: " + relativeRotation.ToString());
            }
            //
            //TODO:Write it to other user using RPC call
            //
            spawnedObject[spawnedObject.Count - 1].transform.position = hitPose.position;
            spawnedObject[spawnedObject.Count - 1].transform.rotation = hitPose.rotation;

            PlacementObject placementObject = spawnedObject[spawnedObject.Count - 1].GetComponent<PlacementObject>();
            if (placementObject != null)
            {
                placementObject.index = spawnedObject.Count - 1;
            }
            else
            {
                Debug.Log("-------[AddObject]: " + "Placement Object is NULL");
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

            Log("UI Position: " + UIPosition.ToString());

            //Check Hit condition here
            Ray ray = ARCamera.ScreenPointToRay(crosshairPosition);
            RaycastHit hitObject;
            int mask = 1 << 7;
            if (Physics.Raycast(ray, out hitObject, float.MaxValue, mask))
            {
                var position = hitObject.transform.position;
                var placementFlag = hitObject.transform.GetComponent<PlacementFlag>();

                //Calculating relative position to the starting marker
                Vector3 relativePosition = Vector3.zero;
                if (markerData.isStartingMarkerScanned)
                {
                    relativePosition = hitPose.position - markerData.startingPosition;
                    Log("UI Relative Position: " + relativePosition.ToString());
                }
                //
                //TODO: write the position in RPC call
                //
                //Maybe we have to RPC Call later in the if statement, in case we have to send objectName or identity as parameter
                //since there are different sprites

                //Updating UI Position
                UIPosition = ARCamera.WorldToScreenPoint(position);
                if (placementFlag != null && placementFlag.isPinged == false)
                {
                    //Log("[AddAlert] Position: "+position.ToString());
                    //Log("[AddAlert] Original: "+hitPose.position.ToString());
                    spawnedSprites.Add(position);//Instantiate(alertToInstantiate1, position, hitPose.rotation));
                    UISprites.Add(Instantiate(UISpritePrefab1, UIPosition, Quaternion.identity));
                    var placementAlert = UISprites[UISprites.Count - 1].GetComponent<PlacementAlert>();
                    placementFlag.isPinged = true;
                    if (placementAlert != null)
                        placementAlert.setName(placementFlag.flagName);
                    else
                        Log("[AddAlert] Placement Alert is NULL");
                }
                else
                {
                    spawnedSprites.Add(position);//Instantiate(alertToInstantiate2, hitPose.position, hitPose.rotation));
                    UISprites.Add(Instantiate(UISpritePrefab2, UIPosition, Quaternion.identity));
                }
            }
            else
            {
                spawnedSprites.Add(hitPose.position);//Instantiate(alertToInstantiate2, hitPose.position, hitPose.rotation));
                UISprites.Add(Instantiate(UISpritePrefab2, UIPosition, Quaternion.identity));
                Log("Alert should have been placed! " + UIPosition.ToString() + " ");
            }


            //spawnedSprites[spawnedSprites.Count - 1].transform.position = hitPose.position;
            //spawnedSprites[spawnedSprites.Count - 1].transform.rotation = hitPose.rotation;
            UISprites[UISprites.Count - 1].transform.SetParent(spritesAnchor.transform, false);
            UISprites[UISprites.Count - 1].transform.position = UIPosition;
            Log("Parent have been set... UISprites Count: " + UISprites.Count.ToString());
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
