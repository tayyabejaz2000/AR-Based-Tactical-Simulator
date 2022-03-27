using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARAnchorInteraction : MonoBehaviour
{
    public GameObject ARSessionOrigin;
    public ARTrackedImageManager imageManager;
    public GameObject flagPrefab;
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public bool isStartingMarkerScanned;

    //Log Printer
    uint qsize = 10;  // number of messages to keep
    Queue myLogQueue = new Queue();
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 12;
        GUILayout.BeginArea(new Rect(200, 0, 700, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }
    void OnStartImageTracked(GameObject img)
    {
        //TODO: Sync position and rotation using pose of tracked image
        //To avoid multiple scan and no scan
        if (isStartingMarkerScanned == false)
        {
            //We store the position and rotation of starting marker
            startingPosition = img.transform.position;
            startingRotation = img.transform.rotation;
            //Acknowleding that we have scanned the starting marker
            isStartingMarkerScanned = true;
        }
    }

    void OnFlagImageTracked(Transform parent)
    {
        var flagObject = Instantiate(flagPrefab, parent);
        //Make Flag Object stand upright
        flagObject.transform.rotation = Quaternion.Euler(0, parent.eulerAngles.y, 0);
    }

    /// <summary>
    /// Callback binded to <see cref="ARTrackedImageManager.trackedImagesChanged"/>
    /// Called when there is an update in ARTrackedImages
    /// </summary>
    /// <param name="eventArgs">struct containing information about ARTrackedImages</param>
    void OnImageTracked(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //TODO: Add in markers for each unique interaction
        foreach (var image in eventArgs.added)
        {
            switch (image.referenceImage.name)
            {
                case "Marker-1":
                    Debug.Log("Case: Marker 1");
                    //In Case of `Marker-1` tracked, instantiate a Flag Prefab
                    OnFlagImageTracked(image.transform);
                    break;

                    //case "Start-Marker":
                    //Debug.Log("Case: Start-Marker 1");
                    //In-case of Start-Marker, store it's position and rotation in real world
                    //OnStartImageTracked(image.gameObject);
                    //break;
            }
        }
    }

    void Start()
    {
        //Bind `OnImageTracked` function to event ARTrackedImageManager.trackedImagesChanged
        imageManager.trackedImagesChanged += OnImageTracked;
        isStartingMarkerScanned = false;
    }
}
