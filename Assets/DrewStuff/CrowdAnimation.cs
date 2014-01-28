using UnityEngine;
using System.Collections;

public class CrowdAnimation : MonoBehaviour
{
    private int randomAnim = 0;
    
    public void Cheer()
    {
        if (randomAnim == 0)
        {
            randomAnim = Random.Range(1, 4);
        }
        if (randomAnim <= 2)
        {
            animation.CrossFade("Cheer1");
            animation["Cheer1"].wrapMode = WrapMode.Loop;
        }
        else
        {
            animation.CrossFade("Cheer2");
            animation["Cheer2"].wrapMode = WrapMode.Loop;
        }
    }

    public void Idle()
    {
        animation.CrossFade("Idle");
        animation["Idle"].wrapMode = WrapMode.Loop;

        randomAnim = 0;
    }
}
