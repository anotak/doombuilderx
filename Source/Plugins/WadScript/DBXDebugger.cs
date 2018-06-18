using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

// main purpose of this is to prevent infinite loops
// the reason we do it this way in particular is so that
// undos can be properly handled (as it'll quit between API calls)
// partially based on https://gist.github.com/xanathar/2c777a79937398834ad4
// also keep in mind:
// https://github.com/xanathar/moonsharp/blob/master/src/MoonSharp.Interpreter/Debugging/IDebugger.cs
namespace CodeImp.DoomBuilder.DBXLua
{
    class DBXDebugger : IDebugger
    {
        List<DynamicExpression> m_Dynamics = new List<DynamicExpression>();

        public DebuggerCaps GetDebuggerCaps()
        {
            return (DebuggerCaps)0;
        }

        public void SetDebugService(DebugService debugService)
        {

        }

        public void SetSourceCode(SourceCode sourceCode)
        {
        }

        public void SetByteCode(string[] byteCode)
        {
        }

        public bool IsPauseRequested()
        {
            return true;
        }

        public bool SignalRuntimeException(ScriptRuntimeException ex)
        {
            return false;
        }

        public DebuggerAction GetAction(int ip, SourceRef sourceref)
        {
            if (ScriptMode.bScriptCancelled)
            {
                throw new ScriptRuntimeException("Script cancelled by user due to timeout.");
            }

            return new DebuggerAction()
            {
                Action = DebuggerAction.ActionType.StepIn,
            };
        }

        public void SignalExecutionEnded()
        {
        }

        public void Update(WatchType watchType, IEnumerable<WatchItem> items)
        {
        }

        public List<DynamicExpression> GetWatchItems()
        {
            return m_Dynamics;
        }

        public void RefreshBreakpoints(IEnumerable<SourceRef> refs)
        {
        }
    } // class
} // ns
