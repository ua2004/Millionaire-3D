using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LifelinePhone : MonoBehaviour
{

    int persentsOfRightAnswer = 80;

    public void Use()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //if lifeline 5050 was used for this question
        //we have 2 avaliable answers
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        if (GameProcess.gp.isLifeline5050JustUsed)
        {
            //probability of true equals to persentsOfRightAnswer%
            if (Random.Range(1, 101) < persentsOfRightAnswer)
            {
                //returnig correct answer
                ApplyLifeline(GameProcess.gp.question.CorrectAnswer);
            }
            else
            {
                int idOfWrongAnswer = 1; // (1 to 4)                

                //finding id of wrong answer
                for (int i = 0; i < 4; i++)
                {
                    if (GameProcess.gp.isAnswerAvailable[i] == true && GameProcess.gp.question.CorrectAnswer != i + 1)
                    {
                        idOfWrongAnswer = i + 1;
                    }

                }
                ApplyLifeline(idOfWrongAnswer);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //if lifeline 5050 was used for this question
        //we have 2 avaliable answers
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        else
        {
            //probability of true equals to persentsOfRightAnswer%
            if (Random.Range(1, 101) < persentsOfRightAnswer)
            {
                //returnig correct answer
                ApplyLifeline(GameProcess.gp.question.CorrectAnswer);
            }
            else
            {
                //returning wrong answer
                int wrongAnswer;

                do
                {
                    wrongAnswer = Random.Range(1, 5);
                }
                while (wrongAnswer == GameProcess.gp.question.CorrectAnswer);
                ApplyLifeline(wrongAnswer);
            }
        }


    }

    /// <summary>
    /// Applying lifeline
    /// </summary>
    /// <param name="answer">friend's answer, number of question(1 to 4)</param>
    void ApplyLifeline(int answer)
    {

        switch (answer)
        {
            case 1: UIManager.uim.StartCoroutine(LifelinePhoneAnimation("A")); break;
            case 2: UIManager.uim.StartCoroutine(LifelinePhoneAnimation("B")); break;
            case 3: UIManager.uim.StartCoroutine(LifelinePhoneAnimation("C")); break;
            case 4: UIManager.uim.StartCoroutine(LifelinePhoneAnimation("D")); break;
        }


        //making lifelinePhone button not interactable
        UIManager.uim.moneyTreePanel.transform.GetChild(2).GetComponent<Image>().sprite = UIManager.uim.moneyTreeSprites[8];
        UIManager.uim.moneyTreePanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }

    public IEnumerator newc()
    {
        Debug.Log("new co");
        yield return null;
    }

    public IEnumerator LifelinePhoneAnimation(string answer)
    {
        Debug.Log("Calling...");

        yield return new WaitForSeconds(1.5f);

        Debug.Log("- Yes");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("- Hello, it's Who wants to be a milionire!. How do you thing, which answer is right?f");
        yield return new WaitForSeconds(2.5f);

        int timeOfAnswer = Random.Range(0, 25); // time on tiner when friend will give an answer
        int timer = 30;

        UIManager.uim.StartCoroutine(UIManager.uim.CloseMoneyTreePanel());
        UIManager.uim.timerPaneL.SetActive(true);

        //waiting untill timer opens
        yield return new WaitForSeconds(0.611f);
        Debug.Log("- Hmmmm...");

        while (timer >= 0)
        {
            UIManager.uim.timerPaneL.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + timer;

            if (timeOfAnswer == timer)
            {
                Debug.Log("- I think it's " + answer);
                UIManager.uim.timerPaneL.transform.GetChild(0).GetComponent<Animator>().SetBool("HideTimer", true);
            }

            timer--;
            yield return new WaitForSeconds(1f);
        }
    }
}
