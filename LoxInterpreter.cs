using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using LoxInterpreter.Properties;

namespace LoxInterpreter
{
    public class LoxInterpreter
    {
        private static Interpreter interpreter = new Interpreter();
        private static void Main(string[] args)
        {
            
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                //Environment.Exit(64);
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
             List<Token> tokens = scanner.scanTokens();
             Parser parser = new Parser(tokens);
             List<Stmt> statements = parser.Parse();
             Resolver resolver = new Resolver(interpreter);
             resolver.resolve(statements);
             interpreter.Interpret(statements);

            //  foreach (var token in tokens) {
            //      Console.WriteLine(token);
            // }
        }
    }
}