using System;
using System.IO;
using bill.minipython5_indented.analysis;
using bill.minipython5_indented.lexer;
using bill.minipython5_indented.node;
using bill.minipython5_indented.parser;


namespace MiniPython1
{
    class AstPrinter : DepthFirstAdapter
    {
        int indent;

        private void printIndent()
        {
            Console.Write( Environment.NewLine + "".PadLeft(indent, '\t'));
        }

        private void printNode(Node node)
        {
            printIndent();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(node.GetType().ToString().Replace("bill.minipython5_indented.node.", ""));

        }

        public override void DefaultIn(Node node)
        {
            printNode(node);
            indent++;
        }

        public override void DefaultOut(Node node)
        {
            indent--;
        }
    }
}
