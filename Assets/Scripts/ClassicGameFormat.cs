using UnityEngine;
using System.Collections;

public class ClassicGameFormat : GameFormat {

	public ClassicGameFormat()
	{
		this.moneyTree = new int[]{
			100, //first question prize
			200, //second question prize
			300, //etc.
			500,
			1000,
			2000,
			4000,
			8000,
			16000,
			32000,
			64000,
			125000,
			250000,
			500000,
			1000000,
		};

		this.guaranteedPrizes = new int[]{
			5,
			10,
		};
	}
}
