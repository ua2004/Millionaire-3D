using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Question {

	public string question; //question text
	public string[] answers = new string[4]; //array of 4 answers
	public int finalAnswer; //0 means A, 1 means B, etc.
	public int correctAnswer; //0 means A, 1 means B, etc.
	public string correctAnswerText; //text of the correct answer
	public string synopsis; //short explanation of the correct answer which is shown after user gave his final answer
	public Animator[] answerAnimation; // animators for each answer

	private SqliteDatabase db; //SQLite question database

	/*public Question(string question, string[] answers, int correctAnswer, string synopsis)
	{
		this.question = question;
		this.answers = answers;
		this.correctAnswer = correctAnswer;
		this.correctAnswerText = this.answers[this.correctAnswer];
		this.synopsis = synopsis;
	}*/

	public Question()
	{
		this.LoadQuestion();
	}

	/**
	 * Selects one random question from database.
	 */ 
	protected void LoadQuestion()
	{
		this.db = new SqliteDatabase("questions.bytes");
		string query = "SELECT * FROM `questions` WHERE `difficulty_level`='1' ORDER BY RANDOM() LIMIT 1;";
		DataTable result = this.db.ExecuteQuery(query);
		DataRow row = result[0];
		this.question = row["question"].ToString();
		this.answers = new string[] {row["answer1"].ToString(), row["answer2"].ToString(), row["answer3"].ToString(), row["answer4"].ToString()};
		this.correctAnswer = int.Parse(row["correct_answer"].ToString());
		this.correctAnswerText = this.answers[this.correctAnswer];
		this.synopsis = row["synopsis"].ToString();

		//instantiating logenze from prefab
		GameObject logenze = (GameObject) Object.Instantiate((GameObject) Resources.Load("Prefabs/Classic/Logenze", typeof(GameObject)));
		logenze.transform.SetParent(GameObject.Find("Canvas").transform, false);
		//removing "(Clone)" suffix from name
		logenze.transform.name = logenze.transform.name.Replace("(Clone)","").Trim();

		Text questionText = (Text) GameObject.Find("QuestionText").GetComponent<Text>();
		questionText.text = this.question;
		Text ansAText = (Text) GameObject.Find("AnsAText").GetComponent<Text>();
		ansAText.text = this.answers[0];
		Text ansBText = (Text) GameObject.Find("AnsBText").GetComponent<Text>();
		ansBText.text = this.answers[1];
		Text ansCText = (Text) GameObject.Find("AnsCText").GetComponent<Text>();
		ansCText.text = this.answers[2];
		Text ansDText = (Text) GameObject.Find("AnsDText").GetComponent<Text>();
		ansDText.text = this.answers[3];
		
		this.answerAnimation[0] = (Animator) GameObject.Find("AnsAImage").GetComponent<Animator>();
		this.answerAnimation[1] = (Animator) GameObject.Find("AnsBImage").GetComponent<Animator>();
		this.answerAnimation[2] = (Animator) GameObject.Find("AnsCImage").GetComponent<Animator>();
		this.answerAnimation[3] = (Animator) GameObject.Find("AnsDImage").GetComponent<Animator>();
		for(int i=0; i<=3; i++)
		{
			this.answerAnimation[i].Play("ActiveAnswer");
		}
		
		//Debug.Log(this.question.synopsis);
		//Debug.Log("Question: " + q.question + " Correct answer: " + q.correctAnswer + " - " + q.correctAnswerText);
	}	

	/**
	 * Returns number of available questions in database.
	 * 
	 * @todo It should count only questions for current language and category.
	 */
	protected int Count()
	{
		string query = "SELECT COUNT(*) AS `count` FROM `questions`";
		return (int)this.db.ExecuteQuery(query)[0]["count"];
	}
}
