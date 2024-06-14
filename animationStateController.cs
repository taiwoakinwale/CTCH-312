using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isCaughtHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isCaughtHash = Animator.StringToHash("isCaught");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isCaught = animator.GetBool(isCaughtHash);
        bool movementPressed = Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d");

/*
set condition for player getting caught here
        if(enemy.isAttacking)
        {
            animator.SetBool(isCaughtHash, true);
        }
*/
        if (!isWalking && movementPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !movementPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }
    }
}
