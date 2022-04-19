using System;
using System.IO;
using System.Text;

namespace LoxInterpreter
{
    public class LoxInterpreter
    {
        private static readonly Interpreter Interpreter = new Interpreter();

        /// <summary>
        /// takes input from command line
        /// can take repl input or read from a file
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            if (args.Length > 1)
                Console.WriteLine("Usage: jlox [script]");
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        /// <summary>
        /// runs code if a file is input
        /// </summary>
        /// <param name="path"></param>
        private static void RunFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));
        }

        /// <summary>
        /// runs repl input version of lox
        /// </summary>
        private static void RunPrompt()
        {
            for (;;)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                Run(line);
            }
        }

        /// <summary>
        /// runs the code (runPrompt and runFile are wrappers for this)
        /// </summary>
        /// <param name="source"></param>
        private static void Run(string source)
        {
            // scans in tokens
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            // parses tokens
            var parser = new Parser(tokens);
            // turns parsed tokens into statements
            var statements = parser.Parse();
            // resolves statements
            var resolver = new Resolver(Interpreter);
            resolver.Resolve(statements);
            // evaluates all expressions
            Interpreter.Interpret(statements);
        }
    }
}