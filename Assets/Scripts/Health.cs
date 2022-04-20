using UnityEngine;
using TMPro;
using Photon.Pun;

public class Health : MonoBehaviour
{
    TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateHealth()
    {
        string currentHealth = text.text.Split(':')[1].Trim();
        int health = int.Parse(currentHealth) - 1;
        text.text = "Health: " + health.ToString();

        GetComponent<PhotonView>().RPC("SyncHealthRPC", RpcTarget.Others, text.text);
    }

    [PunRPC]
    public void SyncHealthRPC(string health)
    {
        text.text = health;
    }
}
