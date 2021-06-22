# Release Notes:

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