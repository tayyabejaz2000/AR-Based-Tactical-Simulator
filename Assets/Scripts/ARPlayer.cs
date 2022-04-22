using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class ARPlayer : MonoBehaviour
{
	public string playerMarkerPrefabPath = "Prefabs/PlayerMarker";
	PhotonView photonView;

	public Vector3 localPosition;

	public Component[] localComponents;
	public Component[] otherComponents;

	private bool _isSet;

	GameObject playerMarker;
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
		photonView = GetComponent<PhotonView>();
		isMine = photonView.IsMine;

		if (photonView.IsMine)
		{
			transform.parent = GameObject.Find("AR Session Origin").transform;
			gameObject.GetComponent<Camera>().enabled = true;
			gameObject.GetComponent<ARPoseDriver>().enabled = true;
			gameObject.GetComponent<ARCameraManager>().enabled = true;
			gameObject.GetComponent<ARCameraBackground>().enabled = true;

			GameObject.Find("ARCore Extensions").GetComponent<ARCoreExtensions>().CameraManager = GetComponent<ARCameraManager>();
			GetComponentInParent<ARSessionOrigin>().camera = GetComponent<Camera>();

			playerMarker = PhotonNetwork.Instantiate(playerMarkerPrefabPath, Vector3.zero, Quaternion.identity);

			playerMarker.transform.GetChild(0).GetComponent<Image>().enabled = false;
			foreach (var textComp in playerMarker.GetComponentsInChildren<TMPro.TextMeshProUGUI>())
				textComp.enabled = false;
		}
		else
		{
			transform.parent = GameObject.Find("ScenarioObjects").transform;
		}
	}

	void LateUpdate()
	{
		if (photonView.IsMine)
		{
			localPosition = GameObject.Find("ScenarioObjects").transform.InverseTransformPoint(transform.position);
			playerMarker.GetComponent<ARPlayerMarker>().SetData(localPosition, PhotonNetwork.LocalPlayer.NickName);
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (photonView.IsMine && other.gameObject.tag == "Mine")
		{
			FindObjectOfType<Health>().UpdateHealth();
			PhotonView minePhoton = other.gameObject.GetComponent<PhotonView>();
			var viewID = minePhoton.ViewID;

			photonView.RPC("DestroyMine", RpcTarget.MasterClient, viewID);
		}
	}

	[PunRPC]
	void DestroyMine(int viewID)
	{
		PhotonView minePhoton = PhotonView.Find(viewID);
		if (minePhoton != null)
			PhotonNetwork.Destroy(minePhoton.gameObject);
	}
}
