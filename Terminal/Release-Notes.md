# Release Notes

v2.0.1.1 [Not released in nuget yet]

- Fixed bug on changing chapters using `NextChapter()` and `PreviousChapter()` methods.
- "Back to main menu" function is replaced with "Back to book select"
- A bit of text formatting

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