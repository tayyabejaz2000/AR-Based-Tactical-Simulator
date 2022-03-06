using UnityEngine;
using UnityEngine.XR.ARFoundation;



public class ARAnchorInteraction : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject flagPrefab;
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public bool isStartingMarkerScanned;
    void Start()
    {
        //Bind `OnImageTracked` function to event ARTrackedImageManager.trackedImagesChanged
        imageManager.trackedImagesChanged += OnImageTracked;
        isStartingMarkerScanned = false;
    }

    void OnStartImageTracked(GameObject parent)
    {
        //TODO: Sync position and rotation using pose of tracked image
        //To avoid multiple scan and no scan
        if ( isStartingMarkerScanned == false )
        {
            //We store the position and rotation of starting marker
            startingPosition = transform.position;
            startingRotation = transform.rotation;
            //Acknowleding that we have scanned the starting marker
            isStartingMarkerScanned = true;
        }
    }

    void OnFlagImageTracked(GameObject parent)
    {
        var flagObject = Instantiate(flagPrefab, parent.transform);
        //Make Flag Object stand upright
        flagObject.transform.eulerAngles = Vector3.Scale(flagObject.transform.eulerAngles, Vector3.up);
    }

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

                case "Start Marker":
                    OnStartImageTracked(image.gameObject);
                    break;
            }
        }
    }
}
