using UnityEngine;
using System.Collections;

public static class LightAnimation
{

    public static void SmallCircleUp()
    {
        Debug.Log("sc light up animation");
        GameManager.gm.smallCircleLight.SetTrigger("lightUp");
    }

    public static void SmallCircleDown()
    {
        Debug.Log("sc light down animation");
        GameManager.gm.smallCircleLight.SetTrigger("lightDown");
    }

    public static void BigCircleUp()
    {
        Debug.Log("bc light up animation");
        GameManager.gm.bigCircleLight.SetTrigger("lightUp");
        GameManager.gm.doorLight.SetTrigger("lightUp");
    }

    public static void BigCircleDown()
    {
        Debug.Log("bc light down animation");
        GameManager.gm.bigCircleLight.SetTrigger("lightDown");
        GameManager.gm.doorLight.SetTrigger("lightDown");
    }
}
