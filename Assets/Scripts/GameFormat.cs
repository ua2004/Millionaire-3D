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
	public int[] moneyTree;

	//array which contains question numbers which bring the player guaranteed prizes
	/*private readonly int[] guaranteedPrizes;
	public int[] GuaranteedPrizes {
		get { return this.guaranteedPrizes; }
	}*/
	public int[] guaranteedPrizes;
}
