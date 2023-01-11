```
A bug under IE/Edge with local file links containing non US-ASCII chars 
- with escape chars in URL with %..
```
1. ![](tst01.png "Title 01")
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/tst01.png" alt="" title="Title 01" />`

2. ![](tą01.png)
	* x : `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/t%C4%8501.png" alt="" />`
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test nonAscii path/tą01.png" alt="" />`

3. ![](tst%20(1).png)
	* x : `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/tst%2520(1).png" alt="" />`
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test nonAscii path/tst%20(1).png" alt="" />`

4. ![](AĄ%20(2).png "Title Ą")
	* x : `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/A%C4%84%2520(2).png" alt="" title="Title Ą" />`
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test nonAscii path/AĄ%20(2).png" alt="" title="Title Ą" />`

5. ![](AąCćE/tst01.png "Title 05")
	* x : `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/A%C4%85C%C4%87E/tst01.png" alt="" title="Title 05" />`
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test nonAscii path/AąCćE/tst01.png" alt="" title="Title 05" />`

6. ![](AąCćE/AĄ%20(2).png "Title AĄ")
	* x : `<img src="file:///C:/tmp/test.nonAscii.path/test%20nonAscii%20path/A%C4%85C%C4%87E/A%C4%84%2520(2).png" alt="" title="Title AĄ" />`
	* ok: `<img src="file:///C:/tmp/test.nonAscii.path/test nonAscii path/AąCćE/AĄ%20(2).png" alt="" title="Title AĄ" />`

This can be inserted in `MarkdownPreviewForm.cs` in line 128 (desperate solution).
````cs
            //using System.Text.RegularExpressions;
            //string inp = " * %25%20x : `<img src=\"file:///C:/tmp/test%20nonAscii%20path/A%C4%85C%C4%87E/A%C4%84%2520(2).png\" />`";
            //outp:          * %25%20x : `<img src="file:///C:/tmp/test nonAscii path/AąCćE/AĄ%20(2).png" />`
            Regex regex = new Regex("src=\"file:///[^\"]+");
            resultForBrowser = regex.Replace(resultForBrowser, m => Uri.UnescapeDataString(m.Value));
````
