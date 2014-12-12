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
	private SqliteDatabase db; //SQLite question database
	public State state; // current game state
	//some answers may be unavailable after using 50x50 lifeline
	public bool[] isAnswerAvailable = new bool[4];
	public Question question;
	public int difficlutyLevel;
	public int questionNumber;
	public GameFormat gameFormat;
	public Animator[] answerAnimation; // animators for each answer, specified in Unity editor

	public Image currentPrizeImage; // image with current prize text, it is shown after

	// When the game starts
	void Start () {
		this.l = new Language("uk-UA");
		this.StartGame();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame()
	{
		this.db = new SqliteDatabase("questions.bytes");
		this.gameFormat = new ClassicGameFormat();
		this.difficlutyLevel = 1;
		this.questionNumber = 1;
		LoadQuestion();
	}

	public void AnswerSelected(int answerNumber)
	{
		if( (this.state == State.WAITING_ANSWER) && (this.isAnswerAvailable[answerNumber]) )
		{
			//answerAnimation.Play("FinalAnswer");
			this.state = State.FINAL_ANSWER_GIVEN;
			this.question.finalAnswer = answerNumber;
			this.answerAnimation[answerNumber].Play("FinalAnswer");
			StartCoroutine("RevealAnswer");
		}
	}

	public IEnumerator RevealAnswer()
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
				//GameObject btn = (GameObject) Object.Instantiate(currentPrizeImage);
				Debug.Log("Correct! You won " + this.gameFormat.GetPrizeForQuestion(this.questionNumber));
				this.answerAnimation[this.question.finalAnswer].Play("CorrectAnswer");
				yield return new WaitForSeconds(3);
				this.questionNumber++;
				this.LoadQuestion();
			}
		}
		else
		{
			this.state = State.WRONG_ANSWER;
			this.answerAnimation[this.question.correctAnswer].Play("WrongAnswer");
			Debug.Log("Wrong! Your total prize is " + this.gameFormat.GetGuaranteedPrizeForQuestion(this.questionNumber));
		}
	}

	/**
	 * Selects one random question from database.
	 */ 
	public void LoadQuestion()
	{
		string query = "SELECT * FROM `questions` WHERE `difficulty_level`='" + this.difficlutyLevel + "' ORDER BY RANDOM() LIMIT 1;";
		DataTable result = this.db.ExecuteQuery(query);
		DataRow row = result[0];
		this.question = new Question(
			row["question"].ToString(),
			new string[] {row["answer1"].ToString(), row["answer2"].ToString(), row["answer3"].ToString(), row["answer4"].ToString()},
			int.Parse(row["correct_answer"].ToString()),
			row["synopsis"].ToString()
		);

		Text questionText = (Text) GameObject.Find("QuestionText").GetComponent<Text>();
		questionText.text = this.question.question;
		Text ansAText = (Text) GameObject.Find("AnsAText").GetComponent<Text>();
		ansAText.text = this.question.answers[0];
		Text ansBText = (Text) GameObject.Find("AnsBText").GetComponent<Text>();
		ansBText.text = this.question.answers[1];
		Text ansCText = (Text) GameObject.Find("AnsCText").GetComponent<Text>();
		ansCText.text = this.question.answers[2];
		Text ansDText = (Text) GameObject.Find("AnsDText").GetComponent<Text>();
		ansDText.text = this.question.answers[3];

		for(int i=0; i<=3; i++)
		{
			this.isAnswerAvailable[i] = true;
			this.answerAnimation[i].Play("ActiveAnswer");
		}
		this.state = State.WAITING_ANSWER;

		//Debug.Log(this.question.synopsis);
		//Debug.Log("Question: " + q.question + " Correct answer: " + q.correctAnswer + " - " + q.correctAnswerText);
	}

	/**
	 * Returns number of available questions in database.
	 * 
	 * @todo It should count only questions for current language and category.
	 */
	private int QuestionCount()
	{
		string query = "SELECT COUNT(*) AS `count` FROM `questions`";
		return (int)this.db.ExecuteQuery(query)[0]["count"];
	}
}
