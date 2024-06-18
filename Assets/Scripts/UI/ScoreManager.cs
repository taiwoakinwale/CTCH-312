using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Text scoreText;          // Score display
    public static int scoreCount;   // Score count

    public int getScoreCount()
    {
        return scoreCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Display score value
        scoreText.text = "Score: " + Mathf.Round(scoreCount);
    }
}
