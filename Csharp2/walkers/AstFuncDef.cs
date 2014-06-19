using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using bill.minipython5_indented.analysis;
using bill.minipython5_indented.lexer;
using bill.minipython5_indented.node;
using bill.minipython5_indented.parser;
using MiniPython1.walkers.SymbolAttributes_Datastructures;
using MiniPython1.walkers.TypeAttributeNamespace;


namespace MiniPython1.walkers
{
    class AstFuncDef : AstSemantics2
    {

        public bool isCallFromFunctionCall;

        ContainersAndCurrentObjects containers = null;
        private bool acceptMoreThan1stmts = false;

        public AstFuncDef(ContainersAndCurrentObjects containersArg, bool isCallFromFunctionCall = false, bool acceptMoreThan1stmts = false)
            : base(containersArg)
        {
            containers = containersArg;
            this.isCallFromFunctionCall = isCallFromFunctionCall;

            this.acceptMoreThan1stmts = acceptMoreThan1stmts;
        }




        public void addToFunctionDefLocalVars(string keyIdentifier)
        {

        }


        public override void CaseAStatementStmtOrFundef(AStatementStmtOrFundef node)
        {

        }

        public override void CaseAFundefStmtOrFundef(AFundefStmtOrFundef node)
        {

            if (node.GetFunction() != null)
            {
                node.GetFunction().Apply(this);
            }

        }




        public override void CaseAFunction(AFunction node)
        {


            ArgumTypeAttribute argTypeFromFunCall = null;           
            if (isCallFromFunctionCall)
            {
               argTypeFromFunCall =(ArgumTypeAttribute) containers.Current_TypeAttribute;
            }


            containers.Current_FuncDef_Attributes = new FuncDef_Attributes();



            if (isCallFromFunctionCall)
            {

                containers.Current_FuncDef_Attributes.symbolTable_of_FuncDef.previous = containers.SymbolTable;
                
            }
            containers.SymbolTable = containers.Current_FuncDef_Attributes.symbolTable_of_FuncDef;

            containers.Current_FuncDef_Attributes.Node = (AFunction)node.Clone();

            // fProperties.Node.Apply(new AstSemantics1());






            TIdentifier ident = null;
            InAFunction(node);
            string keyIdent = "";
            if (node.GetIdentifier() != null)
            {
                ident = node.GetIdentifier();
                keyIdent = ident.Text;
                containers.Current_FuncDef_Attributes.identifier = keyIdent;

                node.GetIdentifier().Apply(this);
            }
            else { throw new Exception("Cannot be function without identifier"); }




            if (!isCallFromFunctionCall)
            {
                if (node.GetArgum() != null)
                {
                    node.GetArgum().Apply(this);

                    if (containers.Current_TypeAttribute.GetType() == typeof(ArgumTypeAttribute))
                    {
                        containers.Current_FuncDef_Attributes.argumType = (ArgumTypeAttribute)containers.Current_TypeAttribute;
                    }
                } 
            }
            else
            {

                if (node.GetArgum() != null)
                {
                    node.GetArgum().Apply(this);

                    if (containers.Current_TypeAttribute.GetType() == typeof(ArgumTypeAttribute))
                    {
                        containers.Current_FuncDef_Attributes.argumType = (ArgumTypeAttribute)containers.Current_TypeAttribute;
                    }
                } 


               // containers.Current_FuncDef_Attributes.argumType = argTypeFromFunCall;
                    if (isCallFromFunctionCall)
                    {
                        for (int i = 0; i < containers.Current_FuncDef_Attributes.argumType.List_of_types.Count; i++)
                        {
                            
                            
                            if (containers.Current_FuncDef_Attributes.argumType.List_of_types[i].Name == "general")
                            {
                                containers.Current_FuncDef_Attributes.argumType.List_of_types[i].Name = argTypeFromFunCall.List_of_types[i].Name;
                            }
                        }
                    }
                
            }





            {
                Object[] temp = new Object[node.GetStmt().Count];
                if (!acceptMoreThan1stmts && temp.Length > 1) throw new Exception("This Language does not accept more than one statement in statement or Function Definition Line near:" + containers.line_of_terminal);
                node.GetStmt().CopyTo(temp, 0);
                TypeAttribute cType = new TypeAttribute("void");
                for (int i = 0; i < temp.Length; i++)
                {
                    ((PStmt)temp[i]).Apply(this);
                    if (containers.Current_TypeAttribute.Name != "void" && cType.Name == "void")
                    {
                        cType = containers.Current_TypeAttribute;
                    }
                    else if (containers.Current_TypeAttribute.Name != "void" && containers.Current_TypeAttribute.Name != cType.Name && cType.Name != "void")
                    {
                        throw new Exception(" The function definition at Line: " + ident.Line + " return more than one type");
                    }

                }
                containers.Current_FuncDef_Attributes.ReturnType = cType;

            }
            if (!isCallFromFunctionCall)
            {
                if (containers.List_of_FunctionDefinitions.Any(p => p.Key == keyIdent))
                {
                    IEnumerable<FuncDef_Attributes> f_attributes = containers.List_of_FunctionDefinitions.Where(p => p.Key == keyIdent).Select(p => p.Value);

                    foreach (var f_attribute in f_attributes)
                    {


                        int diff_of_argum = containers.Current_FuncDef_Attributes.Argums_List.Count - f_attribute.Argums_List.Count;
                        //FuncDef_Attributes small_FuncDef_Attr = null;
                        //if (diff_of_argum >= 0) small_FuncDef_Attr = f_attribute;
                        //else small_FuncDef_Attr = current_FunDef_Attr;
                        if (diff_of_argum == 0)
                        {
                            throw new Exception("There are 2 functions definition with the some signature. Line: " + ident.Line);
                        }
                        else if (containers.Current_FuncDef_Attributes.Argums_List.Where(p => p.Value.Type.Name == "general").Count() ==
                            f_attribute.Argums_List.Where(p => p.Value.Type.Name == "general").Count())
                        {

                            throw new Exception("There are 2 function with unexpected signature. Line: " + ident.Line);
                        }



                    }
                    
                        containers.List_of_FunctionDefinitions.Add(new KeyValuePair<string, FuncDef_Attributes>(keyIdent, containers.Current_FuncDef_Attributes));
                    

                }
                else
                {

                   
                    
                        containers.List_of_FunctionDefinitions.Add(new KeyValuePair<string, FuncDef_Attributes>(keyIdent, containers.Current_FuncDef_Attributes));
                    
                } 
            }
           if(containers.Current_FuncDef_Attributes.argumType != null) containers.Current_FuncDef_Attributes.argumType.List_of_types.Reverse();
           
            containers.Current_TypeAttribute = new TypeAttribute("void");

            if (containers.Current_FuncDef_Attributes.argumType != null)
            {
                containers.Current_FuncDef_Attributes.num_of_general_args = containers.Current_FuncDef_Attributes.argumType.List_of_types.Where(p => p.Name == "general").Count(); 
            }

            OutAFunction(node);



        }


        public override void takeActionForUndecleredIdentExpression(TIdentifier ident)
        {
            containers.Current_FuncDef_Attributes.rightval_var_undeclared.Add(ident.Text);
            containers.Current_TypeAttribute = new TypeAttribute("general");
        }













        public override void CaseAArgum(AArgum node)
        {

            containers.Current_TypeAttribute = null;

            SymbolAttributes arg_symAttr = new SymbolAttributes();
            arg_symAttr.Type = new TypeAttribute("general");



            if (node.GetIdentifier() != null)
            {


                node.GetIdentifier().Apply(this);
                TIdentifier ident = node.GetIdentifier();
                string identText = ident.Text;

                if (containers.Current_FuncDef_Attributes.Argums_List.ContainsKey(identText))
                {
                    new Exception("There is argument redeclared in function definition at line :" + ident.Line + " and position :" + ident.Pos);
                }

                containers.Current_TypeAttribute = new TypeAttribute("general");




                if (node.GetValue() != null)
                {

                    node.GetValue().Apply(this);
                    arg_symAttr.Type = containers.Current_TypeAttribute;
                    arg_symAttr.Value = containers.Current_ValueAttribute;

                }

                containers.Current_FuncDef_Attributes.Argums_List.Add(identText, arg_symAttr);
                containers.SymbolTable.put(identText, arg_symAttr);


                //if (containers.Current_TypeAttribute.GetType() == typeof(ArgumTypeAttribute))
                //{
                //    ((ArgumTypeAttribute)containers.Current_TypeAttribute).count++;
                //}


            }
            if (node.GetArgum() != null)
            {
                node.GetArgum().Apply(this);
            }
            else
            {
                containers.Current_TypeAttribute = new ArgumTypeAttribute();
            }

            ArgumTypeAttribute tempArgumAttribute = (ArgumTypeAttribute)containers.Current_TypeAttribute;
           // tempArgumAttribute.count++;
            tempArgumAttribute.List_of_types.Add(arg_symAttr.Type);
            
            


        }


        #region operations

        public override void CaseAMultExpression(AMultExpression node)
        {
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            InAMultExpression(node);

            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                leftType = containers.Current_TypeAttribute;
                if (leftType.Name != "number" || leftType.Name != "general") throw new Exception("Numbers must in be operands of a multiplication");
            }
            else { throw new Exception("There isn't left Expression in a Multiplication"); }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (rightType.Name != "number" || leftType.Name != "general") throw new Exception("Numbers must in be operands of a multiplication");
            }
            else { throw new Exception("There isn't right Expression in a Multiplication"); }

            if (rightType.Name == "general") containers.Current_TypeAttribute = leftType;
            else containers.Current_TypeAttribute = rightType;

            OutAMultExpression(node);
        }

        public override void CaseADivExpression(ADivExpression node)
        {
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            InADivExpression(node);

            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                leftType = containers.Current_TypeAttribute;
                if (leftType.Name != "number" || leftType.Name !="general") throw new Exception("Numbers must in be operands of a division");
            }
            else { throw new Exception("There isn't left Expression in a Division"); }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (rightType.Name != "number"|| leftType.Name !="general") throw new Exception("Numbers must in be operands of a division");
            }
            else { throw new Exception("There isn't right Expression in a Division"); }

            if (rightType.Name == "general") containers.Current_TypeAttribute = leftType;
            else containers.Current_TypeAttribute = rightType;

            OutADivExpression(node);
            

        }



        public override void CaseAMinusExpression(AMinusExpression node)
        {
            InAMinusExpression(node);
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;

            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);

                leftType = containers.Current_TypeAttribute;
                if (!(leftType.Name == "number" || leftType.Name == "general")) throw new Exception("Minus operands must be numbers or strings at node: " + node.GetLeft().ToString());
            }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
                if (!(rightType.Name == "number" ||  rightType.Name == "general")) throw new Exception("Minus operands must be numbers or strings");
            }
            if ((rightType.Name == "string" && leftType.Name == "number") || (rightType.Name == "number" && leftType.Name == "string")) throw new Exception("Minus operands must have the some type");

            if (rightType.Name == "general") containers.Current_TypeAttribute = leftType;
            else containers.Current_TypeAttribute = rightType;
            OutAMinusExpression(node);
        }

        public override void CaseAPlusExpression(APlusExpression node)
        {
            InAPlusExpression(node);
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            
            //if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);

                leftType = containers.Current_TypeAttribute;
                if (!( leftType.Name == "number" || leftType.Name == "string" || leftType.Name == "general")) throw new Exception("Plus operands must be numbers or strings at node: " + node.GetLeft().ToString());
            }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);

                rightType = containers.Current_TypeAttribute;
                if (!(rightType.Name == "number" || rightType.Name == "string" || rightType.Name == "general")) throw new Exception("Plus operands must be numbers or strings");
            }
            if ((rightType.Name == "string" && leftType.Name == "number") || (rightType.Name == "number" && leftType.Name == "string")) throw new Exception("Plus operands must have the some type");

            if (rightType.Name == "general") containers.Current_TypeAttribute = leftType;
            else containers.Current_TypeAttribute = rightType;
            OutAPlusExpression(node);
        }


        #endregion



        public override void CaseAExpressionCComparison(AExpressionCComparison node)
        {
            TypeAttribute rightType = null;
            TypeAttribute leftType = null;
            InAExpressionCComparison(node);
            if (node.GetLeft() != null)
            {
                node.GetLeft().Apply(this);
                leftType = containers.Current_TypeAttribute;
            }
            if (node.GetRight() != null)
            {
                node.GetRight().Apply(this);
                rightType = containers.Current_TypeAttribute;
            }

            if (!(rightType.Name == leftType.Name || rightType.Name == "general" || leftType.Name== "general")) throw new Exception("Comparison operands must have the some type");
            else containers.Current_TypeAttribute = new TypeAttribute("boolean");
            OutAExpressionCComparison(node);
        }






    }







}
