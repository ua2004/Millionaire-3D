using UnityEngine;
using System.Collections;

public class Question {

	public string text; //question text
	public string[] answers = new string[4]; //array of 4 answers
	public int number; //question number (usually 1-15)
	public int price; //money that will bring the correct answer to this question
	public int level; //difficulty level (usually 1-3 where 1 means quesions 1-5, 2 means 6-10 etc.)
}
