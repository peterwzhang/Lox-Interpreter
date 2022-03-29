using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public class LoxInterpreter
    {
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                //Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            run(Encoding.Default.GetString(bytes));
        }

        private static void runPrompt()
        {
            // StreamReader input = new StreamReader(Console.ReadLine());
            // BufferedStream reader = new BufferedStream(input);

            for (;;)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null) break;
                run(line);
            }
        }

        private static void run(string source)
        {
            var scanner = new Scanner(source);
             List<Token> tokens = scanner.scanTokens();
            
             foreach (var token in tokens) {
                 Console.WriteLine(token);
            }
        }
    }
}