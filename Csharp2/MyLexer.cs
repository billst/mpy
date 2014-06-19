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
    class MyLexer : Lexer
    {
        private int count;
        private TComment comment;
        private StringBuilder text;


        public MyLexer(TextReader tr)
            : base(tr)
        {

        }


        protected override void Filter()
        {
            if (this.currentState.Equals(State.COMMENT))
            {
                if (comment == null)
                {
                    comment = (TComment)token;
                    text = new StringBuilder(comment.Text);
                    count = 1;
                    token = null;

                }
                else
                {
                    text.Append(token.Text);
                    if (token is TComment)
                    {
                        count++;
                    }
                    else if (token is TNewLine)
                    {
                        count--;
                    }
                    if (count != 0)
                    {
                        token = null;
                    }
                    else
                    {
                        comment.Text = text.ToString();
                        token = comment;
                        this.currentState = State.NORMAL;
                        comment = null;
                    }
                }
            }

        }
    }
}
