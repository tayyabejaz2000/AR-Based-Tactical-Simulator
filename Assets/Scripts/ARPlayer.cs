using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class ARPlayer : MonoBehaviour
{
    PhotonView photonView;
    public Component[] localComponents;
    public Component[] otherComponents;

    private bool _isSet;
    public bool isMine
    {
        set
        {
            if (!_isSet)
            {
                _isSet = true;
                if (value)
                {
                    foreach (var component in otherComponents)
                        Destroy(component);
                }
                else
                {
                    foreach (var component in localComponents)
                        Destroy(component);
                }
            }
        }
    }

    void Start()
    {
        transform.parent = GameObject.Find("AR Session Origin").transform;
        photonView = GetComponent<PhotonView>();
        isMine = photonView.IsMine;

        if (photonView.IsMine)
        {
            gameObject.GetComponent<Camera>().enabled = true;
            gameObject.GetComponent<ARPoseDriver>().enabled = true;
            gameObject.GetComponent<ARCameraManager>().enabled = true;
            gameObject.GetComponent<ARCameraBackground>().enabled = true;

            GameObject.Find("ARCore Extensions").GetComponent<ARCoreExtensions>().CameraManager = GetComponent<ARCameraManager>();
            GetComponentInParent<ARSessionOrigin>().camera = GetComponent<Camera>();
        }
    }
}
