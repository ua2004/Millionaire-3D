using UnityEngine;
using System.Collections;

public enum State
{
	RULE_EXPLANATION,
	READING_QUESTION,
	FULL_QUESTION,
	FINAL_ANSWER_GIVEN,
	CORRECT_ANSWER,
	WRONG_ANSWER,
	USING_LIFELINE,
	MONEY_TAKEN,
};

public class GameProcessScript : MonoBehaviour {

	public State state; // current game state
	//some answers may be unavailable after using 50x50 lifeline
	public bool[] isAnswerAvailable = new bool[4];

	// When the game starts
	void Start () {
		this.state = State.FULL_QUESTION;
		for (int i=0; i<4; i++)
		{
			this.isAnswerAvailable[i] = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
