using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lifeline50x50 : Lifeline {

	public int[] Use()
	{
        int[] wrongAnswers = new int[2];
        //int wrongAnswer1, wrongAnswer2;
        do
        {
            wrongAnswers[0] = Random.Range(1, 5);
        }
        while (wrongAnswers[0] == GameProcess.gp.question.CorrectAnswer);

        do
        {
            wrongAnswers[1] = Random.Range(1, 5);
        }
        while ((wrongAnswers[1] == GameProcess.gp.question.CorrectAnswer) || (wrongAnswers[1] == wrongAnswers[0]));

        GameProcess.gp.isLifeline5050JustUsed = true;
        GameProcess.gp.isAnswerAvailable[wrongAnswers[0] - 1] = false;
        GameProcess.gp.isAnswerAvailable[wrongAnswers[1] - 1] = false;

        return wrongAnswers;        
	}
}
