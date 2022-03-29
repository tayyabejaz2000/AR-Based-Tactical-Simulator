using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class ARInteraction : MonoBehaviour
{
    public string objectPrefabPath = "Prefabs/Ping_Prefab";


    public string UISpritePrefabPath1 = "Prefabs/Alert-1";
    public string UISpritePrefabPath2 = "Prefabs/Alert-2";
    private ARRaycastManager _arRaycastManager;
    private Vector2 crosshairPosition;

    private Camera ARCamera;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Session Origin Setter
    List<Vector3> markerPointPosition;
    List<Quaternion> markerPointRotation;
    Vector3 positionCentroid;
    Quaternion rotationCentroid;

    //Sample Scenario List
    List<Vector3> objectiveWorldPosition;
    List<Quaternion> objectiveWorldRotation;


    /// <summary>
    /// Adds a 3D Ping Object in scene, binded to UI Ping Button
    /// </summary>
    public void AddObject()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            //Get the first hit pose
            var hitPose = hits[0].pose;

            //Just for hard coding the sample scenario
            Debug.Log("HitPose Position: " + hitPose.position);
            Debug.Log("HitPose Rotation: " + hitPose.rotation);

            //Spawn a 3D Ping and on the hit pose in 3D
            PhotonNetwork.Instantiate(objectPrefabPath, hitPose.position, hitPose.rotation);
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

                // If the hitObject was a Capture Flag, set the text for 2D alert as <c>placementFlag.flagName</c>
                // and mark the flag as pinged
                if (hitObject.TryGetComponent<PlacementFlag>(out var placementFlag) && placementFlag.isPinged == false)
                {
                    alertText = placementFlag.flagName;
                    placementFlag.isPinged = true;
                    //Instantiate a 2D alert
                    alertObject = PhotonNetwork.Instantiate(UISpritePrefabPath1, Vector3.zero, Quaternion.identity);
                }
            }
            else
            {
                //If no Physics Object was hit, spawn the 2nd Alert
                alertObject = PhotonNetwork.Instantiate(UISpritePrefabPath2, Vector3.zero, Quaternion.identity);
            }

            if (alertObject.TryGetComponent<PlacementAlert>(out var placementAlert))
            {
                //Set the text for spawned alert and the 3D world position for the UI Sprite
                placementAlert.SetData(alertText, worldPosition);
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

    public void SyncScenarioObjects()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            var hitPose = hits[0].pose;
            var scenarioObjects = GameObject.Find("ScenarioObjects");
            scenarioObjects.transform.position = hitPose.position;
            scenarioObjects.transform.eulerAngles = Vector3.Scale(hitPose.rotation.eulerAngles, Vector3.up);

            Camera.main.transform.parent.position = hitPose.position;
            Camera.main.transform.parent.eulerAngles = Vector3.Scale(hitPose.rotation.eulerAngles, Vector3.up);
        }
    }

    /// <summary>
    /// Stores the position and rotation of scanned marker
    /// </summary>
    /// <returns> Returns the total number of scanned marker</returns>
    public int GetMarkerPoint()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            //Get the first hit pose
            var hitPose = hits[0].pose;

            markerPointPosition.Add(hitPose.position);
            markerPointRotation.Add(hitPose.rotation);
            Debug.Log("Position marker : " + hitPose.position.ToString());
            Debug.Log("Position rotation : " + hitPose.rotation.ToString());
        }

        Debug.Log("Marker Position Count: " + markerPointPosition.Count.ToString());
        if (markerPointPosition.Count == 4)
        {
            Debug.Log("Calculating Position centroid");
            positionCentroid = CalculatePositionCentroid();
            rotationCentroid = InterpolateRotationCentroid();
            Debug.Log("Centroid Position : " + positionCentroid.ToString());
            Debug.Log("Centroid Rotation : " + rotationCentroid.ToString());
        }

        return markerPointPosition.Count;
    }
    /// <summary>
    /// Calculates the mid positon by taking 4 points as input
    /// </summary>
    /// <returns> Returns the mid point of 4 points </returns>
    Vector3 CalculatePositionCentroid()
    {
        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < markerPointPosition.Count; i++)
        {
            centroid += markerPointPosition[i];
        }
        centroid /= markerPointPosition.Count;
        return centroid;
    }
    /// <summary>
    /// Interpolate the rotation of 4 points
    /// </summary>
    /// <returns>Returns the centroid interpolated rotation of 4 points</returns>
    Quaternion InterpolateRotationCentroid()
    {
        //Interpolate to mid of top-left and top-right
        Quaternion firstMidPoint = Quaternion.Lerp(markerPointRotation[0], markerPointRotation[1], 0.5f);
        //Interpolate to mid of bottom-left and bottom-right
        Quaternion secondMidPoint = Quaternion.Lerp(markerPointRotation[2], markerPointRotation[3], 0.5f);
        //Interpolate to centroid by interpolating to mid of top mid-point and bottom mid-point
        return Quaternion.Lerp(firstMidPoint, secondMidPoint, 0.5f);
    }

    public void AddObjectiveToScenario()
    {
        //Add the objectives to scenario
        for (int i = 0; i < objectiveWorldPosition.Count; i++)
        {
            var flag = PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity);
            flag.GetComponent<PlacementFlag>().SetPose(objectiveWorldPosition[i], objectiveWorldRotation[i]);
        }
    }
    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Start()
    {
        ARCamera = FindObjectOfType<ARCameraManager>().GetComponent<Camera>();
        //ARCamera = GameObject.Find("AR Camera(Clone)").GetComponent<Camera>();
        //ARCamera = Camera.main
        if (ARCamera == null)
        {
            Application.Quit(0);
        }

        crosshairPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        markerPointPosition = new List<Vector3>();
        markerPointRotation = new List<Quaternion>();

        objectiveWorldPosition = new List<Vector3>();
        objectiveWorldRotation = new List<Quaternion>();
        //Append sample scenario objective here
        objectiveWorldPosition.Add(new Vector3(-0.1f, -2.0f, 0.6f));
        objectiveWorldRotation.Add(new Quaternion(0.0f, -0.1f, 0.0f, 1.0f));

        objectiveWorldPosition.Add(new Vector3(-0.4f, -2.0f, 0.4f));
        objectiveWorldRotation.Add(new Quaternion(0.0f, -0.3f, 0.0f, 0.9f));

        objectiveWorldPosition.Add(new Vector3(0.3f, -2.0f, 0.6f));
        objectiveWorldRotation.Add(new Quaternion(0.0f, 0.1f, 0.0f, 1.0f));

        objectiveWorldPosition.Add(new Vector3(-0.2f, -2.0f, 1.1f));
        objectiveWorldRotation.Add(new Quaternion(0.0f, -0.1f, 0.0f, 1.0f));

        objectiveWorldPosition.Add(new Vector3(1.6f, -1.9f, -0.1f));
        objectiveWorldRotation.Add(new Quaternion(0.0f, 0.6f, 0.0f, 0.8f));


    }
}
