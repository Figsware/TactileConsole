using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tactile.Console.Commands;
using Tactile.Console.Parameters;
using Tactile.Console.Printing;
using Tactile.Console.Utility;
using UnityEngine;

namespace Tactile.Console.Interfaces
{
    public class IMGUIConsole : Console
    {
        #region Display Settings

        public float CommandBarHeight { get; set; } = 20f;
        public int FontSize { get; set; } = 12;
        public int NumberOfCommandSuggestions { get; set; } = 10;

        #endregion

        public event Action OnRepaint;

        private readonly StringBuilder _buffer = new();
        private string _input;
        private Vector2 _scrollPos;
        private readonly string _inputFieldControlName;
        private readonly ICommandPrinter _printer;
        private readonly BasePrintBuilder _suggestionsBuilder;
        private List<BaseCommand> _suggestions;
        private static Font _monospaceFont;

        private static readonly string[] MonospaceFontNames =
        {
            "JetBrains Mono",
            "Consolas",
            "SF Mono"
        };

        public IMGUIConsole()
        {
            AddConsoleCommand(new Command("clear", "Clears the console", (_, _) => { _buffer.Clear(); }));

            AddConsoleCommand(new Command<int>("fontsize", "Sets the font size for the console",
                new IntegerParameter("fontsize", "The fontsize to set the console to"),
                (_, args) => { FontSize = args.Arg1; }));

            AddConsoleCommand(new Command<bool>("richtext", "Sets whether richtext is enabled",
                new BooleanParameter("enabled", "Whether richtext is enabled"),
                (_, args) => { Format.UseRichText = args.Arg1; }));

            _inputFieldControlName = Guid.NewGuid().ToString();
            OnPrintLine += AddLineToBuffer;

            _suggestions = new List<BaseCommand>();
            _suggestionsBuilder = new RichTextPrintBuilder(new PrintFormat());
            _printer = new CommandPrinter
            {
                PrintDescription = false,
                RecurseIntoSubcommands = false
            };
        }

        public void OnGUI()
        {
            GUI.skin.font = GetMonospaceFont();
            GUILayout.BeginVertical();

            DrawConsoleOutput();
            DrawCommandSuggestions();
            DrawCommandBar();

            GUILayout.EndVertical();
        }

        private void DrawConsoleOutput()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            var consoleTextStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerLeft,
                richText = true,
                fontSize = FontSize
            };

            GUILayout.Label(_buffer.ToString(), consoleTextStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
        }

        private void DrawCommandBar()
        {
            var inputStyle = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = false
            };

            GUILayout.BeginHorizontal();

            // Input Field
            var wasEnterKeyHitInInput = Event.current.type == EventType.KeyUp &&
                                        Event.current.keyCode == KeyCode.Return &&
                                        GUI.GetNameOfFocusedControl() == _inputFieldControlName;
            GUI.SetNextControlName(_inputFieldControlName);
            _input = GUILayout.TextField(_input, inputStyle, GUILayout.ExpandWidth(true),
                GUILayout.Height(CommandBarHeight));

            // Buttons
            var wasButtonClicked =
                GUILayout.Button("Execute", GUILayout.ExpandWidth(false), GUILayout.Height(CommandBarHeight));
            if (!string.IsNullOrEmpty(_input) && (wasEnterKeyHitInInput || wasButtonClicked))
            {
                Print((p, f) => p + f.InputColor + _input);
                Execute(_input);

                _input = string.Empty;
                OnRepaint?.Invoke();
            }

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false), GUILayout.Height(CommandBarHeight)))
            {
                _buffer.Clear();
                OnRepaint?.Invoke();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawCommandSuggestions()
        {
            GUILayout.BeginVertical("box");

            if (!string.IsNullOrEmpty(_input))
            {
                FindCommandSuggestions();
                
                if (_suggestions.Count > 0)
                {
                    var rt = new GUIStyle(GUI.skin.label);
                    rt.richText = true;
                    _suggestionsBuilder.Clear();
                    for (var index = 0; index < _suggestions.Count; index++)
                    {
                        var command = _suggestions[index];
                        _printer.PrintCommand(command, _suggestionsBuilder);
                        if (index < _suggestions.Count - 1)
                        {
                            _suggestionsBuilder.AppendLine();
                        }
                    }

                    GUILayout.Label(_suggestionsBuilder.Build(), rt);
                }
            }

            GUILayout.EndVertical();
        }

        private void FindCommandSuggestions()
        {
            var commandStack = new Stack<(BaseCommand cmd, string name)>();
            var inputLower = _input.ToLower();
            foreach (var command in GetCommands().Reverse())
            {
                commandStack.Push((command, command.Name.ToLower()));
            }

            _suggestions.Clear();
            while (commandStack.Count > 0 && _suggestions.Count < NumberOfCommandSuggestions)
            {
                var (command, name) = commandStack.Pop();

                if (!command.Hidden && name.StartsWith(inputLower))
                {
                    _suggestions.Add(command);
                }

                if (command is not BaseCommandGroup commandGroup) continue;
                foreach (var subcommand in commandGroup.GetSubcommands().OrderByDescending(c => c.Name))
                {
                    commandStack.Push((subcommand, $"{name} {subcommand.Name.ToLower()}"));
                }
            }
        }

        private void AddLineToBuffer(string line)
        {
            _buffer.AppendLine(line);
            _scrollPos = new Vector2(0, float.PositiveInfinity);
            OnRepaint?.Invoke();
        }

        private static Font GetMonospaceFont()
        {
            if (_monospaceFont) return _monospaceFont;
            var osFonts = Font.GetOSInstalledFontNames();
            var fontName = MonospaceFontNames.FirstOrDefault(name => osFonts.Contains(name));
            _monospaceFont = fontName != null ? Font.CreateDynamicFontFromOSFont(fontName, 12) : null;

            return _monospaceFont;
        }
    }
}