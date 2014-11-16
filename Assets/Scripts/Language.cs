using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Language {

	/**
	 * All language codes can be found here http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
	 */
	public string code;
	public const string DefaultLanguage = "en-US";

	public Language()
	{
		this.code = Language.DefaultLanguage;
		this.loadLanguageFile();
	}

	public Language(string code)
	{
		this.code = code;
		this.loadLanguageFile();
	}

	/**
	 * Loads XML file with strings for current language.
	 */
	private void loadLanguageFile()
	{
		TextAsset textAsset = (TextAsset) Resources.Load("Strings/" + this.code);
		if(!textAsset) //if language file doesn't exist
		{
			//Debug.Log("Asset not found. Loading en-US...");
			textAsset = (TextAsset) Resources.Load("Strings/en-US");
		}
		else
		{
			//Debug.Log ("Asset found!");
		}
		//Debug.Log(textAsset);
		
		/*XmlDocument xmldoc = new XmlDocument();
		xmldoc.LoadXml(textAsset.text.Replace("\n", String.Empty));
		foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
		{
			//Debug.Log(node);
		}*/
	}
}
