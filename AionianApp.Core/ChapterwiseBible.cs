using Aionian;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AionianApp
{
	/// <summary>A bible wrapped with helper functions that load a single chapter of a bible at a time</summary>
	public class ChapterwiseBible : Bible
	{
		/// <summary>The path of the books of Bible, given the root</summary>
		public static string GetBookPath(string root, BibleBook book) => $"{root}/{(byte)book}.dat";
		///<summary>The root folder where files for this bible is stored</summary>
		public readonly string RootPath;
		/// <summary>
		/// Loads the Bible from the asset
		/// </summary>
		/// <param name="loadedBible">The bible queried to load.</param>
		/// <param name="path">The path of bible books to load.</param>
		/// <returns>The bible object deserialized from the asset file</returns>
		public ChapterwiseBible(BibleDescriptor loadedBible, string path) : base(loadedBible) => RootPath = path;

		/// <summary>Fetches a book from this bible</summary>
		public override Book FetchBook(BibleBook book) =>
			JsonSerializer.Deserialize<Book>(
				File.ReadAllText(
					GetBookPath(RootPath, book)));
	}
}