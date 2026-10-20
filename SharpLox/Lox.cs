namespace SharpLox
{
    internal class Lox
    {
        static bool hadError = false;

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
            }
            else
            {
                Console.WriteLine($"Unable to find file {filePath}");
                Environment.Exit(66);
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

                if (source == null)
                    break;

                Run(source);
                hadError = false;
            }
        }

        static void Run(string? source)
        {
            Scanner scanner = new Scanner(source != null ? source : "");
            List<Token> tokens = scanner.ScanTokens();

            if (!hadError)
            {
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t);
                }
            }
        }

        static public void Error(int line, string message)
        {
            LogError(line, "", message);
            hadError = true;
        }

        static void LogError(int line, string location, string message)
        {
            Console.Error.WriteLine($"[line {line}]: Error {location}: {message}");
        }
    }
}
