using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Proceed to Level scene
    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ScoreManager.scoreCount = 0;    // Set score to default value of zero
    }

    // Load Title Screen
    public void returnToTitle()
    {
        SceneManager.LoadScene(0);
    }
}
