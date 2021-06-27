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
- Populating the XUnit tests for the existing `Aionian` and `AionianApp.Core` projects.
- Creating a helpful wiki and/or Github page
- Any bug-fixes
- Typos and grammatical mistakes

Help/feedback on any aspect of the project is always welcome.

# Release Notes:

- [Aionian Library](./Aionian/Release-Notes.md)
- [Terminal Tool](./Terminal/Release-Notes.md)

# Building

Building the projects requires dotnet-sdk, available for download from [Microsoft's dotnet-sdk official site](https://dotnet.microsoft.com/download). It is recommended to use the latest stable version of dotnet.
The currently recommended version is [dotnet-sdk-5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

To generate executable of the terminal tool, run the following command

    dotnet publish "path/to/Terminal.csproj" -f=net5.0 -c Release -o "path/of/output/files"

# Running

To run the Terminal Tool, use the script `dotnet run -f=net5.0` for the `Terminal.csproj` Project file.

# Packing

Packing both the projects into `.nupkg` files

At the project's root folder, where `Aionian.sln` is located, use the script

	dotnet build -c Release

to begin packing. The nuget files are generated in the folder `Nuget`.
You can use the packages to add dependancy to your project, or install the terminal tool using `dotnet tool`

# License/Copyright

Both the projects in these repositories are under [Creative Commons Attribution-No Derivatives 4.0.](https://creativecommons.org/licenses/by/4.0/)
