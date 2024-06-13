using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBotSoundController : MonoBehaviour
{

    public AudioSource audioSourceWalk;
    public AudioSource audioSourceRun;
    public AudioSource audioSourceAU;
    public AudioSource audioSourceAD;

    
    
    void Footstep()
    {
        audioSourceWalk.Play();
    }

    void Runstep()
    {
        audioSourceRun.Play();
    }

    void ArmsUp()
    {
        audioSourceAU.Play();
    }

    void ArmsDown()
    {
        audioSourceAD.Play();
    }
    
}
