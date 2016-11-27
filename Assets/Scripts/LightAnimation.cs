using UnityEngine;
using System.Collections;

public static class LightAnimation
{

    public static void SmallCircleUp()
    {
        //Debug.Log("sc light up animation");
        GameManager.instance.smallCircleLight.SetTrigger("lightUp");
    }

    public static void SmallCircleDown()
    {
        //Debug.Log("sc light down animation");
        GameManager.instance.smallCircleLight.SetTrigger("lightDown");
    }

    public static void BigCircleUp()
    {
        //Debug.Log("bc light up animation");
        GameManager.instance.bigCircleLight.SetTrigger("lightUp");
        GameManager.instance.doorLight.SetTrigger("lightUp");
    }

    public static void BigCircleDown()
    {
        //Debug.Log("bc light down animation");
        GameManager.instance.bigCircleLight.SetTrigger("lightDown");
        GameManager.instance.doorLight.SetTrigger("lightDown");
    }

    public static void TurnOnBigCircle()
    {
        GameManager.instance.bigCircleLight.gameObject.SetActive(true);
    }

    public static void TurnOffBigCircle()
    {
        GameManager.instance.bigCircleLight.gameObject.SetActive(false);
    }
}
