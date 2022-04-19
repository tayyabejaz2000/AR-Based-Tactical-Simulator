using UnityEngine;
using TMPro;
using Photon.Pun;

public class Score : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateScore()
    {
        string currentScore = text.text.Split(':')[1].Trim();
        int score = int.Parse(currentScore) + 1;
        text.text = "Score: " + score.ToString();

        GetComponent<PhotonView>().RPC("SyncScoreRPC", RpcTarget.Others, text.text);
    }

    [PunRPC]
    public void SyncScoreRPC(string score)
    {
        text.text = score;
    }
}
