using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using bill.minipython5_indented.lexer;
using bill.minipython5_indented.node;
using bill.minipython5_indented.parser;
using MiniPython1.walkers;
using System.Text.RegularExpressions;


namespace MiniPython1
{
    class Program
    {

        private static void Main(string[] args)
        {
            if (args.Length != 1)
                exit("Usage: compiler.exe filename");

            string programString = Tabs.Process(args);

            bool acceptMoreThan1Stmts = false;



          
            using (TextReader sr = new StringReader(programString))
            {
                // Read source
                MyLexer lexer = new MyLexer(sr);

                // Parse source
                Parser parser = new Parser(lexer);
                Start ast = null;

                try
                {
                    ast = parser.Parse();
                }
                catch (Exception ex)
                {
                    exit(ex.ToString());
                }

                // Print tree
                ContainersAndCurrentObjects containers = new ContainersAndCurrentObjects();
                AstPrinter printer = new AstPrinter();
                //ast.Apply(printer);
                AstFuncDef astFunDef = new AstFuncDef(containers);

                AstSemantics2 semantics2 = new AstSemantics2(containers);
              //  ast.Apply(astFunDef);
               // ast.Apply(semantics2);
                try
                {
                    ast.Apply(astFunDef);
                    ast.Apply(semantics2);
                }
                catch (Exception e)
                {
                  
                    exit(e.Message);
                    
                }
                
                ast.Apply(printer);
            }

            exit( "The Compiler finished without Exceptions. There are no problems");
        }

        private static void exit(string msg)
        {
            if (msg != null)
                Console.WriteLine(Environment.NewLine + msg);
            else
                Console.WriteLine();

            Console.WriteLine(Environment.NewLine + "Press any key to exit...");
            Console.Read();
            Environment.Exit(0);
        }
    }



    class State
    {
        public int stored = 0;
        public int level = 0;

        public void reset()
        {
            stored = 0;
            level = 0;
        }

        public int update(int measured)
        {
            if (this.stored == 0 && this.level == 0)
            {
                if (measured > 0)
                {

                    this.stored = 1;
                    this.level = measured - this.stored;
                    return 1;
                }
                else return 0;
            }
            else
            {
                if (measured - (level + stored) > 1) throw new Exception("there is a problem with indents");
                if (measured < this.level)
                {
                    int tempStored = this.stored;
                    this.reset();
                    if (measured > 0) throw new Exception("there is a problem with the indents");
                    return -1 * tempStored;

                }
                else if (level <= measured)
                {
                    int tempOutput = (measured - this.level) - this.stored;
                    this.stored = measured - level;
                    this.level = level;
                    if (level == measured) this.reset();
                    return tempOutput;
                }
                return 0;

            }

        }
    }


    class Tabs
    {

        public static string Process(string[] args)
        {
            string filepath = args[0];
            return identInLines(filepath);

        }



        static string identInLines(string filepath)
        {

            string indent = "$";
            string dedent = "!";

            State state = new State();
            string input = "";
            using (StreamReader sr = new StreamReader(filepath))
            {

                input = sr.ReadToEnd();
                // sr.Close();
                //input += Environment.NewLine + " ";
            }


            var matchesLines = Regex.Matches(input, @"(..+)" + Environment.NewLine, RegexOptions.Multiline);
            string[] lines = new string[matchesLines.Count + 1];
            // lines[lines.Length - 1] = " ";
            int index = 0;

            int measured = 0;
            foreach (Match line in matchesLines)
            {

                measured = line.Value.Count(p => p == '\t');
                // measured = Regex.Match(line.Value, @"^(\t)+", RegexOptions.Multiline).Captures.Count;

                int outputIndents = state.update(measured);
                if (outputIndents > 0)
                {
                    lines[index] = indent + line.Value;

                }
                else if (outputIndents < 0)
                {
                    string dedentString = Environment.NewLine;
                    for (int i = 0; i < (-1 * outputIndents); i++)
                    {
                        dedentString += dedent + Environment.NewLine;
                    }
                    lines[index] = line.Value;
                    lines[index - 1] = lines[index - 1].TrimEnd();
                    while (lines[index - 1].Substring(lines[index - 1].Length - 2, 2) == Environment.NewLine)
                    {
                        lines[index - 1] = lines[index - 1].Trim(Environment.NewLine.ToCharArray());
                    }

                    lines[index - 1] = lines[index - 1] + dedentString;


                }
                else
                {
                    lines[index] = line.Value;
                }
                index++;

            }
            for (int j = 0; j < state.stored; j++)
            {
                lines[lines.Length - 1] += dedent + Environment.NewLine;
            }

            string output = "";

            output = String.Concat(lines);

            //StreamWriter sw = new StreamWriter(filepath);
            //sw.Write(output);
            //sw.Close();

            return output;

        }





















    }
}
