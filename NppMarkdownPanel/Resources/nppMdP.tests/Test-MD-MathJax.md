# MathJax rendering test

Covers the math support added in this pull request: dollar and backslash delimiters, code
left untouched, formulas that keep up while typing, and the offline asset path (the preview
renders with no network access).

## Inline math

- Dollar form: $E = mc^2$ inside a sentence.
- Backslash form: \(x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}\) inside a sentence.
- Greek letters and constants: $\alpha + \beta = \gamma$, $\pi \approx 3.14159$.

## Display math

Dollar form:

$$\sum_{i=1}^{n} x_i = \frac{n(n+1)}{2}$$

Bracket form:

\[
A_{m,n} =
\begin{pmatrix}
  a_{1,1} & a_{1,2} & \cdots & a_{1,n} \\
  a_{2,1} & a_{2,2} & \cdots & a_{2,n} \\
  \vdots  & \vdots  & \ddots  & \vdots  \\
  a_{m,1} & a_{m,2} & \cdots & a_{m,n}
\end{pmatrix}
\]

An `equation` environment:

$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
\end{equation}
$$

## Must not render as math

Inline code: `$not math$` and `\(not math\)` stay as typed.

Fenced code block:

```text
$fenced $code $block$
$$ \frac{1}{2} $$
```

Escaped dollar: \$5 stays a plain dollar sign.

## While typing

Type into one of the formulas above: the preview re-renders and the formula follows, without
a reload and without the first render coming out untypeset.
