using UnityEngine;
using System.Collections;

public class LifelineAudience : MonoBehaviour {

    private int probabilityOfCorrectAnswer = 90;

    /// <summary>
    /// Creates audience answers
    /// </summary>
    /// <returns>Returns array of 4 int values (at [0] is persents ofanswer A , [1] - B, etc)</returns>
    public int[] Use()
    {
        int[] results = new int[4];
        int idOfRightAnswer = GameProcess.instance.question.CorrectAnswer; // (1 to 4)

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //if lifeline 5050 was used for this question
        //we have 2 avaliable answers
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        if(GameProcess.instance.isLifeline5050JustUsed)
        {
            int idOfWrongAnswer = 1; // (1 to 4)                

            //finding id of wrong answer
            for (int i = 0; i < 4; i++)
            {
                if (GameProcess.instance.isAnswerAvailable[i] == true && idOfRightAnswer != i + 1)
                {
                    idOfWrongAnswer = i + 1;
                }
                //all other answers persents is set to 0
                else
                {
                    results[i] = 0;
                }
            }

            //if audience should give wrong answer
            if (Random.Range(1, 101) > probabilityOfCorrectAnswer) //probability of this is 100 - probabilityOfCorrectAnswer %
            {       
                //assigning wrong persents then rigth
                results[idOfWrongAnswer - 1] = Random.Range(51, 101);
                results[idOfRightAnswer - 1] = 100 - results[idOfWrongAnswer - 1];                
            }
            //if audience should give right answer
            else
            {
                //assigning rigth persents then wrong
                results[idOfRightAnswer - 1] = Random.Range(51, 101);
                results[idOfWrongAnswer - 1] = 100 - results[idOfRightAnswer - 1];
            }

            return results;

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //if lifeline 5050 was NOT used for this question
        //we have 4 avaliable answers
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        else
        {            
            //if audience should give wrong answer 
            if (Random.Range(1, 101) > probabilityOfCorrectAnswer) //probability of this is 100 - probabilityOfCorrectAnswer %
            {                
                int newIndex;
                do
                {
                    newIndex = Random.Range(1, 5);
                }
                while (newIndex == results[idOfRightAnswer - 1]);
            }

            //creating correct answer persentage
            results[idOfRightAnswer - 1] = Random.Range(40, 90);

            int notUsedPersents = 100 - results[idOfRightAnswer - 1];


            int indexOfQuestion = 1;
            //creating other persentages
            while (indexOfQuestion < 5)
            {
                //if it's not correct answer
                if (indexOfQuestion != idOfRightAnswer)
                {

                    //if it's last question, is's persents will be all pers. that was not used
                    if (indexOfQuestion == 4)
                    {
                        results[indexOfQuestion - 1] = notUsedPersents;
                    }
                    else
                    {
                        //taking random value from notUsedPersents but smaller than persents of correct answer
                        //if not used pers. biger than pers. of correct answer
                        if (notUsedPersents > results[idOfRightAnswer - 1])
                        {
                            results[indexOfQuestion - 1] = Random.Range(0, results[idOfRightAnswer - 1] - 1);
                            notUsedPersents -= results[indexOfQuestion - 1];
                        }
                        else
                        {
                            results[indexOfQuestion - 1] = Random.Range(0, notUsedPersents);
                            notUsedPersents -= results[indexOfQuestion - 1];
                        }

                    }

                }
                indexOfQuestion++;
            }

            return results;
        }
        
    }
}
