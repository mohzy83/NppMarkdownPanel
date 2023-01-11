### MathJax

<https://docs.mathjax.org/>

#### **AsciiMath2jax** (`$$`...`$$`) 

- <http://asciimath.org>
- <http://www.wjagray.co.uk/maths/SymbolList.html>
- <http://www1.chapman.edu/~jipsen/mathml/asciimathsyntax.html>

$$ sum_(k=1)^n k = 1+2+ cdots +n=(n(n+1)) / 2 $$ _ _ _ $$ f'(x) = dy/dx =\$100 $$ 

<span style="color:red">\$ab = cd$</span> - if two `$` appears in the same line disable `$...$` with `<span>` without class `kdmath` (for Jekyll) and use first `\$` (for N++ MdPanel).

$$ int_0^oo (x^3)/(e^x-1) dx = (pi^4)/(15) $${:style="display:block;text-align:center;"}

$$ ( a bc\ d ) \ \  [[a,b],[c,d]] $$

$$ ((n),(r)) $$; _ _ _ 
$$ A_(m,n) = ((a_(11), cdots , a_(1n)),(vdots, ddots, vdots),(a_(m1), cdots , a_(mn))) $$
$$ S_(m,n) = ((s_(11), cdots , s_(1n)),(vdots, ddots, vdots),(s_(m1), cdots , s_(mn))) $$

<!--mathjs in Notepad++ Markdown Panel--> 
<script type="text/javascript">
  window.MathJax = {
    tex2jax: {skipTags: ["p","div", "span","script","noscript","style","textarea","pre","code"], ignoreClass:"math"} ,asciimath2jax: {skipTags: ["span","script","noscript","style","textarea","pre","code"],processClass:"math",delimiters:[['$$','$$'],['\\(','\\)'],['\\[','\\]']]}
  };
  //skip <span> but not <span class="math">
  //https://docs.mathjax.org/en/v2.7-latest/options/preprocessors/asciimath2jax.html
</script>
<script type="text/javascript" async src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/latest.js?config=AM_SVG"></script>
