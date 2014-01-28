using UnityEngine;
using System.Collections;

public class PlayIntroOnce : MonoBehaviour
{
    public static bool hasPlayedOnce = false;

    public AudioClip intro;

	// Update is called once per frame
	void Update ()
    {
        if (!hasPlayedOnce)
        {
            audio.PlayOneShot(intro);
            hasPlayedOnce = true;
        }
	}
}
