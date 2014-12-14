using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum State
{
	RULE_EXPLANATION,
	READING_QUESTION,
	WAITING_ANSWER,
	FINAL_ANSWER_GIVEN,
	CORRECT_ANSWER,
	WRONG_ANSWER,
	USING_LIFELINE,
	MONEY_TAKEN,
	MILLION_WON,
};

public class GameProcessScript : MonoBehaviour {

	public Language l; //current game language chosen by user
	public State state; // current game state
	//some answers may be unavailable after using 50x50 lifeline
	public bool[] isAnswerAvailable = new bool[4];
	public Question question;
	public int difficlutyLevel;
	public int questionNumber;
	public GameFormat gameFormat;

	public Image logenze; // logenze image, it contains question and 4 answers
	public Image currentPrizeImage; // image with current prize text, it is shown after correct answer

	// When the game starts
	void Start ()
	{
		this.l = new Language("uk-UA");
		this.StartGame();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void StartGame()
	{
		this.gameFormat = new ClassicGameFormat();
		this.difficlutyLevel = 1;
		this.questionNumber = 1;
		this.question = new Question();
		this.state = State.WAITING_ANSWER;
	}

	public void AnswerSelected(int answerNumber)
	{
		if( (this.state == State.WAITING_ANSWER) && (this.isAnswerAvailable[answerNumber]) )
		{
			this.state = State.FINAL_ANSWER_GIVEN;
			this.question.finalAnswer = answerNumber;
			//this.answerAnimation[answerNumber].Play("FinalAnswer");
			//StartCoroutine("RevealAnswer");
		}
	}

	/*public IEnumerator RevealAnswer()
	{
		// wait 3 to 6 seconds before revealing correct answer
		yield return new WaitForSeconds(Random.Range(3f, 6f));
		if( (this.state == State.FINAL_ANSWER_GIVEN) && (this.question.finalAnswer == this.question.correctAnswer) )
		{
			if(this.questionNumber == this.gameFormat.QuestionCount) // if last question correct
			{
				this.state = State.MILLION_WON;
				this.answerAnimation[this.question.finalAnswer].Play("CorrectAnswer");
				Debug.Log("Bravo! You are a millionaire!");
			}
			else
			{
				this.state = State.CORRECT_ANSWER;
				Debug.Log("Correct! You won " + this.gameFormat.GetPrizeForQuestion(this.questionNumber));
				this.answerAnimation[this.question.finalAnswer].Play("CorrectAnswer");
				//showing current prize bar
				Image prize = (Image) Object.Instantiate(this.currentPrizeImage);
				prize.transform.SetParent(GameObject.Find("Canvas").transform, false);
				Text prizeText = prize.GetComponentInChildren<Text>();
				prizeText.text = this.l.FormatPrize(this.gameFormat.GetPrizeForQuestion(this.questionNumber));

				yield return new WaitForSeconds(3);
				prize.transform.DestroyChildren();
				Destroy(prize);

				this.questionNumber++;
				this.question = new Question();
			}
		}
		else
		{
			this.state = State.WRONG_ANSWER;
			this.answerAnimation[this.question.correctAnswer].Play("WrongAnswer");
			Debug.Log("Wrong! Your total prize is " + this.gameFormat.GetGuaranteedPrizeForQuestion(this.questionNumber));
		}
	}	*/
}
