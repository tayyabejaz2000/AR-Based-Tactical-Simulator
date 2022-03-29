using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void UpdateScore()
    {
        string currentScore = text.text;
        int score = int.Parse(currentScore);
        //Debug.Log(currentScore);
        score++;
        text.text = score.ToString();
    }
}
