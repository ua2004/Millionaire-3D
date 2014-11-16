using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBox {

	public delegate void Callback();

	/**
	 * Shows message box with OK button. Prefab at Assets/Prefabs/MessageBoxes/OK must exist.
	 * Main UI canvas must be named Canvas.
	 * 
	 * @param string text Text to be displayed
	 */
	public static void Show(string text)
	{
		GameObject msgBox = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/MessageBoxes/OK"));
		GameObject msgCanvas = GameObject.Find("Canvas");
		msgBox.transform.SetParent(msgCanvas.transform, false);
		Button msgBoxOKButton = (Button) GameObject.Find("OKButton").GetComponent<Button>();
		msgBoxOKButton.onClick.AddListener(delegate {
			GameObject.Destroy(msgBox);
		});
	}

	/**
	 * Shows message box with OK button. Prefab at Assets/Prefabs/MessageBoxes/YesNo must exist.
	 * Main UI canvas must be named Canvas.
	 * 
	 * @param string text Text to be displayed
	 * @param Callback yesCallback Delegate which is called when user clicks Yes button
	 * @param Callback noCallback Delegate which is called when user clicks No button
	 */
	public static void ShowYesNo(string text, Callback yesCallback, Callback noCallback)
	{
		GameObject msgBox = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/MessageBoxes/YesNo"));
		GameObject msgCanvas = GameObject.Find("Canvas");
		msgBox.transform.SetParent(msgCanvas.transform, false);

		bool whichButton = false;
		Button msgBoxYesButton = (Button) GameObject.Find("YesButton").GetComponent<Button>();
		msgBoxYesButton.onClick.AddListener(delegate {
			GameObject.Destroy(msgBox);
			whichButton = true;
			yesCallback();
		});
		Button msgBoxNoButton = (Button) GameObject.Find("NoButton").GetComponent<Button>();
		msgBoxNoButton.onClick.AddListener(delegate {
			GameObject.Destroy(msgBox);
			whichButton = false;
			noCallback();
		});
	}
}
