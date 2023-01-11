## Wordle

**Wordcloud for data in table**

<div class="box"> <canvas id="my_canvas" class="canvas" width="450" height="350"></canvas> </div>

| phrase      | count |
| ---         | ---   |
| HDMI+audio  | 13    |
| USB-C DP    | 8     |
| VGA         | 0     |
| Miracast    | 1     |
| Chromecast  | 1     |
| DLNA        | 1     |
| other       | 5     |


<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/wordcloud2.js/1.1.0/wordcloud2.min.js"></script>
<!-- https://github.com/timdream/wordcloud2.js/blob/gh-pages/API.md - API document for available options.
     V. 1.1.0 wordcloud2.js works in Notepad++ - Markdown Panel (JScript 11.0.16384) -->
<script type="text/javascript">

  const MIN = 12, FACTOR = 4; //min. font size and scale

  let list = []; //[ [word,count], ...]
  let table0 = document.getElementsByTagName('table')[0].innerHTML; 
  //for the table on the page, search for 2-column rows, with a number in col.2
  re = /<tr[^>]*>\s*<td[^>]*>(.+?)<\/td>\s*<td[^>]*>(\d+)<\/td>\s*<\/tr>/gi
  while ( (m = re.exec(table0)) !== null ) { //alert(m[1] + "->" + m[2]);//test
    list.push( [ m[1], Math.max( MIN, FACTOR * parseInt(m[2]) ) ] );
  } //see. https://bugzilla.mozilla.org/show_bug.cgi?id=1776381
  
  WordCloud(document.getElementById('my_canvas'), { drawOutOfBound:true, backgroundColor:'#F0FFFF', list: list } );
</script>
<style> 
#my_canvas { pointer-events: none; cursor: pointer; margin: 10px;
    box-shadow: 0 0 7px 7px rgba(0, 0, 155, 0.3); background-color: aqua; border-radius: 100px;
} 
table {border-collapse: collapse; margin: 10px;}
</style>
