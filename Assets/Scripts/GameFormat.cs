using UnityEngine;
using System.Collections;

/**
 * Base game format class. If you create your own game format then its class
 * should extend this one.
 */
public abstract class GameFormat {

	//array of money prizes for each question
	/*private readonly int[] moneyTree;
	public int[] MoneyTree {
		get { return this.moneyTree; }
	}*/
	protected int[] moneyTree;

	public int QuestionCount
	{
		get
		{
			return this.moneyTree.Length;
		}
	}

	//array which contains question numbers which bring the player guaranteed prizes
	/*private readonly int[] guaranteedPrizes;
	public int[] GuaranteedPrizes {
		get { return this.guaranteedPrizes; }
	}*/
	protected int[] guaranteedPrizes;

	/**
	 * Returns amount of money that user gets when he answers the question correctly.
	 * 
	 * @param int questionNumber starts from 1
	 * @return int
	 */
	public int GetPrizeForQuestion(int questionNumber)
	{
		if( (questionNumber <= 0) || (questionNumber > this.moneyTree.Length) ) // if incorrect question number
		{
			throw new UnityException("Question with this number does not exist!");
		}
		return this.moneyTree[questionNumber - 1];
	}

	/**
	 * Returns formatted according to locale amount of money that user gets
	 * when he answers the question correctly.
	 * 
	 * @param int questionNumber starts from 1
	 * @return string
	 */
	/*public string GetFormattedPrizeForQuestion(int questionNumber)
	{
		int prize = this.GetPrizeForQuestion(questionNumber);

		return "";
	}*/

	/**
	 * Returns amount of money that users gets when he answers the question incorrectly.
	 * 
	 * @param int questionNumber starts from 1
	 * @return int
	 */
	public int GetGuaranteedPrizeForQuestion(int questionNumber)
	{
		if( (questionNumber <= 0) || (questionNumber > this.moneyTree.Length) ) // if incorrect question number
		{
			throw new UnityException("Question with this number does not exist!");
		}
		else if(questionNumber <= this.guaranteedPrizes[0]) // if before first guaranteed prize
		{
			return 0;
		}
		else if(questionNumber == this.moneyTree.Length) // if very last question
		{
			return this.moneyTree[this.guaranteedPrizes[this.guaranteedPrizes.Length - 2] - 1];
		}
		int i=0;
		while(this.guaranteedPrizes[i] < questionNumber)
		{
			i++;
		}
		return this.moneyTree[this.guaranteedPrizes[i - 1] - 1];
	}
}
