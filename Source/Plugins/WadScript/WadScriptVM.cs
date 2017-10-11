using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.WadScript
{
    // this runs compiled wadscript by calling
    // the methods to interact with DB itself
    // as well as through the ScriptMode class
    // the actual compilation to bytecode
    // happens in Compiler

    public class WadScriptVM
    {
        protected ScriptMode mode;

        protected List<WSInstruction> executable;
        protected List<int> arguments;
        protected List<string> stringTable;
        protected List<int> linenumbers;
        internal List<string> lines;

        public DrawnVertex origin;
        public Vector2D cursor;
        public bool snaptogrid;
        public bool snaptonearest;

        public List<DrawnVertex> points;
        public float stitchrange;
        public bool scaletorenderer;

        public int instruction_ptr;
        public int instruction_count;

        public bool comparison_flag;

        public Stack<int> stack;

        public WadScriptVM(ScriptMode nmode)
        {
            mode = nmode;

            executable = new List<WSInstruction>();
            arguments = new List<int>();
            stringTable = new List<string>();
            linenumbers = new List<int>();
            stack = new Stack<int>(128);
            lines = new List<string>();


            Compiler compiler = new Compiler();
            compiler.CompileFromFilename(Path.Combine(General.SettingsPath,@"scripts\test.wadscript"), this);
            //compiler.CompileFromFilename(Path.Combine(General.SettingsPath, @"scripts\orderofoperations.wadscript"), this);
            //InsertTestProgram();
        }

        public void InsertTestProgram()
        {
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("TESTING SCRIPT LOG")); // 0
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("TESTING SCRIPT LOG2")); // 1
            StringToTableIndex("BLORP");
            StringToTableIndex("ASDFASDFASDF");
            AddInstruction(WSInstruction.LOG_IMMEDIATE, 0); // = StringToTableIndex("TESTING SCRIPT LOG") // 2
            AddInstruction(WSInstruction.NOP, 0); // 3
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("TESTING SCRIPT LOG63")); // 4
            AddInstruction(WSInstruction.LOGSTATE); // 5
            AddInstruction(WSInstruction.PUSH, StringToTableIndex("PUSH POP TEST FAILED IF THIS GETS LOGGED")); // 6
            AddInstruction(WSInstruction.PUSH, StringToTableIndex("PUSH POP TEST 2")); // 7
            AddInstruction(WSInstruction.PUSH, StringToTableIndex("PUSH POP TEST FAILED IF THIS GETS LOGGED")); // 8
            AddInstruction(WSInstruction.PUSH, 77); // 9
            AddInstruction(WSInstruction.POP, 0); // 10
            AddInstruction(WSInstruction.PUSH, StringToTableIndex("PUSH POP TEST 1")); // 11
            AddInstruction(WSInstruction.LOG_STACK); // 12
            AddInstruction(WSInstruction.POP, 0); // 13
            AddInstruction(WSInstruction.LOG_STACK); // 14
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("doing JMP test")); // 15
            AddInstruction(WSInstruction.JMP_IMMEDIATE, 3); //16
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("JMP TEST FAILED")); // 17
            AddInstruction(WSInstruction.TERMINATE); // 18 // skipped
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("JMP TEST PASS")); // 19

            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("doing loop test")); // 15

            AddInstruction(WSInstruction.PUSH, 0); // counter

            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("loop iteration.."));
            AddInstruction(WSInstruction.LOG_PEEK_STACK_INT, 0);

            AddInstruction(WSInstruction.ADD_IMM, 1);
            AddInstruction(WSInstruction.CMP_S_LT_I, 10);
            AddInstruction(WSInstruction.JMP_IF_IMM, -4);

            AddInstruction(WSInstruction.CMP_S_EQ_I, 10);
            AddInstruction(WSInstruction.JMP_IF_IMM, 3);
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("LOOP TEST FAILED"));
            AddInstruction(WSInstruction.JMP_IMMEDIATE, 2);
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("LOOP TEST SUCCESS"));

            

            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("testing over"));
            AddInstruction(WSInstruction.TERMINATE);
            AddInstruction(WSInstruction.LOG_IMMEDIATE, StringToTableIndex("UNREACHABLE CODE DETECTED"));
        }

        public void AddInstruction(WSInstruction ins, int arg = 0)
        {
            AddInstruction(-executable.Count, ins, arg);
        }

        public void AddInstruction(int ln, WSInstruction ins, int arg = 0)
        {
            linenumbers.Add(ln);
            executable.Add(ins);
            arguments.Add(arg);
        }

        public int StringToTableIndex(string ins)
        {
            int count = stringTable.Count;
            stringTable.Add(ins);
            return count;
        }

        public void ResetState()
        {
            points = new List<DrawnVertex>();
            snaptogrid = true;
            snaptonearest = false;
            scaletorenderer = true;
            stitchrange = BuilderPlug.Me.StitchRange;
            instruction_ptr = 0;
            instruction_count = 0;
            comparison_flag = false;
        }

        public void RunVM(bool ui_snaptogrid, bool ui_snaptonearest, Vector2D mappos)
        {
            ResetState();

            snaptogrid = ui_snaptogrid;
            snaptonearest = ui_snaptonearest;

            origin = mode.GetVertexAt(mappos, snaptonearest, snaptogrid, stitchrange, scaletorenderer);
            cursor = origin.pos;

            if (executable.Count != linenumbers.Count
                || executable.Count != arguments.Count)
            {
                VMError("mismatch between executable / linenumbers / args counts!");
            }

            bool bContinueExecuting = true;
            while(bContinueExecuting && instruction_ptr >= 0)
            {
                if (instruction_ptr >= executable.Count)
                {
                    bContinueExecuting = false;
                    break;
                }

                instruction_count++;
                if (instruction_count > 32768)
                {
                    VMError("instruction_count too high, runaway code execution!");
                    bContinueExecuting = false;
                    break;
                }

                switch (executable[instruction_ptr])
                {
                    case WSInstruction.NOP:
                        break;

                    // DEBUGGING
                    case WSInstruction.LOG_IMMEDIATE:
                        LogFromStringTable(arguments[instruction_ptr]);
                        break;
                    case WSInstruction.LOGSTATE:
                        {
                            LogExecutionState();
                        }
                        break;
                    case WSInstruction.LOG_STACK:
                        {
                            int arg = Pop();
                            LogFromStringTable(arg);
                        }
                        break;
                    case WSInstruction.LOG_PEEK_STACK_INT:
                        {
                            Logger.WriteLogLine("WadScript: LOG_PEEK_STACK_INT: " + Peek());
                        }
                        break;


                    case WSInstruction.TERMINATE:
                        bContinueExecuting = false;
                        break;

                        // STACK
                    case WSInstruction.PUSH:
                        Push(arguments[instruction_ptr]);
                        break;
                    case WSInstruction.POP:
                        Pop();
                        break;

                        // JMPs
                    case WSInstruction.JMP_IMMEDIATE:
                        Jmp(instruction_ptr + arguments[instruction_ptr]);
                        continue;
                    case WSInstruction.JMP_STACK:
                        Jmp(Pop());
                        continue;
                    case WSInstruction.JMP_IF_IMM:
                        if (comparison_flag)
                        {
                            Jmp(instruction_ptr + arguments[instruction_ptr]);
                            continue;
                        }
                        break; // otherwise do nothing

                    // CMPs
                    case WSInstruction.CMP_S_LT_I:
                        comparison_flag = Peek() < arguments[instruction_ptr];
                        break;

                    case WSInstruction.CMP_S_EQ_I:
                        comparison_flag = Peek() == arguments[instruction_ptr];
                        break;

                    // ARITHMETIC
                    case WSInstruction.ADD_IMM:
                        {
                            Push(Pop() + arguments[instruction_ptr]);
                        }
                        break;
                    default:
                        throw new Exception("UNKNOWN OPCODE " + executable[instruction_ptr]);
                }

                instruction_ptr++;
            }

            /*
            DrawPoint();

            scaletorenderer = false;
            stitchrange = 4f;

            cursor += new Vector2D(0f, 128f);
            DrawPoint();
            cursor += new Vector2D(128f, 0f);
            DrawPoint();
            cursor += new Vector2D(0f, -128f);
            DrawPoint();
            cursor += new Vector2D(-128f, 0f);
            DrawPoint();

            mode.FinishDrawingPoints(points);
            */
        } // runvm

        private void Push(int arg)
        {
            stack.Push(arg);
        }

        private int Pop()
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            else
            {
                VMError("attempted pop while stack is empty");
                return 0;
            }
        }

        private int Peek()
        {
            if (stack.Count > 0)
            {
                return stack.Peek();
            }
            else
            {
                VMError("attempted peek while stack is empty");
                return 0;
            }
        }

        private void Jmp(int tgt)
        {
            if (tgt >= 0 && tgt < executable.Count)
            {
                instruction_ptr = tgt;
            }
            else
            {
                VMError("jmp to out of bounds location " + tgt);
            }
        }

        private void VMError(string message)
        {
            Logger.WriteLogLine("WARNING: VM ERROR");
            LogExecutionState();
            ScriptError("VM ERROR: " + message);
        }

        private void ScriptError(string message)
        {
            StringBuilder build = new StringBuilder();
            build.Append("WadScript: Runtime Error: ");
            build.Append(message);
            build.Append(": at line ");
            if (instruction_count < linenumbers.Count)
            {
                build.Append(linenumbers[instruction_count]);
            }
            else
            {
                build.Append("???");
            }
            build.Append(": ");
            // TODO put script text here
            Logger.WriteLogLine(build.ToString());
            instruction_ptr = -1;
        }

        private void LogFromStringTable(int str)
        {
            if (stringTable.Count <= str)
            {
                ScriptError("VM ERROR: string is missing from stringtable!");
            }
            else
            {
                Logger.WriteLogLine("WadScript: " + stringTable[str]);
            }
        }

        private void LogExecutionState()
        {
            Logger.WriteLogLine("WadScript: write VM state!");
            if (instruction_ptr < executable.Count)
            {
                Logger.WriteLogLine("WadScript: instruction: " + executable[instruction_ptr].ToString());
            }
            else
            {
                Logger.WriteLogLine("WadScript: instruction_ptr is out of bounds for exec!");
            }
            if (instruction_ptr < arguments.Count)
            {
                Logger.WriteLogLine("WadScript: argument: " + arguments[instruction_ptr]);
            }
            else
            {
                Logger.WriteLogLine("WadScript: instruction_ptr is out of bounds for arg!");
            }
            if (instruction_ptr < linenumbers.Count)
            {
                Logger.WriteLogLine("WadScript: linenumber: " + linenumbers[instruction_ptr]);
            }
            else
            {
                Logger.WriteLogLine("WadScript: instruction_ptr is out of bounds for linenumbers!");
            }
            Logger.WriteLogLine("WadScript: instruction_ptr: " + instruction_ptr);
            Logger.WriteLogLine("WadScript: instruction_count: " + instruction_count);
            Logger.WriteLogLine("WadScript: executable.Count: " + executable.Count);
            Logger.WriteLogLine("WadScript: arguments.Count: " + arguments.Count);
            Logger.WriteLogLine("WadScript: linenumbers.Count: " + linenumbers.Count);
            Logger.WriteLogLine("WadScript: stringTable.Count: " + stringTable.Count);

            Logger.WriteLogLine("WadScript: origin.pos: " + origin.pos);
            Logger.WriteLogLine("WadScript: cursor: " + cursor);
            Logger.WriteLogLine("WadScript: snaptogrid: " + snaptogrid);
            Logger.WriteLogLine("WadScript: snaptonearest: " + snaptonearest);

            Logger.WriteLogLine("WadScript: points.Count " + points.Count);
            Logger.WriteLogLine("WadScript: stitchrange: " + stitchrange);
            Logger.WriteLogLine("WadScript: scaletorenderer: " + scaletorenderer);
        }

        public bool DrawPoint()
        {
            DrawnVertex vertex = mode.GetVertexAt(cursor,
                snaptonearest, snaptogrid,
                stitchrange, scaletorenderer,
                points);

            Logger.WriteLogLine("write point @ " + vertex.pos);
            return DrawPointAt(vertex);
        }

        // This draws a point at a specific location
        public bool DrawPointAt(DrawnVertex vertex)
        {
            if (vertex.pos.x < General.Map.Config.LeftBoundary || vertex.pos.x > General.Map.Config.RightBoundary ||
                vertex.pos.y > General.Map.Config.TopBoundary || vertex.pos.y < General.Map.Config.BottomBoundary)
                return false;

            points.Add(vertex);

            return true;
        }


        


    } //vm class
} // ns
