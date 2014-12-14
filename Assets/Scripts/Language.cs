using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Language {

	/**
	 * All language codes can be found here http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
	 */
	public string code;
	public const string DefaultLanguage = "en-US";
	private Dictionary<string, string> strings;
	private Dictionary<string, string> defaultStrings;

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
		this.strings = new Dictionary<string, string>();
		if(textAsset) // if language file exists
		{
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.LoadXml(textAsset.text.Replace("\n", string.Empty ));
			foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
			{
				this.strings.Add(node.Attributes.GetNamedItem("id").Value, node.InnerXml);
			}
		}

		textAsset = (TextAsset) Resources.Load("Strings/" + Language.DefaultLanguage);
		this.defaultStrings = new Dictionary<string, string>();
		if(textAsset) // if default language file exists
		{
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.LoadXml(textAsset.text.Replace("\n", string.Empty ));
			foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
			{
				this.defaultStrings.Add(node.Attributes.GetNamedItem("id").Value, node.InnerXml);
			}
		}
		else // if default language file doesn't exist
		{
			/**
			 * @todo Exception must call message box
			 */
			throw new UnityException("Def lang file doesn't exist!");
		}
	}

	/**
	 * Returns language string with given ID.
	 * 
	 * @param string id String ID in XML language file
	 * @return string
	 */
	public string T(string id)
	{
		if (this.strings.ContainsKey(id))
		{
			return this.strings[id];
		}
		else if(this.defaultStrings.ContainsKey(id))
		{
			return this.defaultStrings[id];
		}
		else
		{
			return id;
		}
	}

	/**
	 * Returns money prize amount formatted for the current locale.
	 * E.g., for en-US it returns $1,000 and for en-GB returns £1,000.
	 * 
	 * @param double prize Amount of money to be formatted
	 * @return string
	 */
	public string FormatPrize(double prize)
	{
		string formattedPrize = string.Format(new System.Globalization.CultureInfo(this.code), "{0:C0}", prize);
		if(this.code == "uk-UA")
		{
			//removing Ukrainian currency name "грн." at the end of the string
			//because in the Ukrainian TV show there was only a number without currency name
			formattedPrize = formattedPrize.Substring(0,formattedPrize.LastIndexOf(" "));
		}
		return formattedPrize;
	}
}
