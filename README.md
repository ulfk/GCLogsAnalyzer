# GCLogsAnalyzer
Quick example-implementation to read "My Founds"-GPX-file from https://www.geocaching.com and produce HTML-page with statistics. 

Yes there are cool tools like https://gsak.net/ or https://project-gc.com/ but I wanted to do it myself just for fun :-)

# Details

Currently these statistics are printed to HTML:
- Founds by Country
- Founds by German "Bundesland"
- Founds by Cache Type
- Founds by Container Size
- Every 100th Found
- Founds by Owner (five and more founds)
- Cardinal Direction Maximums
- Logs ordered by Found Date
- Logs ordered by Placed Date

Output details:
- The GC-code is generated as link to the cache page.
- The coordinates are generated as Google Maps link to easily map these coordinates.
- The Owners are generated with a link to the profile-page.
- Each listed log has a link to jump to the "View Log"-page on gc.com
- The HTML page has a table of contents with link to the sections and each section has a link to jump back to the top.
- Tables have a maximum heigth and will be displayed with individual scrollbar, the table-header stays visible during scrolling

# ToDo

- Improove the styling of the output
- Add some more statistics
