using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateEnemies : MonoBehaviour
{
    [SerializeField] private GameObject theEnemy;

    [SerializeField] private int enemyAmount;

    [SerializeField] private Transform mazeTransform; // Reference to the maze's transform

    [SerializeField] private int mazeWidth;

    [SerializeField] private int mazeDepth;

    [SerializeField] private int mazeStartX;

    [SerializeField] private int mazeStartZ;

    private int enemyCount;
    private int enemyTotal;

    void Start()
    {
        enemyTotal = enemyAmount + ScoreManager.scoreCount;
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
        while (enemyCount < enemyTotal)
        {
            // Generate random positions within the maze's bounds
            float xPos = UnityEngine.Random.Range(mazeStartX, mazeStartX + mazeWidth);
            float zPos = UnityEngine.Random.Range(mazeStartZ, mazeStartZ + mazeDepth);

            // Round to the nearest 0.5
            xPos = Mathf.Round(xPos * 2) / 2f;
            zPos = Mathf.Round(zPos * 2) / 2f;

            // Apply the maze's scale to the positions
            xPos *= mazeTransform.localScale.x;
            zPos *= mazeTransform.localScale.z;

            // Translate the positions relative to the maze's position
            Vector3 spawnPosition = mazeTransform.position + new Vector3(xPos, 0.5f, zPos);

            Instantiate(theEnemy, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);

            enemyCount++;
        }
    }
}
