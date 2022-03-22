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

    [SerializeField]
    private GameObject spritesAnchor;
    [SerializeField]
    private GameObject UISpritePrefab1;
    [SerializeField]
    private GameObject UISpritePrefab2;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    private Vector2 crosshairPosition;

    private Camera ARCamera;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Logging Functionality
    [SerializeField]
    private TMPro.TextMeshProUGUI LogText;
    private Touch touch;

    /// <summary>
    /// Logging Utility for Mobile Devices
    /// Logs to a UI Text Element identified by <c>LogText</c>
    /// </summary>
    /// <param name="message">Message to Log</param>
    public void Log(string message)
    {
        LogText.text += $"{message}";
    }
    /// <summary>
    /// Logging Utility for Mobile Devices
    /// Logs to a UI Text Element with end line identified by <c>LogText</c>
    /// </summary>
    /// <param name="message">Message to Log</param>
    public void LogLn(string message)
    {
        LogText.text += $"{message}\n";
    }


    /// <summary>
    /// Adds a 3D Ping Object in scene, binded to UI Ping Button
    /// </summary>
    public void AddObject()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            //Get the first hit pose
            var hitPose = hits[0].pose;

            //Spawn a 3D Ping and on the hit pose in 3D
            Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);

            //Get the spawnedObject position relative to the scanned marker
            if (markerData.isStartingMarkerScanned)
            {
                var relativePosition = hitPose.position - markerData.startingPosition;
                var relativeRotation = hitPose.rotation * Quaternion.Inverse(markerData.startingRotation);
                //LogLn("Relative Position: " + relativePosition.ToString());
                //LogLn("Relative Rotation: " + relativeRotation.ToString());
                Debug.Log("Relative Position: " + relativePosition.ToString());
                Debug.Log("Relative Rotation: " + relativeRotation.ToString());
            }
            else
            {
                Debug.Log("Starting marker not scanned");
            }
            ///TODO: Broadcast the spawned ping to other players using Photon RPC
        }
    }

    /// <summary>
    /// Adds a UI Alert on the Canvas, binded to UI Alert Button
    /// Dynamically spawn different Alerts on basis of Crosshair position
    /// </summary>
    public void AddAlert()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            GameObject alertObject = null;
            var alertText = "";

            //World Position where the alert will spawn
            var worldPosition = hits[0].pose.position;

            var ray = ARCamera.ScreenPointToRay(crosshairPosition);
            var mask = 1 << 7;
            //Check Hit condition here
            if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, mask))
            {
                var hitObject = hitInfo.transform;
                //If the ray hit a physics object i.e. 3D Ping, update the spawn world position
                worldPosition = hitObject.position;

                //Instantiate a 2D alert
                alertObject = Instantiate(UISpritePrefab1, Vector3.zero, Quaternion.identity, spritesAnchor.transform);

                // If the hitObject was a Capture Flag, set the text for 2D alert as <c>placementFlag.flagName</c>
                // and mark the flag as pinged
                if (hitObject.TryGetComponent<PlacementFlag>(out var placementFlag))
                {
                    alertText = placementFlag.flagName;
                    placementFlag.isPinged = true;
                }

                //Calculating relative position to the starting marker
                var relativePosition = Vector3.zero;
                if (markerData.isStartingMarkerScanned)
                {
                    relativePosition = worldPosition - markerData.startingPosition;
                    //LogLn("UI Relative Position: " + relativePosition.ToString());
                    Debug.Log("UI Relative Position: " + relativePosition.ToString());
                }
                else
                {
                    Debug.Log("Starting marker not scanned");
                }
                ///TODO: write the position in RPC call
                //Maybe we have to RPC Call later in the if statement, in case we have to send objectName or identity as parameter
                //since there are different sprites
            }
            else
            {
                //If no Physics Object was hit, spawn the 2nd Alert
                alertObject = Instantiate(UISpritePrefab2, Vector3.zero, Quaternion.identity, spritesAnchor.transform);
            }

            if (alertObject.TryGetComponent<PlacementAlert>(out var placementAlert))
            {
                //Set the text for spawned alert
                placementAlert.alertText = alertText;
                //Set the 3D world position for the UI Sprite
                placementAlert.worldPosition = worldPosition;
            }
        }
    }
    /// <summary>
    /// Removes a 3D object from scene, binded to UI Remove Button
    /// </summary>
    public void RemoveObject()
    {
        var ray = ARCamera.ScreenPointToRay(crosshairPosition);
        var mask = 1 << 6;
        if (Physics.Raycast(ray, out var hitObject, float.MaxValue, mask))
            Destroy(hitObject.collider.gameObject);
    }

    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Start()
    {
        ARCamera = Camera.main;
        crosshairPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }
}
