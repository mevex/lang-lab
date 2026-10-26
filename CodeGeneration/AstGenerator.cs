string fileName = args[0];

Console.WriteLine("Current directory:");
Console.WriteLine(Environment.CurrentDirectory);

Console.WriteLine("\nWriting to file:");
Console.WriteLine(Path.GetFullPath(fileName));

List<string> astDescription = new() {
    "Binary     : Expr left, Token opr, Expr right",
    "Grouping   : Expr expression",
    "Literal    : object? value",
    "Unary      : Token opr, Expr right",
};

// Namespace opening and abstract parent class
string CodeGenerated = "namespace SharpLox \r\n{\r\n    internal abstract class Expr\r\n    {";
CodeGenerated += "\r\n        public abstract R Accept<R>(ExprVisitor<R> visitor);";
CodeGenerated += "\r\n    }\r\n\r\n";

// Visitor interface
CodeGenerated += "\r\n    internal interface ExprVisitor<R>";
CodeGenerated += "\r\n    {";
foreach (string description in astDescription)
{
    // Strings preprocessing
    string className = description.Split(':')[0].Trim();

    CodeGenerated += $"\r\n        public R Visit{className}({className} expr);";
}
CodeGenerated += "\r\n    }\r\n\r\n";

foreach (string description in astDescription)
{
    // Strings preprocessing
    string className = description.Split(':')[0].Trim();
    string parameters = description.Split(':')[1].Trim();
    string[] paramArray = parameters.Split(',');

    for (int i = 0; i < paramArray.Length; ++i)
    {
        paramArray[i] = paramArray[i].Trim();
    }

    // Class definition
    CodeGenerated += $"\r\n    internal class {className} : Expr\r\n    {{\r\n";

    // Member parameters
    foreach (string param in paramArray)
    {
        CodeGenerated += $"        public {param};\r\n";
    }
    CodeGenerated += "\r\n";

    // Constructor
    CodeGenerated += $"        public {className}(";
    for (int i = 0; i < paramArray.Length - 1; ++i)
    {
        CodeGenerated += $"{paramArray[i]}, ";
    }
    CodeGenerated += $"{paramArray[paramArray.Length - 1]})\r\n        {{\r\n";
    foreach (string param in paramArray)
    {
        CodeGenerated += $"            this.{param.Split(" ")[1]} = {param.Split(" ")[1]};\r\n";
    }

    // Close constructor
    CodeGenerated += "        }\r\n";

    CodeGenerated += "\r\n        public override R Accept<R>(ExprVisitor<R> visitor)";
    CodeGenerated += "\r\n        {";
    CodeGenerated += $"\r\n            return visitor.Visit{className}(this);";
    CodeGenerated += "\r\n        }";

    CodeGenerated += "\r\n    }\r\n";
}

// Close namespace
CodeGenerated += "}\r\n";

// Output
//Console.WriteLine($"\n\n{CodeGenerated}");
File.WriteAllText(fileName, CodeGenerated);
