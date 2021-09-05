using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI debugText;


    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
    private void Start()
    {
        Input.gyro.enabled = true;
    }
    private void Update()
    {
        CalculateRotation();

        //Reset
        if (Input.touchCount > 0)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    void CalculateRotation()
    {
        var rotation = Input.gyro.attitude.eulerAngles;
        transform.localRotation = Quaternion.Euler(360 - rotation.x, 360 - rotation.y, rotation.z);
    }
}
