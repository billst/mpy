using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using bill.minipython5_indented.analysis;
using bill.minipython5_indented.lexer;
using bill.minipython5_indented.node;
using bill.minipython5_indented.parser;
using MiniPython1.walkers.SymbolAttributes_Datastructures;
using MiniPython1.walkers.TypeAttributeNamespace;
using System.Linq;


namespace MiniPython1.walkers
{
    class AstSemantics2 : DepthFirstAdapter
    {

        

        ContainersAndCurrentObjects containers;
         
        public AstSemantics2(ContainersAndCurrentObjects containersArg)
        {
            containers = containersArg;
            

        }


        private Object Value = new object();
        //private TypeAttributeNamespace.TypeAttribute Type = null;

        public override void CaseAStatementStmtOrFundef(AStatementStmtOrFundef node)
        {
            InAStatementStmtOrFundef(node);
            if (node.GetStmt() != null)
            {
                node.GetStmt().Apply(this);
            }
            OutAStatementStmtOrFundef(node);
        }

        public override void CaseAFundefStmtOrFundef(AFundefStmtOrFundef node)
        {


        }





        private void addIdentifierInSymbolTable(string ident, TypeAttributeNamespace.TypeAttribute type)
        {
            containers.Current_symbolAttributes = new SymbolAttributes(type);
            containers.SymbolTable.put(ident, containers.Current_symbolAttributes);
            
        }

        public override void CaseAAssignmentArrayStmtStmt(AAssignmentArrayStmtStmt node)
        {
            
            //TODO: Incomplete. Symbol Table etc
            InAAssignmentArrayStmtStmt(node);
            ArrayTypeAttribute TypeResult = new ArrayTypeAttribute();
            TIdentifier ident = null;
            if (node.GetIdentifier() != null)
            {
                node.GetIdentifier().Apply(this);
                ident = node.GetIdentifier();
            }
            if (node.GetExprInBrackets() != null)
            {
                node.GetExprInBrackets().Apply(this);
            }
            if (node.GetExprAssigned() != null)
            {
                node.GetExprAssigned().Apply(this);
                if (containers.Current_TypeAttribute.Name != "number") throw new Exception("The index of an Array must be integer Error in Line:" + ident.Line);
            }
            OutAAssignmentArrayStmtStmt(node);
        }
        

        public override void CaseTNumber(TNumber node)
        {
            //base.CaseTNumber(node);
            containers.Current_TypeAttribute= new TypeAttributeNamespace.TypeAttribute("number");
            containers.line_of_terminal = node.Line;
        }

        public override void CaseTString(TString node)
        {
            //base.CaseTString(node);
            containers.Current_TypeAttribute = new TypeAttributeNamespace.TypeAttribute("string");
            containers.line_of_terminal = node.Line;

        }

        public override void CaseANumberValue(ANumberValue node)
        {
           // base.CaseANumberValue(node);
            containers.Current_TypeAttribute = new TypeAttributeNamespace.TypeAttribute("number");
            

        }

        public override void CaseAString1Value(AString1Value node)
        {
           // base.CaseAString1Value(node);
            containers.Current_TypeAttribute = new TypeAttributeNamespace.TypeAttribute("string");

        }




        public override void CaseANumberExpression(ANumberExpression node)
        {
            InANumberExpression(node);
            if (node.GetNumber() != null)
            {
                node.GetNumber().Apply(this);
            }

            OutANumberExpression(node);
            containers.Current_TypeAttribute = new TypeAttribute("number");
        }











        public virtual void takeActionForUndecleredIdentExpression(TIdentifier ident)
        {
            throw new Exception("There is an assigment of undefinied identitifier in right side. Line:" + ident.Line + " Pos:" + ident.Pos);

        }

        public override void CaseAIdentExpression(AIdentExpression node)
        {
            InAIdentExpression(node);
            if (node.GetIdentifier() != null)
            {

                TIdentifier ident = node.GetIdentifier();
                string identKey = ident.Text;


                containers.line_of_terminal = ident.Line;

                if (!containers.contains_symbol(identKey))
                {
                    takeActionForUndecleredIdentExpression(ident);
                }
                else
                {
                    containers.Current_TypeAttribute = containers.SymbolTable.get(identKey).Type;
                }




                node.GetIdentifier().Apply(this);
            }
            else { throw new Exception("Some problem in an Expression.Ident"); }
            OutAIdentExpression(node);
        }





        #region operations

        public override void CaseAMultExpression(AMultExpression node)
        {
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            //InAMultExpression(node);
            String Line = "";
            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                leftType = containers.Current_TypeAttribute;
                
                Line = containers.line_of_terminal != -1 ? Convert.ToString(containers.line_of_terminal) : "";


                if (leftType.Name != "number") throw new Exception("Numbers must in be operands of a Multiplication. Line: " + Line);
            }
            else { throw new Exception("There isn't left Expression in a Multiplication. Line: " + Line); }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (rightType.Name != "number") throw new Exception("Numbers must in be operands of a Multiplication. Line: "+ Line);
            }
            else { throw new Exception("There isn't right Expression in a Multiplication. Line: " + Line); }
            OutAMultExpression(node);
            containers.Current_TypeAttribute = leftType;
        }


        public override void CaseADivExpression(ADivExpression node)
        {
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            //InADivExpression(node);
            String Line = "";
            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                leftType = containers.Current_TypeAttribute;

                Line = containers.line_of_terminal != -1 ? Convert.ToString(containers.line_of_terminal) : "";


                if (leftType.Name != "number") throw new Exception("Numbers must in be operands of a division. Line: " + Line);
            }
            else { throw new Exception("There isn't left Expression in a Division. Line: " + Line); }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (rightType.Name != "number") throw new Exception("Numbers must in be operands of a division. Line: " + Line);
            }
            else { throw new Exception("There isn't right Expression in a Division. Line: " + Line); }
            OutADivExpression(node);
            containers.Current_TypeAttribute = leftType;

        }



        public override void CaseAMinusExpression(AMinusExpression node)
        {
            InAMinusExpression(node);
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            string Line = "";
            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                
                leftType = containers.Current_TypeAttribute;

                Line = containers.line_of_terminal != -1 ? Convert.ToString(containers.line_of_terminal) : "";



                if (!(leftType.Name == "number" || leftType.Name == "string")) throw new Exception("Minus operands must be numbers or strings at node. Line: " + Line);
            }



            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (!(rightType.Name == "number" || rightType.Name == "string")) throw new Exception("Minus operands must be numbers or strings. Line: " + Line);
            }
            if (rightType.Name != leftType.Name) throw new Exception("Minus operands must have the some type. Line: " + Line);
            OutAMinusExpression(node);
        }

        public override void CaseAPlusExpression(APlusExpression node)
        {
  
            InAPlusExpression(node);
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            string Line = "";
            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);


                Line = containers.line_of_terminal != -1 ? Convert.ToString(containers.line_of_terminal) : "";

                leftType = containers.Current_TypeAttribute;
                if (!(leftType.Name == "number" || leftType.Name == "string") ) throw new Exception("Plus operands must be numbers or strings at node. Line: " + Line);
            } 
        


            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);

                rightType = containers.Current_TypeAttribute;
                if (!(rightType.Name == "number" || rightType.Name == "string") ) throw new Exception("Plus operands must be numbers or strings. Line: " + Line);
            }
            if (rightType.Name != leftType.Name) throw new Exception("Plus operands must have the some type Line: " +Line);
            OutAPlusExpression(node);
        }


        #endregion

        public override void CaseAReturnStmtStmt(AReturnStmtStmt node)
        {
            InAReturnStmtStmt(node);
            if (node.GetExpression() != null)
            {
                node.GetExpression().Apply(this);
            }

            OutAReturnStmtStmt(node);
        }



        public override void CaseAIfStmtStmt(AIfStmtStmt node)
        {

            InAIfStmtStmt(node);
            if (node.GetComparison() != null)
            {
                node.GetComparison().Apply(this);
            }
            else { throw new Exception("there isn't comparison Expression in a if statement"); }
            if (!containers.Current_TypeAttribute.Name.Equals("Boolean")) throw new Exception("Comparison must be Boolean");
            {
                Object[] temp = new Object[node.GetStmt().Count];
                node.GetStmt().CopyTo(temp, 0);


                for (int i = 0; i < temp.Length; i++)
                {
                    ((PStmt)temp[i]).Apply(this);

                    //in first return a make break without check next statements
                    if (containers.Current_TypeAttribute.Name != "void") break;

                }

            }
            OutAIfStmtStmt(node);
        }



        public override void CaseAAssignmentStmtStmt(AAssignmentStmtStmt node)
        {
            InAAssignmentStmtStmt(node);
            TIdentifier identifier = null;
            if (node.GetIdentifier() != null)
            {
                identifier = node.GetIdentifier();
                node.GetIdentifier().Apply(this);
            }
            else { throw new Exception("There is no identifier "); }
            if (node.GetExpression() != null)
            {

                node.GetExpression().Apply(this);


            }
            else { throw new Exception("There is no Expression in some assigment"); }
            OutAAssignmentStmtStmt(node);
            SymbolAttributes temp_sa = containers.SymbolTable.get(identifier.Text);
            if (temp_sa == null)
            {
                containers.addIdentifierInSymbolTable(identifier.Text, containers.Current_TypeAttribute);
            }
            else
            {
                temp_sa.Type = containers.Current_TypeAttribute;
            }
            containers.Current_TypeAttribute = new TypeAttribute("void");


        }



      

        public override void CaseAStringExpression(AStringExpression node)
        {

            InAStringExpression(node);
            if (node.GetString() != null)
            {
                TString tokenString = node.GetString();
                string keytString = tokenString.Text;

                containers.Current_TypeAttribute = new TypeAttribute("string");


                node.GetString().Apply(this);
            }
            else throw new Exception("Some error in an Expression.String");
            OutAStringExpression(node);
        }









        public override void OutANumberValue(ANumberValue node)
        {
            containers.Current_TypeAttribute = new TypeAttribute( "number");
        }

        public override void OutAString1Value(AString1Value node)
        {
            containers.Current_TypeAttribute = new TypeAttribute( "string");
        }





    
               

        public override void CaseAFuncCall(AFuncCall node)
        {

          
            InAFuncCall(node);
            if (node.GetIdentifier() == null)
            { throw new Exception("there is an Error in a Fun Call Expression"); }
            TIdentifier ident = node.GetIdentifier();
            string keyIdent = ident.Text;
            node.GetIdentifier().Apply(this);

            IEnumerable< FuncDef_Attributes> funDefs = containers.List_of_FunctionDefinitions.Where(p => p.Key == keyIdent).Select(p=>p.Value);
            if (funDefs.Count() == 0) throw new Exception("Call to undefined function in Line: " + ident.Line);


            ArgumTypeAttribute argums_type_of_function_call = null;

            if (node.GetArgList() != null)
            {
                node.GetArgList().Apply(this);
                argums_type_of_function_call = (ArgumTypeAttribute)containers.Current_TypeAttribute;
            }
            int accepted_defined_functions = 0;
            FuncDef_Attributes final_fundef = null;
            foreach (var fundef in funDefs)
            {
                if (!( fundef.Argums_List.Count < argums_type_of_function_call.List_of_types.Count || fundef.num_of_general_args > argums_type_of_function_call.List_of_types.Count))
                {
                    accepted_defined_functions++;
                    final_fundef = fundef;
                } 
                //if (fundefPair.num_of_general_args > argums_type_of_function_call.List_of_types.Count)
                //{
                //    throw new Exception("The Function Definition signature is different from the signature of the function call at Line: " + ident.Line + "and Postition: " + ident.Pos);
                //}
            }
            if (accepted_defined_functions != 1) throw new Exception("The Function Definition signature is different from the signature of the function call at Line: " + ident.Line + "and Postition: " + ident.Pos);

            OutAFuncCall(node);

            AstFuncDef astf = new AstFuncDef(containers, true);
            
          final_fundef.Node.Apply(astf);

            containers.Current_TypeAttribute = containers.Current_FuncDef_Attributes.ReturnType;
             
           
        }

        public override void CaseAArgList(AArgList node)
        {
            //InAArgList(node);
            ArgumTypeAttribute arg_type = new ArgumTypeAttribute();
            if (node.GetL() != null)
            {
                node.GetL().Apply(this);
                arg_type.List_of_types.Add(containers.Current_TypeAttribute);
            }
            else { throw new Exception("There is a problem in an Argum List"); }
            {
                Object[] temp = new Object[node.GetArgListTail().Count];
                node.GetArgListTail().CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                {
                    ((PExpression)temp[i]).Apply(this);
                    arg_type.List_of_types.Add(containers.Current_TypeAttribute);
                }
            }
            containers.Current_TypeAttribute = arg_type;
            OutAArgList(node);
        }



    }



    //class FunctionDefNodesAdapter : DepthFirstAdapter
    //{
    //    public override void CaseAAssignmentStmtStmt(AAssignmentStmtStmt node)
    //    {
    //        InAAssignmentStmtStmt(node);
    //        if (node.GetIdentifier() != null)
    //        {
    //            node.GetIdentifier().Apply(this);
    //        }
    //        if (node.GetExpression() != null)
    //        {
    //            node.GetExpression().Apply(this);
    //        }
    //        OutAAssignmentStmtStmt(node);
    //    }
    //}






    class ContainersAndCurrentObjects
    {
  
        public FuncDef_Attributes Current_FuncDef_Attributes = null;
        public SymbolAttributes_Datastructures.SymbolAttributes Current_symbolAttributes = null;
        public SymbolTable_LinkedList SymbolTable = new SymbolTable_LinkedList();
       // public Dictionary<string, FuncDef_Attributes> List_of_FunctionDefinitions = new Dictionary<string, FuncDef_Attributes>();
     
        public List<KeyValuePair<string, FuncDef_Attributes>> List_of_FunctionDefinitions = new List<KeyValuePair<string,FuncDef_Attributes>>();
             
        public TypeAttributeNamespace.TypeAttribute Current_TypeAttribute = null;
        public object Current_ValueAttribute = null;

        public int line_of_terminal = -1;

        public ContainersAndCurrentObjects()
        {
            
        }


        public void addIdentifierInSymbolTable(string ident, TypeAttribute type)
        {
            this.Current_symbolAttributes = new SymbolAttributes(type);
            this.SymbolTable.put(ident, this.Current_symbolAttributes);

        }


        public bool contains_symbol(string identKey)
        {
 




            SymbolAttributes sp = this.SymbolTable.get(identKey);
            if (sp == null) return false;
            else return true;
        }

    }



    namespace SymbolAttributes_Datastructures
    {

        class FuncDef_Attributes
        {

            public string identifier = "";

            public Dictionary<string, SymbolAttributes> Argums_List = new Dictionary<string, SymbolAttributes>();
            public TypeAttributeNamespace.TypeAttribute ReturnType = null;
            public Dictionary<string, string> local_var = new Dictionary<string, string>();
            public List<string> functionCalledIdentifiers = new List<string>();
            public List<string> rightval_var_undeclared = new List<string>();

            public int num_of_general_args = 0;

            public AFunction Node = null;

            public SymbolTable_LinkedList symbolTable_of_FuncDef = new SymbolTable_LinkedList();
            public ArgumTypeAttribute argumType = null;

        }

        class SymbolAttributes
        {
            public TypeAttribute Type = null;
            public object Value = null;

            public SymbolAttributes() { }
            public SymbolAttributes(TypeAttributeNamespace.TypeAttribute type)
            {
                Type = type;
            }


        }



        class SymbolTable_LinkedList
        {
            public SymbolTable_LinkedList previous = null;
            Dictionary<string, SymbolAttributes> Table = new Dictionary<string, SymbolAttributes>();

            public void put(string identText, SymbolAttributes type)
            {
                Table.Add(identText, type);
            }

            public SymbolAttributes get(string identText)
            {
                for (SymbolTable_LinkedList e = this; e != null; e = e.previous)
                {
                    SymbolAttributes t;
                    if (e.Table.TryGetValue(identText, out t))
                    {
                        return t;
                    }

                }
                return null;
            }



        }




    }





    namespace TypeAttributeNamespace
    {


        class TypeAttribute
        {
            public string Name = "";

            public TypeAttribute(string name)
            {
                Name = name;

            }
        }

        class FunctionCallTypeAttribute : TypeAttribute
        {
            public Dictionary<string, string> Argum_Types = new Dictionary<string, string>();
            public string ReturnType = "";

            public FunctionCallTypeAttribute(string name = "functioncall")
                : base(name)
            {


            }
        }

        class ArgumTypeAttribute : TypeAttribute
        {
            public ArgumTypeAttribute(string TypeName = "argum") : base(TypeName) { }

            //public int count = 0;
            public List<TypeAttribute> List_of_types = new List<TypeAttribute>();

        }


        class ArrayTypeAttribute : TypeAttribute
        {

            public int Length = 0;
            public TypeAttribute typeOfElements = null;


            public ArrayTypeAttribute(string TypeName = "array")
                : base(TypeName)
            {
            }


        }
    }








    

}


