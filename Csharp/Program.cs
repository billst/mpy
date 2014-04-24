using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace tabPreproc1
{



    class Ident
    {
        public int length = 0;
        public int position = 0;
    }


    class Program
    {

        static void Main(string[] args)
        {
            string filepath = args[0];
            identInLines(filepath);

        }



        static void identInLines(string filepath)
        {

            string indent = "$";
            string dedent = "!";


            string input = "";
            using (StreamReader sr = new StreamReader(filepath))
            {

                input = sr.ReadToEnd();
                sr.Close();
                //input += Environment.NewLine + " ";
            }

            
            var matchesLines = Regex.Matches(input, @"(.+\w+.+)" + Environment.NewLine, RegexOptions.Multiline);
            string[] lines = new string[matchesLines.Count +1];
           // lines[lines.Length - 1] = " ";
            int index = 0; 
            int stored = 0;    
            int measured = 0;
            foreach (Match line in matchesLines)
            {

                measured = line.Value.Count(p => p == '\t');
               // measured = Regex.Match(line.Value, @"^(\t)+", RegexOptions.Multiline).Captures.Count;
                if (measured > stored)
                {
                    lines[index] = indent + line.Value;
                    stored++;
                }
                else if (measured < stored)
                {
                    string dedentString = Environment.NewLine;
                    for(int i = 0; i < stored - measured; i++)
                    {
                        dedentString += dedent + Environment.NewLine;
                    }
                    lines[index] = line.Value;
                    lines[index - 1] = lines[index - 1].TrimEnd();
                    while ( lines[index - 1].Substring(lines[index - 1].Length-2, 2) == Environment.NewLine )
                    {
                        lines[index - 1] = lines[index - 1].Trim(Environment.NewLine.ToCharArray());
                    }
                    
                    lines[index - 1] =  lines[index - 1] + dedentString ;

                    stored = measured;
                }
                else
                {
                    lines[index] = line.Value;
                }
                index++;

            }
            for (int j = 0; j < stored; j++)
            {
                lines[lines.Length - 1] += dedent + Environment.NewLine;
            }

            string output = "";

            output = String.Concat(lines);

            StreamWriter sw = new StreamWriter(filepath);
            sw.Write(output);
            sw.Close();
                        
        }


     



    }
}
