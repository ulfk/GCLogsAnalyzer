# GCLogsAnalyzer
Quick example-implementation to read "My Founds"-GPX-file from https://www.geocaching.com and produce HTML-page with statistics. Yes there is for example https://gsak.net/ but I wanted to do it myself just for fun :-)

# Details

Currently these statistics are printed to HTML:
- Founds by Country
- Founds by German "Bundesland"
- Founds by Cache Type
- Founds by Container Size
- Every 100th Found
- Founds by Owner (five and more founds)
- Logs ordered by Found Date
- Logs ordered by Placed Date

Output details:
- The GC-code is generated as link to the cache page.
- The coordinates are generated as Google Maps link to easyly go to that coordinates.
- The Owners are generated with a link to the profile-page.
- The HTML page has a table of contents with link to the sections and each section has a link to jump back to the top.

# ToDo

- Generate HTML without html/head/body to be able to be able to easyly include it in an existing HTML page
- Improove the styling of the output
- Add some more statistics (farest North/South/East/West, ...)