# Security / Sanitizer Test

Open this file in the preview. **Pass criteria:**

- No calculator or other program starts, no alert/dialog pops up.
- No red "XSS" banner is visible anywhere on the page.
- Every line marked `[visible]` is shown in the preview.
- Nothing marked `[hidden]` appears in the preview (the section headings and descriptions do).

---

## 1. Custom tags keep their content (issue #194)

<INSTRUCTIONS>

### [visible] Heading inside custom tag

- [visible] List item inside `<INSTRUCTIONS>`
- [visible] **Bold** and *italic* inside custom tag

</INSTRUCTIONS>

[visible] Text after the closing custom tag.

<foo>

[visible] Text after an unclosed custom tag `<foo>`.

<outer><inner onclick="document.body.innerHTML='<h1 style=background:red>XSS nested onclick</h1>'">[visible] nested custom tags, the onclick must be stripped (clicking does nothing)</inner></outer>

## 2. GHSA-g7w6-3g9r-883j - script navigation to shell (must NOT launch Calculator)

<script>
var u = 'shell:AppsFolder\\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App';
setTimeout(function(){ try { location.href = u; } catch (e) {} }, 400);
setTimeout(function(){ try { var a=document.createElement('a'); a.href=u; document.body.appendChild(a); a.click(); } catch(e){} }, 1100);
document.body.insertAdjacentHTML('afterbegin', '<h1 style="background:red">XSS script executed</h1>');
</script>

<script>
var v = 'http://markdownpanel-virtualhost/pop.cmd';
setTimeout(function(){ try { location.href = v; } catch (e) {} }, 400);
</script>

[visible] Two script blocks above must be removed completely (no script text shown).

## 3. Dangerous tags inside a custom tag

<custom><script>document.body.insertAdjacentHTML('afterbegin','<h1 style="background:red">XSS script in custom tag</h1>')</script>[visible] text next to a script inside a custom tag</custom>

<custom><style>body{display:none !important}</style>[visible] page must not turn blank (style removed)</custom>

<custom><iframe src="javascript:parent.document.body.innerHTML='XSS iframe'">[hidden] iframe fallback text</iframe></custom>

<custom><object data="javascript:alert('XSS object')">[hidden] object fallback</object><embed src="javascript:alert('XSS embed')"></custom>

<custom><noscript>[hidden] noscript content</noscript></custom>

<custom><template><img src=x onerror="alert('XSS template')"></template></custom>

## 4. Event handlers and javascript: URLs

<img src="x" onerror="document.body.insertAdjacentHTML('afterbegin','<h1 style=background:red>XSS img onerror</h1>')">

<svg onload="alert('XSS svg onload')"><circle r="10"></circle></svg>

<svg><style><img src=x onerror="alert('XSS svg style')"></style></svg>

<math><mtext><table><mglyph><style><img src=x onerror="alert('XSS math mXSS')"></style></mglyph></table></mtext></math>

<noscript><p title="</noscript><img src=x onerror=alert('XSS noscript mXSS')>"></p></noscript>

<body onload="alert('XSS body onload')">

<details open ontoggle="alert('XSS details ontoggle')"><summary>[visible] details element (no alert on toggle)</summary>[visible] details body</details>

<a href="javascript:alert('XSS js link')">[visible] javascript: link, the href must be stripped</a>

[\[visible\] Markdown javascript: link, the href must be stripped](javascript:alert(document.domain))

<a href="vbscript:msgbox('XSS')">[visible] vbscript: link, the href must be stripped</a>

<a href="shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App">[visible] shell: link, clicking must NOT start Calculator</a>

<form action="javascript:alert('XSS form')"><button formaction="javascript:alert('XSS formaction')">[visible] form button (clicking does nothing)</button></form>

<div style="background:url(javascript:alert('XSS css'))">[visible] div with javascript in CSS</div>

<meta http-equiv="refresh" content="0;url=shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App">

<base href="javascript:alert('XSS base')//">

## 5. Obfuscation

<scr<custom></custom>ipt>alert('XSS split tag')</scr<custom></custom>ipt>

<SCRIPT>document.body.insertAdjacentHTML('afterbegin','<h1 style=background:red>XSS uppercase script</h1>')</SCRIPT>

<a href="&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;&#58;alert('XSS entity')">[visible] entity-encoded javascript: link</a>

<a href="jav&#x09;ascript:alert('XSS tab')">[visible] tab-obfuscated javascript: link</a>

---

[visible] End of test. If you can read this line the document was not truncated.
