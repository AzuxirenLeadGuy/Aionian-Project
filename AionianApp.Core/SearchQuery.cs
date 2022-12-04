using Aionian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AionianApp
{
	/// <summary>The Type of searches supported</summary>
	public enum SearchMode : byte
	{
		/// <summary>Matches a verse if any one of the words in the query is present</summary>
		MatchAnyWord,
		/// <summary>Matches a verse if all one of the words in the query is present</summary>
		MatchAllWords,
		/// <summary>Matches a verse using regex</summary>
		Regex,
	}
	/// <summary>Prepares a Search operation for a given string</summary>
	public struct SearchQuery
	{
		/// <summary>The regex string to search across the bible</summary>
		public readonly string SearchString;
		/// <summary>The Mode to search the bible in</summary>
		public readonly SearchMode Mode;
		/// <summary>
		/// Creates the query object with the given options
		///
		/// The input argument is converted into its regex equivalent given the mode to search
		/// </summary>
		/// <param name="query">The string query to search</param>
		/// <param name="mode">The mode to search</param>
		/// <example>
		/// Given a Bible that is initialized
		///
		/// The following query creates a search object for verses contatining "Peter", and verses containing "John" as well
		/// <code>
		/// var q1=new SearchQuery("Peter John", SearchMode.MatchAnyWord);
		///
		///
		/// </code>
		///
		///
		/// The following query creates a search object for verses contatining both "Peter" and "John"
		/// <code>
		/// var q1=new SearchQuery("Peter John", SearchMode.MatchAllWords);
		///
		///
		/// </code>
		///
		/// The following query creates a search object for verses contatining the exact phrase "Peter and John"
		/// <code>
		/// var q1=new SearchQuery("Peter and John", SearchMode.Regex);
		///
		///
		/// </code>
		/// </example>
		public SearchQuery(string query, SearchMode mode)
		{
			Mode = mode;
			if (query.Length == 0)
				throw new ArgumentException(message: "No input recieved", paramName: nameof(query));
			else if (Regex.Match(query, "[.,\\/#!$%\\^&\\*;:{}=\\-_`~()+='\"<>?/|%]").Success && (Mode == SearchMode.MatchAnyWord || Mode == SearchMode.MatchAllWords))
				throw new ArgumentException(message: "Cannot use punctutations for word search. Please use Regex search for that", paramName: nameof(query));
			else if (query.Count(x => x == ' ') >= 5 && Mode == SearchMode.MatchAllWords)
				throw new ArgumentException(message: @"Option 'Search for All of the words' is not available for more than 5 words.", paramName: nameof(query));
			//All exception cases completed
			switch (Mode)
			{
				case SearchMode.MatchAnyWord:
					query = new StringBuilder("(").Append(query).Replace(" ", ")|(").Append(')').ToString();//Use regex that matches any one of the words
					break;
				case SearchMode.MatchAllWords:
					string[] words = query.Split(separator: new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries);
					StringBuilder regexpreparer = new();
					foreach (string word in words) _ = regexpreparer.Append($"(?=.*\\b{word}\\b)");
					query = regexpreparer.Append("(^.*$)").ToString();
					break;
				case SearchMode.Regex://No need to change query
					break;
				default: throw new ArgumentException(message: "Undefined Mode used for SearchMode", paramName: nameof(mode));
			}
			SearchString = query;
		}
		/// <summary>
		/// Returns all matches of the query from a given bible
		/// </summary>
		/// <param name="searchBible">The bible to search from</param>
		/// <param name="ignoreCase">Whether the search should be case-insensitive. By default, it is true</param>
		/// <param name="onProgressUpdate">The IProgress</param>
		/// <param name="ct">The CancellationToken</param>
		/// <returns></returns>
		public IEnumerable<BibleReference> GetResults(Bible searchBible, bool ignoreCase = true, IProgress<float>? onProgressUpdate = null, CancellationToken ct = default)
		{
			Regex r = new(SearchString, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			float CurrentProgress = 0, ProgressInc = 1.0f / 66.0f;
			foreach (KeyValuePair<BibleBook, Book> bk in searchBible.Books)
			{
				foreach (KeyValuePair<byte, Dictionary<byte, string>> ch in bk.Value.Chapter)
				{
					foreach (KeyValuePair<byte, string> v in ch.Value)
					{
						if (ct.CanBeCanceled && ct.IsCancellationRequested) goto ex;
						if (r.Match(v.Value).Success)
						{
							yield return new BibleReference() { Book = bk.Key, Chapter = ch.Key, Verse = v.Key };
						}
					}
				}
				onProgressUpdate?.Report(CurrentProgress += ProgressInc);
			}
		ex:;
		}
	}
}