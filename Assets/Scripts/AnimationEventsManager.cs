using UnityEngine;
using System.Collections;

public class AnimationEventsManager : MonoBehaviour {

    public void AllAnswersAreDisplayed()
    {
        UIManager.instance.allAnswersAreDisplayed = true;
    }
}
