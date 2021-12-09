using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARInteraction : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;

    private List<GameObject> spawnedObject;
    private List<GameObject> spawnedSprites;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    private Vector2 crosshairPosition;

    private Camera ARCamera;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        ARCamera = Camera.main;

        crosshairPosition = new Vector2(1920/2f, 1080/2f);

        spawnedObject = new List<GameObject>();
        spawnedSprites = new List<GameObject>();
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        return;

        /*
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if(spawnedObject == null)
            {
                spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
        */
    }

    void LateUpdate()
    {
        for(int i = 0 ; i < spawnedSprites.Count ; i++)
        {
            spawnedSprites[i].transform.LookAt(ARCamera.transform);
        }
    }

    public void AddObject()
    {
        if(_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if(spawnedObject == null)
            {
                spawnedObject.Add(Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation));
            }
            else
            {
                spawnedObject[spawnedObject.Count-1].transform.position = hitPose.position;
                spawnedObject[spawnedObject.Count-1].transform.rotation = hitPose.rotation;
            }
        }
    }

    public void AddAlert()
    {
        if(_arRaycastManager.Raycast(crosshairPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if(spawnedObject == null)
            {
                spawnedSprites.Add(Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation));
            }
            else
            {
                spawnedSprites[spawnedObject.Count-1].transform.position = hitPose.position;
                spawnedSprites[spawnedObject.Count-1].transform.rotation = hitPose.rotation;
            }
        }
    }

}
