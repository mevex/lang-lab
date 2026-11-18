namespace SharpLox
{
    internal class Lox
    {
        static Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: SharpLox <script>");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        static void RunFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string source = File.ReadAllText(filePath);
                Run(source);

                if (hadError)
                    Environment.Exit(65);
                if (hadRuntimeError)
                    Environment.Exit(70);
            }
            else
            {
                Console.WriteLine($"Unable to find file {filePath}");
                Environment.Exit(10);
            }
        }

        static void RunPrompt()
        {
            Console.WriteLine("Lox terminal execution.");
            Console.WriteLine("Send Ctrl+Z on Windows or type Ctrl+D on Linux to terminate.");

            while (true)
            {
                Console.Write("> ");
                string? source = Console.ReadLine();

                if (source is null)
                    break;

                Run(source);
                hadError = false;
            }
        }

        static void Run(string? source)
        {
            Scanner scanner = new Scanner(source is not null ? source : "");
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            Expr? expression = parser.Parse();

            // Stop if there was an error
            if (hadError)
                return;

            interpreter.Interpret(expression);
        }

        public static void Error(int line, string message)
        {
            LogError(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
                LogError(token.line, "at end of file", message);
            else
                LogError(token.line, $"at '{token.lexeme}'", message);
        }
        public static void Error(RuntimeError e)
        {
            LogError(e.token.line, $"at '{e.token.lexeme}'", e.message);
            hadRuntimeError = true;
        }

        static void LogError(int line, string location, string message)
        {
            hadError = true;
            Console.Error.WriteLine($"[line {line}]: Error {location}: {message}");
        }
    }
}
