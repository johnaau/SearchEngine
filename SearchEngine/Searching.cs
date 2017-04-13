using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SearchEngine
{
	public class Searching
	{
		public Searching(string query)
		{
			_searchWords = Common.ExtractQueryWords(query);
		}

		protected string[] _searchWords;

		public string[] SearchWords
		{
			get
			{
				return _searchWords;
			}
		}

		public void Scorer<T>(List<T> results) where T : ISearchResult
		{
			List<SearchPhrase> searchPhrases = new List<SearchPhrase>();
			for (int i = this.SearchWords.Length - 1; i >= 0; i--)
			{
				for (int j = 0; j < (this.SearchWords.Length - (i + 0)); j++)
				{
					string test = "|";
					for (int z = 0; z < (i + 1); z++)
						test += this.SearchWords[j + z] + "|";

					SearchPhrase searchPhrase = new SearchPhrase();
					searchPhrase.Phrase = test;
					searchPhrase.WordCount = i + 1;
					searchPhrases.Add(searchPhrase);
				}
			}

			for (int i = 0; i < results.Count; i++)
			{
				int[] sectionStart = new int[5];
				int[] sectionPos = new int[4];
				int[] sectionCount = new int[4];
				for (int section = 1; section < 5; section++)
				{
					sectionStart[section] = (results[i].SearchIndex ?? string.Empty).IndexOf("|{" + section + "}|");
				}

				foreach (SearchPhrase searchPhrase in searchPhrases)
				{
					int pos = (results[i].SearchIndex ?? string.Empty).IndexOf(searchPhrase.Phrase);

					int lastSection = 0;
					int occurs = 0;
					while (pos != -1)
					{
						bool exclude = false;
						foreach (SearchPhrase other in searchPhrases)
						{
							if (other.WordCount > searchPhrase.WordCount)
							{
								int pos2 = other.Phrase.IndexOf(searchPhrase.Phrase);
								if (pos2 != -1)
								{
									if ((pos - pos2) >= 0 && results[i].SearchIndex.Length >= ((pos - pos2) + other.Phrase.Length) &&
										string.Compare(results[i].SearchIndex.Substring(pos - pos2, other.Phrase.Length), other.Phrase, false) == 0)
									{
										exclude = true;
										break;
									}
								}
							}
						}

						if (!exclude)
						{
							occurs++;

							int wordsScore = searchPhrase.WordCount;
							int sectionScore = 0;
							int newSectionScore = 0;
							int positionScore = 0;
							for (int section = 1; section < 5; section++)
							{
								if (pos < sectionStart[section] || section == 4)
								{
									int thisPos = (pos - (sectionStart[section - 1] + (section == 1 ? 0 : 4))) + 1;

									if (section == 1)
										sectionScore = 3;
									else if (section == 2)
										sectionScore = 2;
									else
										sectionScore = 1;

									newSectionScore = sectionScore;

									//if (!secondary)
									//	sectionScore = sectionScore + 3;

									int length = sectionStart[section] - sectionStart[section - 1] - ((section == 1 ? 0 : 4) - 1);

									if (section == 1 && thisPos == 1 && length == searchPhrase.Phrase.Length)
										thisPos = 0;

									if (length >= 100)
										length = 100;

									if (thisPos >= 100)
										thisPos = 99;

									positionScore = (int)((1 - ((decimal)thisPos / 100)) * 100);

									if (sectionScore == lastSection)
										exclude = true;

									lastSection = sectionScore;
									break;
								}
							}

							if (!exclude)
							{
								results[i].SearchScore += Math.Pow(2000, wordsScore - 1) * (Math.Pow(1000, newSectionScore - 1) * positionScore);
							}
						}
						if (pos == results[i].SearchIndex.Length)
							break;

						pos = results[i].SearchIndex.IndexOf(searchPhrase.Phrase, pos + 1);
					}
				}
			}
		}

		public string HitHighlight(string content, string startHighlight, string endHighlight, bool emptyIfNoMatch, bool truncate = true)
		{
			string result;
			string testContent = content;
			testContent = Common.StripTags(testContent);
			testContent = HttpContext.Current.Server.HtmlDecode(testContent);
			if (this.SearchWords == null || this.SearchWords.Count() == 0)
			{
				result = truncate ? Common.Truncate(testContent, 250) : testContent;
			}
			else
			{
				result = string.Empty;
				int? startPos = null;
				int pos = 0;
				bool boundary = false;
				bool atEnd = false;
				do
				{
					if (pos == (testContent.Length))
					{
						atEnd = true;
						if (startPos != null)
							boundary = true;
					}
					else
					{
						int chr = Char.ConvertToUtf32(testContent, pos);
						if ((chr >= 48 && chr <= 57) || (chr >= 65 && chr <= 90) || (chr >= 97 && chr <= 122))
						{
							if (startPos == null)
								startPos = pos;
						}
						else
						{
							if (startPos != null)
								boundary = true;
							else
								result += testContent.Substring(pos, 1);
						}
					}
					if (boundary)
					{
						string word = testContent.Substring((int)startPos, (pos - (atEnd ? 0 : 0)) - (int)startPos);
						string compare = word;
						if (!Common.ContainsDigit(compare) && !Config.ExcludedFromStemming.Contains(compare.ToLower()) && Common.GlobalSpellEngine["en"].Spell(compare))
						{
							List<string> stems = Common.GlobalSpellEngine["en"].Stem(compare);
							foreach (string stem in stems)
							{
								if (string.Compare(stem, compare, true) != 0 && stem.StartsWith(compare.ToLower().Substring(0, 1)))
								{
									compare = stem;
									break;
								}
							}
						}
						bool found = false;
						foreach (string term in this.SearchWords)
						{
							if (string.Compare(compare, term, true) == 0)
							{
								found = true;
								break;
							}
							else if (string.Compare(compare, "i", true) == 0 && term == "1")
							{
								found = true;
								break;
							}
							else if (string.Compare(compare, "ii", true) == 0 && term == "2")
							{
								found = true;
								break;
							}
							else if (string.Compare(compare, "iii", true) == 0 && term == "3")
							{
								found = true;
								break;
							}
							else if (string.Compare(compare, "iv", true) == 0 && term == "4")
							{
								found = true;
								break;
							}
						}
						result += (found ? ("<<" + word + ">>") : word);
						startPos = null;
						boundary = false;
						if (pos != (testContent.Length - 0))
							result += testContent.Substring(pos, 1);
					}
					pos++;
				} while (!atEnd);
				int startPos2 = result.IndexOf("<<");
				if (startPos2 == -1)
				{
					if (emptyIfNoMatch)
						result = string.Empty;
					else
						result = truncate ? Common.Truncate(testContent, 250) : testContent;
				}
				else
				{
					if (truncate)
					{
						int sentenceStart = result.LastIndexOf(".", startPos2);
						if (startPos2 - sentenceStart > 200)
						{
							int startPos3 = result.IndexOf(' ', startPos2 - 150);
							if (startPos3 == -1)
								startPos3 = startPos2 - 150;
							result = "&hellip;" + result.Substring(startPos3 + 1);
						}
						else
						{
							result = result.Substring(sentenceStart + 1);
						}
						startPos2 = result.IndexOf("<<");
						int endPos = result.IndexOf(".", startPos2);
						if (endPos != -1)
							result = result.Substring(0, endPos + 1);

						if (result.Length > 300)
						{
							int startPos3 = result.IndexOf(' ', 300);
							if (startPos3 == -1)
								startPos3 = 300;
							result = result.Substring(0, startPos3) + "&hellip;";
						}
					}
				}
				result = result.Replace("<<", startHighlight);
				result = result.Replace(">>", endHighlight);
				result = result.Trim();
			}
			return result;
		}

		protected struct SearchPhrase
		{
			public string Phrase;
			public int WordCount;
		}
	}
}
