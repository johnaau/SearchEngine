using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine
{
	public class Indexing
	{
		public static string GenerateIndex(out List<string> customDictionary, params string[] content)
		{
			customDictionary = new List<string>();

			StringBuilder index = new StringBuilder();
			index.Append("|");

			int priority = 0;

			foreach (string contentItem in content)
			{
				if (priority != 0)
				{
					index.Append("{" + priority + "}|");
				}

				//content[i] = Common.QueryFix(content[i]);

				//foreach (string word in content[i].Split(' '))
				foreach (string word in Common.ExtractQueryWords(contentItem))
				{
					string indexedWord = word;

					if (Common.ContainsDigit(word) || Config.ExcludedFromStemming.Contains(word))
					{
						//Do not stem
					}
					else if (Common.GlobalSpellEngine["en"].Spell(word))
					{
						//Passes spell check, do stem

						//Find best stem
						List<string> stems = Common.GlobalSpellEngine["en"].Stem(word);
						foreach (string stem in stems)
						{
							if (string.Compare(stem, word, true) != 0 && stem.StartsWith(word.Substring(0, 1)))
							{
								indexedWord = stem;
								break;
							}
						}
					}
					else
					{
						//Add to user dictionary
						customDictionary.Add(word);
					}

					index.Append(indexedWord + "|");
				}

				priority++;
			}

			return index.ToString();
		}
	}
}
