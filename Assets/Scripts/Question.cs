using UnityEngine;
using System.Collections;

public class Question {

	public string question; //question text
	public string[] answers = new string[4]; //array of 4 answers
	public int correctAnswer; //0 means A, 1 means B, etc.
	public string correctAnswerText; //text of the correct answer
	public int number; //question number (usually 1-15)
	public string synopsis; //short explanation of the correct answer which is shown after user gave his final answer

	public Question(string question, string[] answers, int correctAnswer, string synopsis)
	{
		this.question = question;
		this.answers = answers;
		this.correctAnswer = correctAnswer;
		this.correctAnswerText = this.answers[this.correctAnswer];
		this.synopsis = synopsis;
	}
}
