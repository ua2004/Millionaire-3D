using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class Question {

	public string question; //question text
	public string[] answers = new string[4]; //array of 4 answers
	public int finalAnswer; //0 means A, 1 means B, etc.
	public int correctAnswer; //0 means A, 1 means B, etc.
	public string correctAnswerText; //text of the correct answer
	public string synopsis; //short explanation of the correct answer which is shown after user gave his final answer
	public Animator[] answerAnimation = new Animator[4]; // animators for each answer

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
		this.GetQuestion();
	}

	/**
	 * Selects one random question from database and renders it on the screen.
	 */ 
	protected void GetQuestion()
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

		//instantiating logenze prefab
		GameObject logenze = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Classic/Logenze"));
		GameObject canvas = GameObject.Find("Canvas");
		logenze.transform.SetParent(canvas.transform, false);
		//removing "(Clone)" suffix from name
		logenze.transform.name = logenze.transform.name.Replace("(Clone)","").Trim();

		Text questionText = (Text) GameObject.Find("QuestionText").GetComponent<Text>();
		questionText.text = this.question;

		GameProcessScript gameProcessScript = (GameProcessScript) canvas.GetComponent<GameProcessScript>();

		/*
		The following initializations cannot be done in for loop, only separately.
		If we do this in the loop and pass "i" as a parameter to
			()=>{gameProcessScript.AnswerSelected(i);
		then "i" variable will always equal 4 since callback will be called after the loop is run.
		Therefore we have to pass a constant expression instead of "i".
		*/
		GameObject ans = GameObject.Find("AnsAText");
		Text ansText = (Text) ans.GetComponent<Text>();
		ansText.text = this.answers[0];
		//adding onclick event handler to answer text
		EventTrigger ansTrigger = (EventTrigger) ansText.GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(0);}, EventTriggerType.PointerClick);
		//adding onclick event handler to answer caption
		ansTrigger = GameObject.Find("AnsACaption").GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(0);}, EventTriggerType.PointerClick);

		ans = GameObject.Find("AnsBText");
		ansText = (Text) ans.GetComponent<Text>();
		ansText.text = this.answers[1];
		//adding onclick event handler to answer text
		ansTrigger = (EventTrigger) ansText.GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(1);}, EventTriggerType.PointerClick);
		//adding onclick event handler to answer caption
		ansTrigger = GameObject.Find("AnsBCaption").GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(1);}, EventTriggerType.PointerClick);

		ans = GameObject.Find("AnsCText");
		ansText = (Text) ans.GetComponent<Text>();
		ansText.text = this.answers[2];
		//adding onclick event handler to answer text
		ansTrigger = (EventTrigger) ansText.GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(2);}, EventTriggerType.PointerClick);
		//adding onclick event handler to answer caption
		ansTrigger = GameObject.Find("AnsCCaption").GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(2);}, EventTriggerType.PointerClick);

		ans = GameObject.Find("AnsDText");
		ansText = (Text) ans.GetComponent<Text>();
		ansText.text = this.answers[3];
		//adding onclick event handler to answer text
		ansTrigger = (EventTrigger) ansText.GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(3);}, EventTriggerType.PointerClick);
		//adding onclick event handler to answer caption
		ansTrigger = GameObject.Find("AnsDCaption").GetComponent<EventTrigger>();
		ansTrigger.AddEventTrigger(()=>{gameProcessScript.AnswerSelected(3);}, EventTriggerType.PointerClick);

		for(int i=0; i<=3; i++)
		{
			char letter = (char)((int)'A'+i); //65 is letter 'A', 66 is 'B', etc.

			this.answerAnimation[i] = (Animator) GameObject.Find("Ans" + letter + "Image").GetComponent<Animator>();
			this.answerAnimation[i].Play("ActiveAnswer");
		}
		
		//Debug.Log(this.question.synopsis);
		//Debug.Log("Question: " + q.question + " Correct answer: " + q.correctAnswer + " - " + q.correctAnswerText);
	}

	/**
	 * Set final answer and play corresponding animation.
	 * 
	 * @param int answerNumber Answer number given by user (0 means A, 1 means B, etc.)
	 */
	public void SetFinalAnswer(int answerNumber)
	{
		if((answerNumber < 0) || (answerNumber > 3))
		{
			answerNumber = 0;
		}
		this.finalAnswer = answerNumber;
		this.answerAnimation[answerNumber].Play("FinalAnswer");
	}

	/**
	 * Returns true if the given final aswer is correct.
	 * 
	 * @return bool
	 */
	public bool IsAnswerCorrect()
	{
		return this.finalAnswer == this.correctAnswer;
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
