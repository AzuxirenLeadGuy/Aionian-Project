# Aionian Bible

This library and tool is a small attempt to bring Aionian Bible to the dotnet

## Aionian Library

Aionian Library contains simplistic features to consume the bible resources. As of now, the following is provided

The `BibleLink` struct which defines the bible link as provided in http://resources.aionianbible.org/, The `Bible` struct which contains all the content of bible using nested Dictionaries, and `BibleBook` enum which has all the books of the bible defined within it.

Provided with these are static utility methods, most importantly for getting all links from the website (link given above) and a method for deserializing the Bible struct from downloaded link

   
	BibleLink[] links = BibleLink.GetAllUrlsFromWebsite();//Gets all download links available from the website
	BibleLink mylink = links[0]; //Taking the first link
	var stream = mylink.DownloadStream();//Downloads the stream of the bible database
	var bible = Bible.ExtractBible(stream);//Now the bible is ready to use
										   
										   //Alternatively
	var AnotherBible = Bible.ExtractBible(mylink.DownloadStream());//One line 
	string verse = AnotherBible[BibleBook.John, 3, 16];

By using a stream, there is an additional option of downloading the file by the URL in the `BibleLink` object and opening it via a `Stream` to load the Bible.

The bible struct is also compatible to be serialized with Json, **provided the option `IncludeFields = true` is set.**
	
	//Prepare Option for Serializing
	var options = new JsonSerializerOptions() { IncludeFields = true }

	//Serialization Example
	string SerializedBible = JsonSerializer.Serialize(BibleObject, options);

	...
	//Deserialization Example
	var Bible = JsonSerializer.Deserialize<Bible>(SerializedBible, options);

You can add this package from [Nuget](https://www.nuget.org/packages/Azuxiren.Aionian/)

## Aionian Tool

This is a basic terminal application (dotnet tool) to showcase the Aionian Library. It features the following:

1) Bible reading
2) Bible word search
3) Download/Manage any Aionian bible package

Install it from [Nuget](https://www.nuget.org/packages/Azuxiren.Aionian.Terminal/)

# Release Notes:

## Aionian Library

v1.0.1.1
*	Corrected Bible abstract IDictionary dependancy

v1.0.1.0
*	Reduced Target Framework requirements;
*	Removed a bug causing potential memory leaks on BibleLink.GetAllUrlsFromWebsite();
*	Made BibleLink Serialiazable with Json (System.Text.Json)

v1.0.0.0 
*	First Release

## Aionian Tool

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