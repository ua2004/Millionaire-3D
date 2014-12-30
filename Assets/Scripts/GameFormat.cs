using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Base game format class. If you create your own game format then its class
 * should extend this one.
 */
public abstract class GameFormat {

	/**
	 * Must have trailing slash.
	 */
	protected string prefabPath;

	/**
	 * Array of money prizes for each question.
	 */
	protected int[] moneyTree;
	public int[] MoneyTree {
		get { return this.moneyTree; }
	}

	public Lifeline[] lifelines;

	public int QuestionCount
	{
		get
		{
			return this.moneyTree.Length;
		}
	}

	/**
	 * Array that contains question numbers which bring the player guaranteed prizes.
	 */
	protected int[] guaranteedPrizes;
	public int[] GuaranteedPrizes {
		get { return this.guaranteedPrizes; }
	}

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

	/**
	 * Shows the money tree.
	 * @param int questionNumber current question number, will be selected
	 */
	public void ShowMoneyTree(int questionNumber)
	{
		GameObject mtree = (GameObject) GameObject.Instantiate(Resources.Load(this.prefabPath + "MoneyTree"));
		GameObject canvas = GameObject.Find("Canvas");
		mtree.transform.SetParent(canvas.transform, false);
		GameProcessScript gameProcessScript = (GameProcessScript) canvas.GetComponent<GameProcessScript>();
		//removing "(Clone)" suffix from name
		mtree.transform.name = mtree.transform.name.Replace("(Clone)","").Trim();
		GameObject a = GameObject.Find("QuestionPrize");
		int n = this.MoneyTree.Length;
		
		for(int i = 0; i < n; i++)
		{
			GameObject b = (GameObject) GameObject.Instantiate(a);
			b.transform.SetParent(mtree.transform, false);
			b.transform.SetPositionY(a.transform.position.y - 22*i);
			//removing "(Clone)" suffix from name
			b.transform.name = b.transform.name.Replace("(Clone)","").Trim() + (n - i);
			Text text = (Text) b.GetComponent<Text>();
			text.text = (n - i) < 10 ? "  " : "";
			text.text += (n - i) + "\t\t" + gameProcessScript.l.FormatPrize(this.MoneyTree[n-i-1]);
			if (System.Array.IndexOf(this.GuaranteedPrizes, n-i) > -1)
			{
				text.color = Color.white;
			}
		}
		GameObject.Destroy(a);
		
		GameObject currentQuestion = GameObject.Find("CurrentQuestion");
		currentQuestion.transform.SetPositionY(currentQuestion.transform.position.y - 22*(n - questionNumber));
	}

	/**
	 * Hides money tree and destroys it from the scene.
	 */
	public void HideMoneyTree()
	{
		GameObject mtree = GameObject.Find("MoneyTree");
		//mtree.transform.DestroyChildren();
		GameObject.Destroy(mtree);
	}
}
