using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class ClassicGameFormat : GameFormat {

	public ClassicGameFormat()
	{
		this.prefabPath = "Prefabs/Classic/";

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

		this.numberOfQuestionsWithGuarantedPrizes = new int[]{
			5,
			10,
			15,
		};

		this.lifelines = new Lifeline[1];
		this.lifelines[0] = new Lifeline50x50();
		//EventTrigger lifelineTrigger = (EventTrigger) GameObject.Find("Lifeline0").GetComponent<EventTrigger>();
		//lifelineTrigger.AddEventTrigger(()=>{this.lifelines[0].Use();}, EventTriggerType.PointerClick);
	}
}
