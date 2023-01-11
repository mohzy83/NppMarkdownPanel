
### Extensions

~~strike through~~  (`~~`)

<http://xy.z>

<abc@xy.z>

#### tasklist 
- [x] foo
  - [ ] bar
  - [x] baz
- [ ] bim

### Code

```markdown
#### tasklist 
- [x] foo
  - [ ] bar
  - [x] baz
- [ ] bim
```

```python
if i > 0:
    print(f''' i =  {i}
2i = {2*i}''')
```

text `and code ` . . .

- - - - -

#### Local images (relative path)

![tst (1).png](test%20nonAscii%20path\tst%20(1).png "tst (1).png" ){width="60px"}  
 `![tst (1).png](test%20nonAscii%20path\tst%20(1).png "tst (1).png" ){width="20px"}`

![tst (1).png](test%20nonAscii%20path\tst%20(1).png?raw=1  "tst (1).png" ){width="60px"}  
 `![tst (1).png](test%20nonAscii%20path\tst%20(1).png?raw=1  "tst (1).png" ){width="20px"}`
(`?raw=1` works in some clouds)

#### HTTP image

![uUSB for power only](https://andrzejq.github.io/El_Prog/assets/img/Mikro-USB.png "uUSB for power only"){style="width:33%;"}  
`![uUSB for power only](https://andrzejq.github.io/El_Prog/assets/img/Mikro-USB.png "uUSB for power only"){style="width:33%;"}`

- - - - -

#### Tabels

header | _Pipe_ | table
-------|-------:|:-----:
  0    |      1 | 2
  3    | 4      | 5

|header | _Pipe_ | table
|-------|-------:|:-----:
|||                   <tr><td colspan=3>0 1 2
|  3    | 4      | 5  <tr><td colspan=3>**6 7** 8
| 9     | 10


#### Syntax highlighting

Python

````py
def f_ąćęł(ńóśźż):
  return ńóśźż
````

#### details ... summary

Does not work in NppMdP but useful in markdown (run SaveAs...).


<details markdown=1><summary markdown="span"> Expensive computation `cached.py` <br> . . . </summary>

````py
def expensive(arg1, arg2, *, _cache={}):
  if (arg1, arg2) in _cache:
    return _cache[(arg1, arg2)]

  result = ... expensive computation ...  # Calculate the value
  _cache[(arg1, arg2)] = result           # Store result in the cache
  return result
````

</details>





