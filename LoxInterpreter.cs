using System;
using System.IO;
using System.Text;

namespace LoxInterpreter
{
    public class LoxInterpreter
    {
        private static readonly Interpreter Interpreter = new Interpreter();

        private static void Main(string[] args)
        {
            if (args.Length > 1)
                Console.WriteLine("Usage: jlox [script]");
            //Environment.Exit(64);
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        private static void RunFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));
        }

        private static void RunPrompt()
        {
            //StreamReader input = new StreamReader(Console.ReadLine());
            //BufferedStream reader = new BufferedStream(input);

            for (;;)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            var resolver = new Resolver(Interpreter);
            resolver.Resolve(statements);
            Interpreter.Interpret(statements);

            //  foreach (var token in tokens) {
            //      Console.WriteLine(token);
            // }
        }
    }
}