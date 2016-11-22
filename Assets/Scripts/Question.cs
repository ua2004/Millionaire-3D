using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class Question
{

    public string question; //question text
    public string[] answers = new string[4]; //array of 4 answers
    public int finalAnswer; //1 means A, 2 means B, etc.
    protected int _correctAnswer; //1 means A, 2 means B, etc.
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

    /// <summary>
    /// Number of right answer(1 to 4)
    /// </summary>
    public int CorrectAnswer
    {
        get
        {
            return _correctAnswer;
        }
    }



    /// <summary>
    ///Selects one random question from database and renders it on the screen.	 
    /// </summary>
    protected void GetQuestion()
    {
        this.db = new SqliteDatabase("questions.bytes");
        string query = "SELECT * FROM `questions` WHERE `difficulty_level`='1' ORDER BY RANDOM() LIMIT 1;";
        DataTable result = this.db.ExecuteQuery(query);
        DataRow row = result[0];
        this.question = row["question"].ToString();
        this.answers = new string[] { row["answer1"].ToString(), row["answer2"].ToString(), row["answer3"].ToString(), row["answer4"].ToString() };
        this._correctAnswer = int.Parse(row["correct_answer"].ToString()) + 1;
        this.correctAnswerText = this.answers[this._correctAnswer - 1];
        this.synopsis = row["synopsis"].ToString();

        

        UIManager.instance.ShowQuestion(question, answers);
    }

    /// <summary>
    /// Set final answer and play corresponding animation.
    /// </summary>
    /// <param name="answerNumber">Answer number given by user (1 means A, 2 means B, etc.)</param>
    public void SetFinalAnswer(int answerNumber)
    {
        if ((answerNumber < 1) || (answerNumber > 4))
        {
            answerNumber = 1;
        }
        finalAnswer = answerNumber;
    }

    /// <summary>
    /// Checks if final answer is correct.
    /// </summary>
    /// <returns>Returns true if the given final aswer is correct</returns>
	public bool IsAnswerCorrect()
    {
        return finalAnswer == _correctAnswer;
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
