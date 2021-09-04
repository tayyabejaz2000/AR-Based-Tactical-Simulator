using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public TMPro.TextMeshProUGUI debugText;

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
        var rotation = Input.gyro.attitude.eulerAngles;
        transform.rotation = Quaternion.Euler(-rotation.x, -rotation.y, rotation.z);
    }
}
