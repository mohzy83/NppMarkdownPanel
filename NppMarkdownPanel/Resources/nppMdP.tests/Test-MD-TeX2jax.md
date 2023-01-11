### Math Inline

<https://docs.mathjax.org/>

#### **TeX2jax** (`$<space>`...`<space>$`, `$$`...`$$`):

- <https://math.meta.stackexchange.com/questions/5020/mathjax-basic-tutorial-and-quick-reference>
- <https://www.onemathematicalcat.org/MathJaxDocumentation/TeXSyntax.htm>


...  $ (ax^2 + bx + c = 0) $   ... **in**line ... 
$ \left(\begin{array}{c} n \newline r \end{array}\right) =\$100   $
... $ = a b $ = $100 = 100$

$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1} dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
$$

$$
A_{m,n} = 
\begin{pmatrix}
  a_{1,1} & a_{1,2} & \cdots & a_{1,n} \\
  a_{2,1} & a_{2,2} & \cdots & a_{2,n} \\
  \vdots  & \vdots  & \ddots & \vdots  \\
  a_{m,1} & a_{m,2} & \cdots & a_{m,n} 
\end{pmatrix}
$$



<!--mathjs in Notepad++ Markdown Panel
https://github.com/lunet-io/markdig/blob/master/src/Markdig.Tests/Specs/MathSpecs.md
--> 
<!--    tex2jax: {inlineMath: [['$','$']],processEscapes:true}
 -If if you enable the `$`...`$` in-line delimiters,  you may use `\$`-->
<script type="text/javascript">
  window.MathJax = {
    tex2jax: {inlineMath: [['$ ',' $'],['\\(','\\)']],displayMath: [['$$','$$'],['\\[','\\]']]
      ,processEscapes:false
    }
  };
</script>
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/latest.js?config=TeX-MML-AM_SVG"></script>

