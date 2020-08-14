# Expression Evaluator
Expression Evaluator is a runtime evaluator for parser a string to linq expression tree then create lambda expression and compile to evaluate its value. Types are resolved like dynamic language.

# Get Started
* Target framework: netstandard2.0
* Supports binary operator(+,-,*,/,%, ==, !=, >, <, <=, <=, |, ^, &), unary operator(+,-,!,~), condition operator(?:)
* Supports access object's property, field, method and index(a.b, a.b(), a[i]).
* More test cases in src/Tests folder.

Example:  
```
var valueObj = new Dictionary<string, object>();
valueObj["a"] = 100;
valueObj["b"] = new { name = "XiaoMing", age = 100 };
valueObj["c"] = new List<int>() { 1, 2, 3, 4, 5 };
valueObj["d"] = new { items = new List<int> { 1, 2, 3, 4, 5 } };

Evaluator evaluator = new Evaluator(new EvaluateContext(valueObj));
evaluator.Evaluate("a"); //100
evaluator.Evaluate("b.name"); //"XiaoMing"
evaluator.Evaluate("b.name.ToString()"); //"XiaoMing"
evaluator.Evaluate("c[2]"); //3
evaluator.Evaluate("d.items[4]"); //5
evaluator.Evaluate("(1+(3-1)*4)/3"); //3d
```

## License
Distributed under the MIT license
