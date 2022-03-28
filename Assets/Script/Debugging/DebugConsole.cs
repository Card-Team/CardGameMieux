using System.Threading;
using UnityEngine;

namespace Script.Debugging
{
    public class DebugConsole : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        private static string myLog = "";
        private string output;
        private string stack;

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= Log;
        }

        private void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
            }
        }


        public void Init()
        {
            // Application.logMessageReceived += Log;
            Application.logMessageReceivedThreaded += Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            Monitor.Enter(this);
            try
            {
                output = logString;
                stack = stackTrace;
                myLog = myLog + "\n" + output;
                if (myLog.Length > 5000) myLog = myLog.Substring(0, 4000);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
        //#endif
    }
}