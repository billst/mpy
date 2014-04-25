import java.io.FileReader;
import java.io.PushbackReader;

//import bill.minipython2.lexer.Lexer;
//import bill.minipython2.node.Token;
import bill.minipython5_indented.lexer.Lexer;
import bill.minipython5_indented.node.Start;
import bill.minipython5_indented.node.Token;
import bill.minipython5_indented.parser.Parser;
import bill.minipython5_indented.*;

public class LexerTest1
{
  public static void main(String[] args)
  {
    try
    {
     MyLexer lexer =
        new MyLexer(
        new PushbackReader(
        new FileReader(args[0].toString()), 1024));

      Token token = lexer.next();
      while ( ! token.getText().equals("") )
      { 
        System.out.print(token);
        token = lexer.next(); 
       
      }
      MyLexer lexer2 =
    	        new MyLexer(
    	        new PushbackReader(
    	        new FileReader(args[0].toString()), 1024));
      Parser parser = new Parser(lexer2);      
      //bill.minipython5_indented.node.EOF e = new bill.minipython5_indented.node.EOF(1,2); 
      Start  ast = parser.parse(); 

    }
    catch (Exception e)
    {
      System.err.println(e);
    }
  }
}
