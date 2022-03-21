using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CardGameEngine;
using CardGameEngine.GameSystems.Effects;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using TMPro;
using UnityEngine;

namespace Script
{
    public class ErrorUtils : MonoBehaviour
    {
        public TMP_InputField textZone;

        private EventDisplayer _eventDisplayer;
        public ConcurrentQueue<LuaException> toPrint = new ConcurrentQueue<LuaException>();

        private void Start()
        {
            _eventDisplayer = GetComponent<EventDisplayer>();
            textZone.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightControl))
            {
                textZone.gameObject.SetActive(!textZone.gameObject.activeSelf);
            }

            while (toPrint.TryDequeue(out var res))
            {
                PrintError(res);
                textZone.gameObject.SetActive(true);
            }
        }

        private const string ScriptError = "<color=\"red\">[Erreur de script]</color> : ";

        public void PrintError(InterpreterException exc, [CanBeNull] string sourceContent = null)
        {
            PrintError(exc, exc.CallStack?.ToList() ?? new List<WatchItem>(),sourceContent);
        }

        public void PrintError(InterpreterException exception, List<WatchItem> callstack,[CanBeNull] string source = null)
        {
            var splitted = exception.DecoratedMessage.Split(':').ToList();
            var scriptName = string.Join(":", splitted.GetRange(0, splitted.Count - 2));
            var msg = exception.Message;
            textZone.text += $"[Erreur de script] : <color=\"blue\">{scriptName}</color> -> <u>{msg}</u>\n" + "\n";
            for (var index = 0; index < callstack.Count; index++)
            {
                var watchItem = callstack[index];
                var text =
                    $"[Erreur de script] : Dans <color=\"blue\">{watchItem.Name}</color> <u>{FormatSourceLocation(watchItem.Location)}</u>";
                if (watchItem.Location is { IsClrLocation: false } && (!watchItem.Name?.StartsWith("<") ?? true))
                {
                    text += " : " + ColoredSource(scriptName, text.Length - ScriptError.Length - 2, watchItem.Location,
                        index == 0, source ?? File.ReadAllText(Application.streamingAssetsPath + "/EffectsScripts/Card/" + scriptName + ".lua")) + "\n";
                }

                textZone.text += text + "\n";
            }

            DumpEvents();
        }

        public void DumpEvents()
        {
            //_eventDisplayer.DumpEvents();
        }

        public void PrintError(InvalidOperationException exception)
        {
            textZone.text += $"<color=\"red\">[Erreur Moteur] : {exception.Message}</color>" + "\n";
            textZone.text += exception + "\n";
            DumpEvents();
        }

        private string ColoredSource(string scriptName, int padding, SourceRef watchItemLocation, bool isError,string scriptContent)
        {

            bool isMultiLine = watchItemLocation.FromLine != watchItemLocation.ToLine && watchItemLocation.ToChar != 0;

            var strings = scriptContent.Split('\n');
            string firstLine = strings[watchItemLocation.FromLine - 1];
            string lastLine = strings[watchItemLocation.ToLine - 1];

            var accumulator = "";

            if (!isMultiLine)
            {
                var before = Console.ForegroundColor;
                for (var i = 0; i < firstLine.Length; i++)
                {
                    if (i == watchItemLocation.FromChar)
                    {
                        accumulator += "<color=\"red\">";
                    }
                    else if (watchItemLocation.ToChar == 0 && i == firstLine.Length - 1 ||
                             watchItemLocation.ToChar != 0 && i == watchItemLocation.ToChar)
                    {
                        accumulator += "</color>";
                    }

                    accumulator += firstLine[i];
                }

                return accumulator;
            }
            else
            {
                accumulator += "|";
                foreach (var t in firstLine)
                {
                    accumulator += t;
                }

                accumulator += "\n";

                accumulator += ScriptError;
                for (var i = 0; i < padding; i++)
                {
                    accumulator += " ";
                }

                accumulator += "\n|";
                accumulator += "...\n";

                accumulator += ScriptError;
                for (var i = 0; i < padding; i++)
                {
                    accumulator += " ";
                }

                accumulator += "\n|";
                foreach (var t in lastLine)
                {
                    accumulator += t;
                }

                return accumulator;
            }
        }

        private string FormatSourceLocation(SourceRef location)
        {
            if (location == null)
            {
                return "";
            }

            return $"({location.FromLine},{location.FromChar})-({location.ToLine},{location.ToChar})";
        }

        public void PrintError(LuaException exception)
        {
            PrintError(exception.RuntimeException, exception.CallStack);
        }

        public void PrintError(InvalidEffectException exception)
        {
            textZone.text += $"<color=\"red\">[Effet invalide]: {exception.Message}</color>" + "\n";
            if (exception.InnerException != null)
                PrintError(exception.InnerException,exception.EffectContent);
            else
                DumpEvents();
        }
    }
}