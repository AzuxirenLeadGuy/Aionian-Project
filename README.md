# Aionian Bible

This library and tool is a small attempt to bring Aionian Bible to the dotnet

## Aionian Library

Aionian Library contains simplistic features to consume the bible resources. As of now, the following is provided

The `BibleLink` struct which defines the bible link as provided in http://resources.aionianbible.org/, The `Bible` struct which contains all the content of bible using nested Dictionaries, and `BibleBook` enum which has all the books of the bible defined within it.

Provided with these are static utility methods, most importantly for getting all links from the website (link given above) and a method for deserializing the Bible struct from downloaded link

   
	var links = BibleLink.GetAllUrlsFromWebsite();//Gets all download links available from the website
	BibleLink mylink = links[0].Link; //Taking the first link
	var stream = mylink.DownloadStream();//Downloads the stream of the bible database
	var bible = Bible.ExtractBible(stream);//Now the bible is ready to use
										   
	//Alternatively, you can also use the following:
	
	var AnotherBible = Bible.ExtractBible(mylink.DownloadStream());//One line 
	string verse = AnotherBible[BibleBook.John, 3, 16];

By using a stream, there is an additional option of downloading the file by the URL in the `BibleLink` object and opening it via a `Stream` to load the Bible. There is now also an Async method for downloading the bible with events that are fired on the update on progress.

The bible struct can also be serialized in Json using `Newtonsoft.Json` package. The `System.Text.Json`package is incompatible since it cannot deal properly with `struct` types.

You can add this package from [Nuget](https://www.nuget.org/packages/Azuxiren.Aionian/)

## Aionian Tool

This is a basic terminal application (dotnet tool) to showcase the Aionian Library. It features the following:

1) Bible reading
2) Bible word search
3) Download/Manage any Aionian bible package

Install it from [Nuget](https://www.nuget.org/packages/Azuxiren.Aionian.Terminal/)

# Pending work and Contributing

These are some of the major tasks that are to be done

- Working on a cross-platform Aionian app (preferrably on the Uno platform)
- Improving the Cross-refernces support
- Any bug-fixes

Help/feedback on any aspect of the project is always welcome.

# Release Notes:

## Aionian Library

v2.0.1

- Removed a critical bug while extracting Bibles

v2.0

- Separated the base `Aionian` project from `System.Text.Json` dependancy
- Have all components of project as structs
- Updated the resources site to a github fork, having more metadata
- Add `RegionalBookName` to `Book`
- Added experimental Cross-Reference support
- Removed bugs due to a regex mistake, added better documentation
- `BibleLink.GetAllUrlsFromWebsite()` method now returns a catalog of all downloads avaiable, along with the filesize

v1.1
* Made properties of `Bible`, `BibleLink` and `Book` to be readonly
* Begin support for cross reference
* Added xUnit Test cases

v1.0.1.1
*	Corrected Bible abstract IDictionary dependancy

v1.0.1.0
*	Reduced Target Framework requirements;
*	Removed a bug causing potential memory leaks on BibleLink.GetAllUrlsFromWebsite();
*	Made BibleLink Serialiazable with Json (System.Text.Json)

v1.0.0.0 
*	First Release

## Aionian Tool

v2.0.1

- Chapterwise BookReading displays books much better
- Allows switching chapters (Next/Previous)

v2.0

- Added support of showing books in regional language
- Bug "more books shown with limited bibles" resolved
- Removed `ReadKey()` methods to support input/output redirection
- "Download Assets" section now shows bible sizes
- Better Console Progress bar
- Added dependancy `AionianApp.Core`, which contains all basic support of the Terminal Tool
- Removed `System.Text.Json` dependancy. Now uses `Newtonsoft.Json`

v1.1
* Removed bug which would crash the application on loading a chapter with missing verses (like Matthew 17:21)

v1.0.1.0
*	Reduced framework requirments;
*	Reduced dependancies; 
*	Made the program code more readable and easier to debug;
*	Corrected package to be tool

v1.0.0.0 
*	First Release

# Building

Building both the projects requires dotnet-sdk, available for download from [Microsoft's dotnet-sdk official site](https://dotnet.microsoft.com/download)

Going to the each of the project's folder, use

	dotnet build

to begin building. The nuget files are generated in the nuget folder.

# License/Copyright

Both the projects in these repositories are under [Creative Commons Attribution-No Derivatives 4.0.](https://creativecommons.org/licenses/by/4.0/)