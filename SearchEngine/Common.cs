using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NHunspell;

namespace SearchEngine
{
	public class Common
	{
		protected static SpellEngine _spellEngine = null;
		public static SpellEngine GlobalSpellEngine
		{
			get
			{
				if (_spellEngine == null)
				{
					_spellEngine = new SpellEngine();
					LanguageConfig enConfig = new LanguageConfig();
					enConfig.LanguageCode = "en";
					enConfig.HunspellAffFile = HttpContext.Current.Server.MapPath("~/App_Data/dictionaries/en-AU.aff");
					enConfig.HunspellDictFile = HttpContext.Current.Server.MapPath("~/App_Data/dictionaries/en-AU.dic");
					_spellEngine.AddLanguage(enConfig);
				}

				return _spellEngine;
			}
		}

		public static string StripTags(string htmlText, string replaceWith = "")
		{
			string result = htmlText == null ? string.Empty : htmlText;

			int startChar = result.IndexOf("<script", StringComparison.OrdinalIgnoreCase);
			while (startChar != -1)
			{
				int endChar = result.IndexOf("</script>", startChar, StringComparison.OrdinalIgnoreCase);
				result = result.Substring(0, startChar) + (endChar == -1 ? string.Empty : result.Substring(endChar + 9));
				startChar = result.IndexOf("<script", StringComparison.OrdinalIgnoreCase);
			}

			result = Regex.Replace(result, "<.*?>", replaceWith);
			return result;
		}

		public static string[] ExtractQueryWords(string phrase)
		{
			string result = phrase == null ? string.Empty : phrase.Trim();

			result = Common.StripTags(result.Trim().ToLower());

			result = HttpUtility.HtmlDecode(result);

			//Handle apostrophes
			//Remove apostrophe followed by s in words ending in 's
			//Remove other apostrophes but do not replace with space
			//result = result.Replace("'s", "");
			result = Regex.Replace(result, @"'s\b", "");
			result = result.Replace("'", "");

			//Fix ampersands
			result = result.Replace(" & ", " and ");

			//foreign characters
			result = result.Replace("à", "a");
			result = result.Replace("á", "a");
			result = result.Replace("â", "a");
			result = result.Replace("ã", "a");
			result = result.Replace("ä", "a");
			result = result.Replace("è", "e");
			result = result.Replace("é", "e");
			result = result.Replace("ê", "e");
			result = result.Replace("ë", "e");
			result = result.Replace("ì", "i");
			result = result.Replace("í", "i");
			result = result.Replace("î", "i");
			result = result.Replace("ï", "i");
			result = result.Replace("ñ", "n");
			result = result.Replace("ò", "o");
			result = result.Replace("ó", "o");
			result = result.Replace("ô", "o");
			result = result.Replace("õ", "o");
			result = result.Replace("ö", "o");
			result = result.Replace("ù", "u");
			result = result.Replace("ú", "u");
			result = result.Replace("û", "u");
			result = result.Replace("ü", "u");
			result = result.Replace("ý", "y");
			result = result.Replace("ÿ", "y");

			result = Regex.Replace(result, @"[^a-zA-Z0-9]", " ");
			while (result.IndexOf("  ") != -1)
				result = result.Replace("  ", " ");

			//Abbreviations
			result = Regex.Replace(result, @"\badmin\b", "administration");
			result = Regex.Replace(result, @"\bcert\b", "certificate");

			//Roman numerals
			result = Regex.Replace(result, @"\bii\b", "2");
			result = Regex.Replace(result, @"\biii\b", "3");
			result = Regex.Replace(result, @"\biv\b", "4");

			//Depluralisation
			result = Regex.Replace(result, @"\bchildrens\b", "child");
			result = Regex.Replace(result, @"\bchildren\b", "child");
			result = Regex.Replace(result, @"\bencyclopedias\b", "encyclopedia");

			//Some words should be split up
			result = Regex.Replace(result, @"\bforklift\b", "fork lift");
			result = Regex.Replace(result, @"\bchildcare\b", "child care");
			result = Regex.Replace(result, @"\bfirefight", "fire fight");

			//Stemming fixes
			result = Regex.Replace(result, @"\bdisabilities\b", "disable");
			result = Regex.Replace(result, @"\bdisability\b", "disable");

			//Other fixes
			result = Regex.Replace(result, @"\bco-ordinat", "coordinat");
			result = Regex.Replace(result, @"\bbased\b", "base");

			result = result.Trim();

			return result.Split(' ');
		}

		public static bool ContainsDigit(string value)
		{
			bool result = false;
			for (int i = 0; i < value.Length; i++)
			{
				if (char.IsNumber(value, i))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static string Truncate(string value, int length, string appendText = "...")
		{
			string result = value == null ? string.Empty : value.Trim();
			if (result.Length > length)
			{
				Regex allowedChars = new Regex("[0-9a-zA-Z]");
				result = result.Remove(length);
				for (int i = (result.Length - 1); i > 0; i--)
				{
					string character = result.Substring(i, 1);
					if (!allowedChars.IsMatch(character))
					{
						result = result.Remove(i);
						break;
					}
				}
				if (!string.IsNullOrEmpty(appendText))
					result += appendText;
			}
			return result;
		}
	}
}
