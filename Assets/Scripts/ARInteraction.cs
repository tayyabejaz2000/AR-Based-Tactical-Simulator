using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

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
    public string flagObjectPath = "Prefabs/Flag_Prefab";
    public string minePrefabPath = "Prefabs/Mine_Prefab";
    public Score score;
    private ARRaycastManager _arRaycastManager;
    private Vector2 crosshairPosition;

    private Camera ARCamera;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    //Session Origin Setter
    List<Vector3> markerPointPosition;
    List<Quaternion> markerPointRotation;
    Vector3 positionCentroid;
    Quaternion rotationCentroid;

    List<GameObject> hostObjects = new List<GameObject>();



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

            var position = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(hitPose.position);
            //Spawn a 3D Ping and on the hit pose in 3D
            var pingObject = PhotonNetwork.Instantiate(objectPrefabPath, position, hitPose.rotation);
            pingObject.GetComponent<PlacementObject>().SetPose(position, hitPose.rotation);
        }
    }

    /// <summary>
    /// Adds a UI Alert on the Canvas, binded to UI Alert Button
    /// Dynamically spawn different Alerts on basis of Crosshair position
    /// </summary>
    public void AddAlert()
    {
        GameObject alertObject = null;
        var alertText = "";
        bool deleteOnTimeout = false;

        Vector3 localPosition = Vector3.zero;

        var ray = ARCamera.ScreenPointToRay(crosshairPosition);
        int mask = 1 << 7;
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, mask))
        {
            var hitObject = hitInfo.transform;
            localPosition = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(hitObject.position);
            if (hitObject.TryGetComponent<PlacementFlag>(out var placementFlag) && placementFlag.isPinged == false)
            {
                alertText = placementFlag.flagName;
                placementFlag.isPinged = true;
                //Instantiate a 2D alert
                alertObject = PhotonNetwork.Instantiate(UISpritePrefabPath1, Vector3.zero, Quaternion.identity);
                //Updating the score
                score.UpdateScore();
                placementFlag.SetScanned();
                placementFlag.myAlert = alertObject.GetComponent<PlacementAlert>();
            }
            else if (hitObject.TryGetComponent<PlacementMine>(out var placementMine) && placementMine.isPinged == false)
            {
                alertText = "Mine";
                placementMine.isPinged = true;
                //Instantiate a 2D alert
                alertObject = PhotonNetwork.Instantiate(UISpritePrefabPath1, Vector3.zero, Quaternion.identity);
                placementMine.myAlert = alertObject.GetComponent<PlacementAlert>();
            }
        }
        else if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            localPosition = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(hits[0].pose.position);
            alertObject = PhotonNetwork.Instantiate(UISpritePrefabPath2, Vector3.zero, Quaternion.identity);
            deleteOnTimeout = true;
        }

        if (alertObject != null && alertObject.TryGetComponent<PlacementAlert>(out var placementAlert))
        {
            //Set the text for spawned alert and the 3D world position for the UI Sprite
            placementAlert.SetData(alertText, localPosition, deleteOnTimeout);
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
        {
            if (hitObject.collider.TryGetComponent<PhotonView>(out var photonView))
            {
                PhotonNetwork.Destroy(photonView);
            }
        }
    }

    public void RemoveHostObjects()
    {
        var ray = ARCamera.ScreenPointToRay(crosshairPosition);
        int mask = 1 << 7;
        if (Physics.Raycast(ray, out var hitObject, float.MaxValue, mask))
        {
            if (hitObject.collider.TryGetComponent<PhotonView>(out var photonView))
            {
                if (hitObject.collider.gameObject.TryGetComponent<PlacementFlag>(out var flag))
                {
                    if (flag.myAlert != null)
                    {
                        PhotonNetwork.Destroy(flag.myAlert.GetComponent<PhotonView>());
                    }
                }
                else if (hitObject.collider.gameObject.TryGetComponent<PlacementMine>(out var mine))
                {
                    if (mine.myAlert != null)
                    {
                        PhotonNetwork.Destroy(mine.myAlert.GetComponent<PhotonView>());
                    }
                }
                hostObjects.Remove(hitObject.collider.gameObject);
                PhotonNetwork.Destroy(photonView);
            }
        }
    }

    public void AddTargetFlag()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            //Get the first hit pose
            var hitPose = hits[0].pose;

            var position = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(hitPose.position);

            //Spawn a 3D Ping and on the hit pose in 3D
            var flagObject = PhotonNetwork.Instantiate(flagObjectPath, position, hitPose.rotation);
            flagObject.GetComponent<PlacementFlag>().SetPose(position, Quaternion.identity);
            hostObjects.Add(flagObject);
        }
    }

    public void AddMine()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            //Get the first hit pose
            var hitPose = hits[0].pose;

            var position = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(hitPose.position);

            //Spawn a 3D Ping and on the hit pose in 3D
            var mineObject = PhotonNetwork.Instantiate(minePrefabPath, position, hitPose.rotation);
            mineObject.GetComponent<PlacementMine>().SetPose(position, Quaternion.identity);
            hostObjects.Add(mineObject);
        }
    }

    public void SyncScenarioObjects()
    {
        if (_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon) && hits.Count > 0)
        {
            var hitPose = hits[0].pose;
            var scenarioObjects = GameObject.Find("ScenarioObjects");
            scenarioObjects.transform.position = hitPose.position;
            scenarioObjects.transform.eulerAngles = Vector3.up * (-Input.compass.trueHeading);
        }
    }

    public void SaveScenarioObjects(string scenarioName)
    {
        var dumpData = hostObjects.Select(o => (o.name.ToLower().Contains("flag"), o.transform.position, o.transform.rotation)).ToList();
        var jsonData = JsonConvert.SerializeObject(dumpData);
        Debug.Log(jsonData);
        File.WriteAllText(Application.persistentDataPath + "/Saves/" + scenarioName, jsonData);
    }

    public void LoadScenarioObjects(string ScenarioName)
    {
        var stringData = File.ReadAllText(Application.persistentDataPath + "/Saves/" + ScenarioName);
        Debug.Log(stringData);
        var data = JsonConvert.DeserializeObject<List<(bool isFlag, Vector3 position, Quaternion rotation)>>(stringData);
        hostObjects.Clear();
        foreach (var obj in data)
        {
            if (obj.isFlag)
            {
                var flagObject = PhotonNetwork.Instantiate(flagObjectPath, obj.position, obj.rotation);
                flagObject.GetComponent<PlacementFlag>().SetPose(obj.position, Quaternion.identity);
                hostObjects.Add(flagObject);
            }
            else
            {
                var mineObject = PhotonNetwork.Instantiate(minePrefabPath, obj.position, obj.rotation);
                mineObject.GetComponent<PlacementMine>().SetPose(obj.position, Quaternion.identity);
                hostObjects.Add(mineObject);
            }
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

    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Start()
    {
        ARCamera = FindObjectOfType<ARCameraManager>().GetComponent<Camera>();
        //score = FindObjectOfType<Score>().GetComponent<Score - Value>;
        //ARCamera = GameObject.Find("AR Camera(Clone)").GetComponent<Camera>();
        //ARCamera = Camera.main
        if (ARCamera == null)
        {
            Application.Quit(0);
        }

        crosshairPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        markerPointPosition = new List<Vector3>();
        markerPointRotation = new List<Quaternion>();
    }
}
