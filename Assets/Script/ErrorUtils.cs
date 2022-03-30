using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CardGameEngine;
using CardGameEngine.GameSystems.Effects;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using Sentry;
using TMPro;
using UnityEngine;

namespace Script
{
    public class ErrorUtils : MonoBehaviour
    {
        private const string ScriptError = "<color=\"red\">[Erreur de script]</color> : ";
        public TMP_InputField textZone;
        public ConcurrentQueue<Exception> toPrint = new ConcurrentQueue<Exception>();

        private EventDisplayer _eventDisplayer;

        private void Start()
        {
            _eventDisplayer = GetComponent<EventDisplayer>();
            textZone.gameObject.SetActive(false);
        }

        private void Update()
        {
            // if (UnityEngine.Input.GetKeyDown(KeyCode.RightControl))
            //     textZone.gameObject.SetActive(!textZone.gameObject.activeSelf);

            while (toPrint.TryDequeue(out var res))
            {
                var content = res switch
                {
                    LuaException lex => PrintError(lex),
                    InterpreterException iex => PrintError(iex),
                    _ => null
                };

                SentrySdk.CaptureException(res, s =>
                {
                    s.AddBreadcrumb(content ?? string.Empty, "lua_error");
                    s.AddBreadcrumb(res.GetType().Name, "exception_type");
                });
                textZone.text = content;
                textZone.gameObject.SetActive(true);
            }
        }

        public string PrintError(InterpreterException exc, [CanBeNull] string sourceContent = null)
        {
            return PrintError(exc, exc.CallStack?.ToList() ?? new List<WatchItem>(), sourceContent);
        }

        public string PrintError(InterpreterException exception, List<WatchItem> callstack,
            [CanBeNull] string source = null)
        {
            var errorText = new StringBuilder();
            var splitted = exception.DecoratedMessage?.Split(':').ToList();
            var scriptName = splitted == null ? "(aucun message)" : string.Join(":", splitted.GetRange(0, splitted.Count - 2));
            var msg = exception.Message;
            errorText.Append($"[Erreur de script] : <color=\"blue\">{scriptName}</color> -> <u>{msg}</u>\n" + "\n");
            for (var index = 0; index < callstack.Count; index++)
            {
                var watchItem = callstack[index];
                var text =
                    $"[Erreur de script] : Dans <color=\"blue\">{watchItem.Name}</color> <u>{FormatSourceLocation(watchItem.Location)}</u>";
                if (watchItem.Location is {IsClrLocation: false} && (!watchItem.Name?.StartsWith("<") ?? true))
                    text += " : " + ColoredSource(scriptName, text.Length - ScriptError.Length - 2, watchItem.Location,
                        index == 0,
                        source ?? File.ReadAllText(Application.streamingAssetsPath + "/EffectsScripts/Card/" +
                                                   scriptName + ".lua")) + "\n";

                errorText.Append(text + "\n");
            }

            DumpEvents();
            return errorText.ToString();
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

        private string ColoredSource(string scriptName, int padding, SourceRef watchItemLocation, bool isError,
            string scriptContent)
        {
            var isMultiLine = watchItemLocation.FromLine != watchItemLocation.ToLine && watchItemLocation.ToChar != 0;

            var strings = scriptContent.Split('\n');
            var firstLine = strings[watchItemLocation.FromLine - 1];
            var lastLine = strings[watchItemLocation.ToLine - 1];

            var accumulator = "";

            if (!isMultiLine)
            {
                var before = Console.ForegroundColor;
                for (var i = 0; i < firstLine.Length; i++)
                {
                    if (i == watchItemLocation.FromChar)
                        accumulator += "<color=\"red\">";
                    else if (watchItemLocation.ToChar == 0 && i == firstLine.Length - 1 ||
                             watchItemLocation.ToChar != 0 && i == watchItemLocation.ToChar)
                        accumulator += "</color>";

                    accumulator += firstLine[i];
                }

                return accumulator;
            }

            accumulator += "|";
            foreach (var t in firstLine) accumulator += t;

            accumulator += "\n";

            accumulator += ScriptError;
            for (var i = 0; i < padding; i++) accumulator += " ";

            accumulator += "\n|";
            accumulator += "...\n";

            accumulator += ScriptError;
            for (var i = 0; i < padding; i++) accumulator += " ";

            accumulator += "\n|";
            foreach (var t in lastLine) accumulator += t;

            return accumulator;
        }

        private string FormatSourceLocation(SourceRef location)
        {
            if (location == null) return "";

            return $"({location.FromLine},{location.FromChar})-({location.ToLine},{location.ToChar})";
        }

        public string PrintError(LuaException exception)
        {
            return PrintError(exception.RuntimeException, exception.CallStack ?? new List<WatchItem>());
        }

        public void PrintError(InvalidEffectException exception)
        {
            textZone.text += $"<color=\"red\">[Effet invalide]: {exception.Message}</color>" + "\n";
            if (exception.InnerException != null)
                PrintError(exception.InnerException, exception.EffectContent);
            else
                DumpEvents();
        }
    }
}