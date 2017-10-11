using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace CodeImp.DoomBuilder.WadScript
{
    public class Compiler
    {

        public List<string> tokens;
        public List<int> tokenLineNumbers;
        public List<string> lines;
        public List<TokenType> tokenTypes;

        public IdentifierNode identifierTree;

        public SyntaxNode syntaxtree; // root node

        public int syntax_index;

        public StringBuilder lineBuilder;
        public StringBuilder tokenBuilder;
        public WadScriptVM vm;

        public Compiler()
        {
            tokens = new List<string>();
            tokenLineNumbers = new List<int>();
            tokenTypes = new List<TokenType>();
            lines = new List<string>();
            lines.Add("null placeholder first line - report a bug if you see this somehow");
            identifierTree = new IdentifierNode();
        }

        private void Error(string message, int linenumber = -1, bool warning = false)
        {
            StringBuilder build = new StringBuilder();
            if (warning)
            {
                build.Append("WadScript: Compile Warning: ");
            }
            else
            {
                build.Append("WadScript: Compile Error: ");
            }
            build.Append(message);
            build.Append(": at line ");

            if (linenumber > 0) // 0 not valid
            {
                build.Append(linenumber);
                if (linenumber < lines.Count)
                {
                    build.Append(": ");
                    build.Append(lines[linenumber].Trim());
                }
                else if(linenumber < lines.Count + 1 && linenumber > 1)
                {
                    build.Append(": current partial line is: ");
                    build.Append(lineBuilder.ToString().Trim());
                }
            }
            else
            {
                build.Append("???");
            }
            // TODO put script text here
            Logger.WriteLogLine(build.ToString());
        }

        private void TokenizerError(string message, int charnumber = -1, int linenumber = - 1)
        {
            if (charnumber > -1)
            {
                message += ": near column " + charnumber + " in partial token `" + tokenBuilder.ToString() + "`";
            }
            Error(message, linenumber);
        }

        private void SyntaxError(string message, bool warning = false, int index = -1)
        {
            if (index == -1)
            {
                index = syntax_index;
            }
            if (index > 0 && index < tokenLineNumbers.Count)
            {
                Error(message + ": at token `" + tokens[index] + "`", tokenLineNumbers[index], warning);
            }
            else
            {
                Error(message, -1, warning);
            }
        }

        // returns false on error
        public bool CompileFromFilename(string filename, WadScriptVM invm)
        {
            if (!File.Exists(filename))
            {
                Logger.WriteLogLine("WadScript.Compiler: attempt to compile nonexistent file " + filename);
                return false;
            }
            
            return CompileFromCharArray(File.ReadAllText(filename).ToCharArray(), invm);
        }

        // returns false on error
        public bool CompileFromCharArray(char[] infile, WadScriptVM invm)
        {
            vm = invm;
            if (!Tokenize(infile))
            {
                Error("Unknown Tokenizer Error (sorry)");
                return false;
            }

            vm.lines = lines;

            for (int i = 0; i < tokens.Count; i++)
            {
                Logger.WriteLogLine(tokenTypes[i].ToString() + " token \"" + tokens[i] + "\" at line number " + tokenLineNumbers[i]);
            }

            SyntaxGen();

            Logger.WriteLogLine(DebugPrintParseTree(string.Empty,syntaxtree, 0));
            
            return true;
        }

        public string DebugPrintParseTree(string input, SyntaxNode node, int level)
        {
            string valuestring = node.type == SyntaxType.FloatLiteral ? node.floatvalue.ToString() +"f" : node.value.ToString();
            string operatorstring = node.operator_precedence != 0 ? node.operator_precedence.ToString() : string.Empty;
            string scopestring = " (null)";

            if (node.scope != null)
            {
                scopestring = " (" + node.scope.scope_text_id + ")";
            }

            input += tokenLineNumbers[node.tokenindex].ToString();
            input += new String(' ', Math.Max(2 + (level*4) - tokenLineNumbers[node.tokenindex].ToString().Length,1))
                + node.type.ToString()
                + operatorstring
                + scopestring
                + ": "
                + new String(' ', Math.Max(70 - node.type.ToString().Length - operatorstring.Length - scopestring.Length - level * 4, 1))
                + valuestring
                + new String(' ', Math.Max(16 - valuestring.Length, 1))
                + "`"
                + tokens[node.tokenindex]
                + "` ;\n";

            if (node.children != null)
            {
                //Logger.WriteLogLine("ASDF " + node.children.Count);
                foreach (SyntaxNode p in node.children)
                {
                    input = DebugPrintParseTree(input, p, level + 1);
                }
            }
            return input;
        }

        public bool SyntaxGen()
        {
            int token_count = tokens.Count;
            syntax_index = 0;
            syntaxtree = new SyntaxNode();
            syntaxtree.type = SyntaxType.Root;
            syntaxtree.tokenindex = 0;
            syntaxtree = SyntaxBlock(syntaxtree, token_count, identifierTree);
            return true;
        }

        public SyntaxNode SyntaxBlock(SyntaxNode node, int token_count, IdentifierNode scope)
        {
            if (node.children == null)
            {
                node.children = new List<SyntaxNode>();
            }
            node.scope = scope;

            while (syntax_index < token_count)
            {
                //Logger.WriteLogLine("w" + tokenLineNumbers[syntax_index] + ", " + syntax_index + ": " + tokens[syntax_index] + " " + node.value + " " + node.type);
                bool bReturnAfterAdding = false;

                SyntaxNode child = new SyntaxNode();

                child.tokenindex = syntax_index;
                ChildSyntaxPlacement placement = ChildSyntaxPlacement.Normal;

                if (tokenTypes[syntax_index] == TokenType.Name)
                {
                    // can be type
                    if (CompilerConstants.Types.Contains(tokens[syntax_index]))
                    {
                        if (node.type == SyntaxType.BaseDeclaration
                            || node.type == SyntaxType.ArgDeclaration
                            || node.type == SyntaxType.FuncDeclaration
                            || node.type == SyntaxType.VarDeclaration)
                        {
                            child.type = SyntaxType.Type;
                            child.value = CompilerConstants.Types.ToList().IndexOf(tokens[syntax_index]); // FIXME THIS IS SLOW
                        }
                        else
                        {
                            if (node.type == SyntaxType.Root)
                            {
                                child.type = SyntaxType.BaseDeclaration;
                            }
                            else if (node.type == SyntaxType.ParensBlock)
                            {
                                child.type = SyntaxType.ArgDeclaration;
                            }
                            else
                            {
                                // no func declarations outside of root scope
                                child.type = SyntaxType.VarDeclaration;
                            }

                            child = SyntaxBlock(child, token_count, scope);
                        }
                    }
                    else if (CompilerConstants.FlowControl.Contains(tokens[syntax_index]))
                    {
                        child.type = SyntaxType.FlowControl;
                        child.value = CompilerConstants.FlowControl.ToList().IndexOf(tokens[syntax_index]); // FIXME THIS IS SLOW

                        if (node.type == SyntaxType.BaseDeclaration
                            || node.type == SyntaxType.VarDeclaration
                            || node.type == SyntaxType.FuncDeclaration
                            || node.type == SyntaxType.ArgDeclaration)
                        {
                            SyntaxError("error in declaration, reserved flow control keyword `" + tokens[syntax_index] + "` can't be used in declaration");
                        }

                        syntax_index++;
                        child = SyntaxBlock(child, token_count, scope.AddNewChild(syntax_index-1));
                    }
                    else
                    {
                        if (node.type == SyntaxType.BaseDeclaration
                            || node.type == SyntaxType.VarDeclaration
                            || node.type == SyntaxType.FuncDeclaration
                            || node.type == SyntaxType.ArgDeclaration)
                        {
                            if (scope.IsInScope(tokens[syntax_index]))
                            {
                                SyntaxError("Identifier `" + tokens[syntax_index] + "` already declared in this scope, can't redeclare/shadow!");
                            }
                            else
                            {
                                child.type = SyntaxType.Identifier;
                                scope.identifierDict.Add(tokens[syntax_index], 0);
                            }
                        }
                        else if (node.type == SyntaxType.Block)
                        {
                            child.type = SyntaxType.Statement;
                            child = SyntaxBlock(child, token_count, scope);
                        }
                        else
                        {
                            child.type = SyntaxType.Identifier;
                        }
                    }
                } // name
                else if (tokenTypes[syntax_index] == TokenType.Symbol)
                {
                    switch (tokens[syntax_index])
                    {
                        case "*":
                        case "+":
                        case "-":
                        case "/":
                        case "%":
                        case "<":
                        case ">":
                        case "==":
                        case "<=":
                        case ">=":
                            if (node.type == SyntaxType.BaseDeclaration
                                || node.type == SyntaxType.VarDeclaration
                                || node.type == SyntaxType.FuncDeclaration
                                || node.type == SyntaxType.ArgDeclaration)
                            {
                                SyntaxError("unexpected binary operator `" + tokens[syntax_index] + "` in declaration");
                            }
                            else if (node.type == SyntaxType.Block)
                            {
                                SyntaxError("unexpected binary operator `" + tokens[syntax_index] + "` without statement (you probably don't have anything it goes with ?)");
                            }
                            child.type = SyntaxType.BinaryOperator;
                            child.operator_precedence = CompilerConstants.OperatorPrecedence[tokens[syntax_index]];

                            /*
                            if (node.type == SyntaxType.RightHandSide && child.operator_precedence < node.operator_precedence)
                            {
                                return node;
                            }

                            placement = ChildSyntaxPlacement.BinaryOperatorPlacement;
                            */
                            break;
                        case ",":
                            if (node.type == SyntaxType.BaseDeclaration
                                || node.type == SyntaxType.VarDeclaration
                                || node.type == SyntaxType.FuncDeclaration)
                            {
                                SyntaxError("unexpected separator `" + tokens[syntax_index] + "` in declaration");
                            }
                            // FIXME || node.type == SyntaxType.ArgDeclaration

                            child.type = SyntaxType.Comma;
                            break;
                        case ";":
                            if (node.type == SyntaxType.FuncDeclaration
                                || node.type == SyntaxType.ArgDeclaration)
                            {
                                SyntaxError("unexpected `;` ender in function declaration");
                            }
                            else if (node.type == SyntaxType.Assignment)
                            {
                                return node;
                            }
                            else if (node.type == SyntaxType.BaseDeclaration
                                || node.type == SyntaxType.VarDeclaration
                                || node.type == SyntaxType.Statement)
                                //|| node.type == SyntaxType.RightHandSide)
                            {
                                if (node.type == SyntaxType.BaseDeclaration)
                                {
                                    node.type = SyntaxType.VarDeclaration;
                                }
                                //syntax_index++;
                                return node;
                            }
                            child.type = SyntaxType.Semicolon;
                            break;
                        case "=":
                            if (node.type == SyntaxType.Assignment)
                            {
                                SyntaxError("multiple `=` assignments unsupported");
                            }
                            else if (node.type == SyntaxType.FuncDeclaration
                                || node.type == SyntaxType.ArgDeclaration) // TODO - support default arguments
                            {
                                SyntaxError("unexpected assignment `=` in function declaration");
                            }
                            else if (node.type == SyntaxType.BaseDeclaration)
                            {
                                node.type = SyntaxType.VarDeclaration;
                            }

                            child.type = SyntaxType.Assignment;
                            syntax_index++;
                            child = SyntaxBlock(child, token_count, scope);
                            if (node.type == SyntaxType.VarDeclaration
                                || node.type == SyntaxType.Statement)
                            {
                                node.children.Add(child);
                                return node;
                            }
                            break;
                        case "{":
                            if (node.type == SyntaxType.VarDeclaration 
                                || node.type == SyntaxType.Assignment
                                || node.type == SyntaxType.ArgDeclaration
                                || node.type == SyntaxType.Statement)
                            {
                                SyntaxError("unexpected `{` in declaration/assignment/statement (did you forget a `;`?)");
                            }

                            if (node.type == SyntaxType.BaseDeclaration)
                            {
                                node.type = SyntaxType.FuncDeclaration;
                            }

                            child.type = SyntaxType.Block;
                            /*if (node.children.Count != 0
                                && node.children[node.children.Count-1].type == SyntaxType.Statement)*/
                                /*
                            if(node.type == SyntaxType.FlowControl
                                || node.type == SyntaxType.FuncDeclaration)
                            {
                                placement = ChildSyntaxPlacement.ChildOfPrevious;
                            }
                            */
                            syntax_index++;
                            child = SyntaxBlock(child, token_count, scope.AddNewChild(syntax_index-1));

                            if (node.type == SyntaxType.FlowControl
                                || node.type == SyntaxType.FuncDeclaration)
                            {
                                bReturnAfterAdding = true;
                            }

                            break;
                        case "(":
                            if (node.type == SyntaxType.ArgDeclaration)
                            {
                                SyntaxError("unexpected `(` in function argument declaration");
                            }
                            else if (node.type == SyntaxType.BaseDeclaration)
                            {
                                node.type = SyntaxType.FuncDeclaration;
                            }

                            child.type = SyntaxType.ParensBlock;
                            placement = ChildSyntaxPlacement.ChildOfPrevious;
                            syntax_index++;
                            child = SyntaxBlock(child, token_count, scope);
                            break;
                        case "[":
                            child.type = SyntaxType.SquareBlock;
                            placement = ChildSyntaxPlacement.ChildOfPrevious;
                            syntax_index++;
                            child = SyntaxBlock(child, token_count, scope);
                            break;
                        case "}":
                            // end block
                            if (node.type != SyntaxType.Block)
                            {
                                if (node.type == SyntaxType.ParensBlock)
                                {
                                    SyntaxError("couldn't find matching closed ')' for open '(' due to unexpected '}' ");
                                }
                                else if (node.type == SyntaxType.SquareBlock)
                                {
                                    SyntaxError("couldn't find matching closed ']' for open '[' due to unexpected '}' ");
                                }
                                else if (node.type == SyntaxType.BaseDeclaration
                                    || node.type == SyntaxType.VarDeclaration
                                    || node.type == SyntaxType.FuncDeclaration
                                    || node.type == SyntaxType.ArgDeclaration
                                    || node.type == SyntaxType.Assignment)
                                {
                                    SyntaxError("unexpected `}` in declaration or assignment (did you forget a `;` maybe?)");
                                }
                                else
                                {
                                    SyntaxError("unexpected closing '}' with no matching '{'");
                                }
                            }

                            return node;
                        case ")":
                            // end parensblock
                            if (node.type != SyntaxType.ParensBlock) //&& node.type != SyntaxType.RightHandSide)
                            {
                                if (node.type == SyntaxType.Block)
                                {
                                    SyntaxError("couldn't find matching closed '}' for open '{' due to unexpected ')' ");
                                }
                                else if (node.type == SyntaxType.SquareBlock)
                                {
                                    SyntaxError("couldn't find matching closed ']' for open '[' due to unexpected ')' ");
                                }
                                else
                                {
                                    SyntaxError("unexpected closing ')' with no matching '('");
                                }
                            }

                            return node;
                        case "]":
                            // end squareblock
                            if (node.type != SyntaxType.SquareBlock)
                            {
                                if (node.type == SyntaxType.Block)
                                {
                                    SyntaxError("couldn't find matching closed '}' for open '{' due to unexpected ']' ");
                                }
                                else if (node.type == SyntaxType.ParensBlock)
                                {
                                    SyntaxError("couldn't find matching closed ')' for open '(' due to unexpected ']' ");
                                }
                                else
                                {
                                    SyntaxError("unexpected closing ']' with no matching '['");
                                }
                            }

                            return node;
                        default:
                            // TODO - might be error?
                            child.type = SyntaxType.Symbol;
                            break;
                    }
                } // symbol
                else if (tokenTypes[syntax_index] == TokenType.IntLiteral)
                {
                    // FIXME error in wrong contexts
                    child.type = SyntaxType.IntLiteral;
                    if (!int.TryParse(tokens[syntax_index], NumberStyles.Integer, CultureInfo.InvariantCulture, out child.value))
                    {
                        SyntaxError("failure to parse integer!");
                    }
                }
                else if (tokenTypes[syntax_index] == TokenType.HexLiteral)
                {
                    // FIXME error in wrong contexts
                    child.type = SyntaxType.IntLiteral;
                    if (!int.TryParse(tokens[syntax_index], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out child.value))
                    {
                        SyntaxError("failure to parse hex integer!");
                    }
                }
                else if (tokenTypes[syntax_index] == TokenType.FloatLiteral)
                {
                    // FIXME error in wrong contexts
                    child.type = SyntaxType.FloatLiteral;
                    if (!float.TryParse(tokens[syntax_index], NumberStyles.Number, CultureInfo.InvariantCulture, out child.floatvalue))
                    {
                        SyntaxError("failure to parse float!");
                    }
                }
                else if (tokenTypes[syntax_index] == TokenType.StringLiteral)
                {
                    // FIXME error in wrong contexts
                    child.type = SyntaxType.StringLiteral;
                    child.value = vm.StringToTableIndex(tokens[syntax_index]);
                    
                    //return node;
                } // string


                if (child.scope == null)
                {
                    child.scope = scope;
                }


                switch (placement)
                {
                    case ChildSyntaxPlacement.ChildOfPrevious:
                        if (node.children.Count == 0)
                        {
                            node.children.Add(child);
                        }
                        else
                        {
                            SyntaxNode prev = node.children[node.children.Count - 1];
                            if (prev.children == null)
                            {
                                prev.children = new List<SyntaxNode>();
                            }
                            prev.children.Add(child);
                            node.children[node.children.Count - 1] = prev;
                        }
                        break;
                        /*
                    case ChildSyntaxPlacement.BinaryOperatorPlacement:
                        {
                            if (node.children.Count <= 0)
                            {
                                SyntaxError("binary operator `" + tokens[syntax_index] + "` with no lefthand side!");
                            }
                            SyntaxNode lhs = new SyntaxNode();
                            lhs.children = node.children;
                            lhs.tokenindex = node.children[0].tokenindex;
                            lhs.type = SyntaxType.LeftHandSide;
                            lhs.operator_precedence = child.operator_precedence;
                            child.children = new List<SyntaxNode>();
                            
                            SyntaxNode rhs = new SyntaxNode();
                            rhs.type = SyntaxType.RightHandSide;

                            syntax_index++;
                            rhs.tokenindex = syntax_index;
                            rhs.operator_precedence = child.operator_precedence;
                            rhs = SyntaxBlock(rhs, token_count, scope);

                            child.children.Add(lhs);
                            child.children.Add(rhs);

                            node.children = new List<SyntaxNode>();
                            node.children.Add(child);
                            return node;
                        }
                        */
                        //break;

                    case ChildSyntaxPlacement.Normal:
                    default:
                        node.children.Add(child);
                        break;
                } // switch placement


                if (bReturnAfterAdding)
                {
                    return node;
                }


                syntax_index++;
            } // while

            if (node.type == SyntaxType.Block)
            {
                SyntaxError("couldn't find matching close '}' for open '{' due to unexpected EOF ", true,node.tokenindex);
            }
            else if (node.type == SyntaxType.ParensBlock)
            {
                SyntaxError("couldn't find matching close ')' for open '(' due to unexpected EOF ", true, node.tokenindex);
            }
            else if (node.type == SyntaxType.SquareBlock)
            {
                SyntaxError("couldn't find matching close ']' for open '[' due to unexpected EOF ", true, node.tokenindex);
            }

            return node;
        }

        public bool Tokenize(char[] infile)
        {
            int linenumber = 1;

            int char_count = infile.Length;
            tokenBuilder = new StringBuilder(64);
            lineBuilder = new StringBuilder(128);

            TokenizerState state = TokenizerState.Default;
            bool bIsInt = true;
            bool bIsHex = false;
            for (int char_index = 0; char_index < char_count; char_index++)
            {
                char c = infile[char_index];
                switch (state)
                {
                    case TokenizerState.Number:
                        {
                            switch (c)
                            {
                                case '\x00':
                                    TokenizerError("unexpected null char in number token ",lineBuilder.Length);
                                    return false;
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    tokenBuilder.Append(c);
                                    lineBuilder.Append(c);
                                    break;
                                case 'x':
                                case 'X':
                                    if (bIsInt && tokenBuilder.Length == 1) // hex
                                    {
                                        bIsHex = true;
                                        //Logger.WriteLogLine("hex detected");
                                        tokenBuilder.Length = 0;
                                        lineBuilder.Append(c);
                                    }
                                    else
                                    {
                                        TokenizerError("unexpected 'x' in number token ", lineBuilder.Length,linenumber);
                                        return false;
                                    }
                                    break;
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                case 'a':
                                case 'b':
                                case 'c':
                                case 'd':
                                case 'e':
                                    if (!bIsHex)
                                    {
                                        TokenizerError("unexpected alphabet char " + c + " in number, if you want hex you need to start it with '0x'", lineBuilder.Length,linenumber);
                                        return false;
                                    }
                                    tokenBuilder.Append(c);
                                    lineBuilder.Append(c);
                                    break;
                                case 'f':
                                case 'F':
                                    if (bIsHex)
                                    {
                                        tokenBuilder.Append(c);
                                        lineBuilder.Append(c);
                                    }
                                    else
                                    {
                                        bIsInt = false;
                                        if (tokenBuilder.Length > 0)
                                        {
                                            tokens.Add(tokenBuilder.ToString());
                                            tokenTypes.Add(TokenType.FloatLiteral);
                                            tokenBuilder.Length = 0;
                                            tokenLineNumbers.Add(linenumber);
                                        }
                                        state = TokenizerState.Default;
                                    }
                                    break;
                                case '.':
                                    if (bIsHex)
                                    {
                                        TokenizerError("unexpected '.' in hex token ", lineBuilder.Length,linenumber);
                                        return false;
                                    }
                                    else if (bIsInt)
                                    {
                                        tokenBuilder.Append(c);
                                        lineBuilder.Append(c);
                                        bIsInt = false;
                                    }
                                    else
                                    {
                                        TokenizerError("unexpected duplicate '.' in float token ", lineBuilder.Length,linenumber);
                                        return false;
                                    }
                                    break;
                                default:
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokens.Add(tokenBuilder.ToString());
                                        if (bIsHex)
                                        {
                                            tokenTypes.Add(TokenType.HexLiteral);
                                        }
                                        else if (bIsInt)
                                        {
                                            tokenTypes.Add(TokenType.IntLiteral);
                                        }
                                        else
                                        {
                                            tokenTypes.Add(TokenType.FloatLiteral);
                                        }
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }
                                    char_index--;
                                    state = TokenizerState.Default;
                                    break;
                            } // inner switch number
                            break;
                        }
                    case TokenizerState.String: // FIXME TODO - add escape characters properly
                        {
                            switch (c)
                            {
                                case '\r':
                                case '\x00':
                                    break;
                                case '\n':
                                    lines.Add(lineBuilder.ToString());
                                    lineBuilder.Length = 0;
                                    //linenumber++;
                                    Error("Multiline strings are not allowed! ", linenumber);
                                    return false;
                                    //break;
                                case '"':
                                    if (char_index > 0 && infile[char_index - 1] != '\\')
                                    {
                                        state = TokenizerState.Default;

                                        if (tokenBuilder.Length > 0)
                                        {
                                            tokens.Add(tokenBuilder.ToString());
                                            tokenTypes.Add(TokenType.StringLiteral);
                                            tokenBuilder.Length = 0;
                                            tokenLineNumbers.Add(linenumber);
                                        }
                                    }
                                    else
                                    {
                                        tokenBuilder.Append(c);
                                    }

                                    lineBuilder.Append(c);
                                    break;
                                default:
                                    tokenBuilder.Append(c);
                                    lineBuilder.Append(c);
                                    break;
                            } // inner switch string
                            break;
                        }
                    case TokenizerState.LineComment:
                        {
                            switch (c)
                            {
                                case '\r':
                                case '\x00':
                                    break;
                                case '\n':
                                    lines.Add(lineBuilder.ToString());
                                    lineBuilder.Length = 0;
                                    linenumber++;
                                    state = TokenizerState.Default;
                                    break;
                                default:
                                    lineBuilder.Append(c);
                                    break;
                            } // inner switch line comment
                            break;
                        }
                    case TokenizerState.BlockComment:
                        {
                            switch (c)
                            {
                                case '\r':
                                case '\x00':
                                    break;
                                case '\n':
                                    lines.Add(lineBuilder.ToString());
                                    lineBuilder.Length = 0;
                                    linenumber++;
                                    break;
                                case '/':
                                    if (char_index > 0)
                                    {
                                        if (infile[char_index - 1] == '*')
                                        {
                                            //Logger.WriteLogLine("end comment @ line " + linenumber);
                                            state = TokenizerState.Default;
                                        }
                                    }

                                    lineBuilder.Append(c);
                                    break;
                                default:
                                    lineBuilder.Append(c);
                                    break;
                            } // inner switch block comment
                            break;
                        }
                    default: // TokenizerState.Default
                        {
                            switch (c)
                            {
                                case '\r':
                                case '\x00':
                                    break;

                                case ' ':
                                case '\t':
                                    // token end without adding this
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenTypes.Add(TokenType.Name);
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }

                                    lineBuilder.Append(c);
                                    break;
                                case '"': // string start
                                    // token end without adding this
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenTypes.Add(TokenType.Name);
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }

                                    lineBuilder.Append(c);
                                    state = TokenizerState.String;
                                    break;
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    // token end and add this char too
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenTypes.Add(TokenType.Name);
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }
                                    state = TokenizerState.Number;
                                    bIsInt = true;
                                    bIsHex = false;
                                    tokenBuilder.Append(c);
                                    lineBuilder.Append(c);
                                    break;
                                case '{':
                                case '}':
                                case '(':
                                case ')':
                                case ',':
                                case ';':
                                case '*':
                                case '+':
                                case '-':
                                case '=':
                                case '!':
                                case '@':
                                case '#':
                                case '$':
                                case '%':
                                case '^':
                                case '&':
                                case '.': // FIXME NUMBERS STARTING WITH '.'
                                case '\\':
                                case '/':
                                case '|':
                                case '>':
                                case '<':
                                case '?':
                                case ':':
                                case '[':
                                case ']':
                                case '~':
                                case '`':
                                    // look ahead check if it's a comment
                                    if (char_index != char_count - 1 && c == '/')
                                    {
                                        if (infile[char_index + 1] == '/')
                                        {
                                            state = TokenizerState.LineComment;
                                        }
                                        else if (infile[char_index + 1] == '*')
                                        {
                                            //Logger.WriteLogLine("start comment @ line " + linenumber);
                                            state = TokenizerState.BlockComment;
                                        }
                                    }
                                    // token end and add this char too
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenTypes.Add(TokenType.Name);
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }

                                    lineBuilder.Append(c);

                                    if (state != TokenizerState.Default)
                                    {
                                        char_index++;
                                    }
                                    else
                                    {
                                        // look ahead for double char symbols like ==
                                        if (char_index != char_count - 1)
                                        {
                                            if ((c == '<' || c == '>' || c == '=')
                                                && infile[char_index + 1] == '=')
                                            {
                                                tokenBuilder.Append(c);
                                                char_index++;
                                                c = infile[char_index];
                                            }
                                        }
                                        tokenBuilder.Append(c);
                                        tokenTypes.Add(TokenType.Symbol);
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }
                                    break;
                                case '\n':
                                    if (tokenBuilder.Length > 0)
                                    {
                                        tokenTypes.Add(TokenType.Name);
                                        tokens.Add(tokenBuilder.ToString());
                                        tokenBuilder.Length = 0;
                                        tokenLineNumbers.Add(linenumber);
                                    }

                                    lines.Add(lineBuilder.ToString());
                                    lineBuilder.Length = 0;
                                    linenumber++;
                                    break;

                                default:
                                    tokenBuilder.Append(c);
                                    lineBuilder.Append(c);
                                    break;
                            } // switch
                            break;
                        } // default:
                } // outer switch
            } // for chars

            if (state == TokenizerState.BlockComment)
            {
                TokenizerError("Unterminated block comment!");
                return false;
            }
            return true;
        } // tokenize
        
    } // class
} //ns
