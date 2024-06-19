using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // When the player collides with the staircase that is tagged as "Level" the scene called Level will be called and the score will increase by one
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Level")
        {
            SceneManager.LoadScene("Level");
            ScoreManager.scoreCount++;
            Debug.Log(SceneManager.GetActiveScene().name);
        }
    }
}
