using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lifeline50x50 : Lifeline {

	public override void Use()
	{
		GameProcessScript gameProcessScript = (GameProcessScript) GameObject.Find("Canvas").GetComponent<GameProcessScript>();
		int wrongAnswer1, wrongAnswer2;
		do
		{
 			wrongAnswer1 = Random.Range(0, 4);
		} while(wrongAnswer1 == gameProcessScript.question.CorrectAnswer);
		do
		{
			wrongAnswer2 = Random.Range(0, 4);
		} while((wrongAnswer2 == gameProcessScript.question.CorrectAnswer) || (wrongAnswer2 == wrongAnswer1));
		char letter1 = (char)((int)'A'+wrongAnswer1); //65 is letter 'A', 66 is 'B', etc.
		char letter2 = (char)((int)'A'+wrongAnswer2);
		gameProcessScript.question.answerAnimation[wrongAnswer1].Play("InactiveAnswer");
		gameProcessScript.question.answerAnimation[wrongAnswer2].Play("InactiveAnswer");
		gameProcessScript.isAnswerAvailable[wrongAnswer1] = false;
		gameProcessScript.isAnswerAvailable[wrongAnswer2] = false;
	}
}
