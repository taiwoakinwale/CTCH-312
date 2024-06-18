using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    // Import animator component and variables
    Animator animator;          // Animator component
    public GameObject player;   // player object, used to stop movement when player gets caught
    int isWalkingHash;          // Hash version of isWalking
    int isCaughtHash;           // Hash version of isCaught
    

    // Start is called before the first frame update
    void Start()
    {
        // Get animator component
        animator = GetComponent<Animator>();

        // Convert animator variables to hash for greater efficiency
        isWalkingHash = Animator.StringToHash("isWalking");
        isCaughtHash = Animator.StringToHash("isCaught");
    }

    // Update is called once per frame
    void Update()
    {
        // Check animator variables
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isCaught = animator.GetBool(isCaughtHash);

        // Check if a movement key is pressed
        bool movementPressed = Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d");

        // If the player is immobilized, play caught animation
        if(player.GetComponent<PlayerController>().immobilized)
            animator.SetBool(isCaughtHash, true);
        
        // If the player is walking, play walking animation
        if (!isWalking && movementPressed)
            animator.SetBool(isWalkingHash, true);
        
        // If the player is not walking, play idle animation
        if (isWalking && !movementPressed)
            animator.SetBool(isWalkingHash, false);
    }
}
