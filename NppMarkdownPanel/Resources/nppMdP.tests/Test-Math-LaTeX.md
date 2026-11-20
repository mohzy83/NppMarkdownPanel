# LaTeX Math Rendering Test

Math is rendered with MathJax v3 (tex-svg) from a CDN.

## Inline formulas

- Dollar delimiter: $E = mc^2$ inside a sentence.
- Alternative delimiter: the quadratic formula \(x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}\) also works.
- Greek letters: $\alpha + \beta = \gamma$ and $\pi \approx 3.14159$.

## Block formulas

Sum of integers with `$$...$$`:

$$\sum_{i=1}^{n} x_i = \frac{n(n+1)}{2}$$

Matrix with `\[...\]`:

\[
A_{m,n} =
\begin{pmatrix}
  a_{1,1} & a_{1,2} & \cdots & a_{1,n} \\
  a_{2,1} & a_{2,2} & \cdots & a_{2,n} \\
  \vdots  & \vdots  & \ddots & \vdots  \\
  a_{m,1} & a_{m,2} & \cdots & a_{m,n}
\end{pmatrix}
\]

Integral with an `equation` environment:

$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
$$

## Mixed content

The energy-mass equivalence is $E = mc^2$, while the area of a circle is:

$$
A = \pi r^2
$$

An inline reference \(e^{i\pi} + 1 = 0\) (Euler's identity).
