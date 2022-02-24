using UnityEngine;
using UnityEngine.XR.ARFoundation;



public class ARAnchorInteraction : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject flagPrefab;

    void Start()
    {
        //Bind `OnImageTracked` function to event ARTrackedImageManager.trackedImagesChanged
        imageManager.trackedImagesChanged += OnImageTracked;
    }

    void OnStartImageTracked(GameObject parent)
    {
        //TODO: Sync position and rotation using pose of tracked image
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
