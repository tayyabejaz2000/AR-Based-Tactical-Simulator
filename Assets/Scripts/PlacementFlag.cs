using UnityEngine;
using Photon.Pun;

public class PlacementFlag : MonoBehaviour
{
	public string flagName = "Capture Point";
	private Texture flagPingedTexture;

	public bool isPinged = false;
	//Attached Alert
	public PlacementAlert myAlert = null;

	private void Awake()
	{
		flagPingedTexture = Resources.Load<Texture>("Models/FlagModels/flag_lowpoly_Material.001_BaseColor_Black");
	}

	public void SetPose(Vector3 position, Quaternion rotation)
	{
		GetComponent<PhotonView>().RPC("SetPoseRPC", RpcTarget.All, position, rotation);
	}

	public void SetScanned()
	{
		GetComponent<PhotonView>().RPC("SetScannedRPC", RpcTarget.All);
	}

	[PunRPC]
	void SetScannedRPC()
	{
		GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", flagPingedTexture);
	}

	[PunRPC]
	void SetPoseRPC(Vector3 position, Quaternion rotation)
	{
		transform.parent = GameObject.Find("ScenarioObjects").transform;
		transform.localPosition = position;
		transform.localRotation = rotation;
	}


}
