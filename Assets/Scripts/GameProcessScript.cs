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

	public Language currentLang; //current game language chosen by user
	private SqliteDatabase db; //SQLite question database
	public State state; // current game state
	//some answers may be unavailable after using 50x50 lifeline
	public bool[] isAnswerAvailable = new bool[4];
	public Question question;
	public int difficlutyLevel;
	public GameFormat gameFormat;

	// When the game starts
	void Start () {
		this.currentLang = new Language("uk-UA");
		//Debug.Log (this.currentLang.code);
		this.db = new SqliteDatabase("questions.bytes");
		this.gameFormat = new ClassicGameFormat();
		this.state = State.FULL_QUESTION;
		this.difficlutyLevel = 1;
		for (int i=0; i<4; i++)
		{
			this.isAnswerAvailable[i] = true;
		}
		//LoadQuestion();

		MessageBox.ShowYesNo("Hohoho!", delegate {
			Debug.Log("User clicks YES!");
		}, 
		delegate {
			Debug.Log("User clicks NO!");
		});
	}
	
	// Update is called once per frame
	void Update () {
	
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
