using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARAnchorInteraction : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject flagPrefab;
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public bool isStartingMarkerScanned;

    void OnStartImageTracked(GameObject parent)
    {
        //TODO: Sync position and rotation using pose of tracked image
        //To avoid multiple scan and no scan
        if (isStartingMarkerScanned == false)
        {
            //We store the position and rotation of starting marker
            startingPosition = parent.transform.position;
            startingRotation = parent.transform.rotation;
            //Acknowleding that we have scanned the starting marker
            isStartingMarkerScanned = true;
        }
    }

    void OnFlagImageTracked(GameObject parent)
    {
        var flagObject = Instantiate(flagPrefab, parent.transform);
        //Make Flag Object stand upright
        flagObject.transform.rotation = Quaternion.identity;
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
                    //In Case of `Marker-1` tracked, instantiate a Flag Prefab
                    OnFlagImageTracked(image.gameObject);
                    break;

                case "Start-Marker":
                    //In-case of Start-Marker, store it's position and rotation in real world
                    OnStartImageTracked(image.gameObject);
                    break;
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
