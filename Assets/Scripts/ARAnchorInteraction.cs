using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARAnchorInteraction : MonoBehaviour
{
    public ARAnchorManager anchorManager;

    public ARTrackedImageManager imageManager;


    void Start()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.trackedImagesChanged += OnImageTracked;
    }

    void OnImageTracked(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var image in eventArgs.added)
        {
            anchorManager.AddAnchor(new Pose(image.transform.position, image.transform.rotation));
        }
    }
}
