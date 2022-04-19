using System;
using System.IO;
using System.Text;
using LoxInterpreter.Properties;

namespace LoxInterpreter
{
    public class LoxInterpreter
    {
        private static readonly Interpreter interpreter = new Interpreter();

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
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            var resolver = new Resolver(interpreter);
            resolver.resolve(statements);
            interpreter.Interpret(statements);

            //  foreach (var token in tokens) {
            //      Console.WriteLine(token);
            // }
        }
    }
}