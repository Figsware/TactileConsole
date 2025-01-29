using System;
using Tactile.Console.Parameters;
using UnityEngine;

namespace Tactile.Console.Commands.Preferences
{
    public abstract class BasePreferencesCommand : BaseCommandGroup
    {
        
        private static BaseCommand[] CreateSubcommands(IPreferences preferences)
        {
            return new BaseCommand[] {
                new ClearCommand(preferences),
                new DeleteCommand(preferences),
                new SaveCommand(preferences),
                new GetterCommandGroup(preferences),
                new SetterCommandGroup(preferences)
            };
        }

        private IPreferences _preferences;

        protected BasePreferencesCommand(string name, IPreferences preferences) : base(name, CreateSubcommands(preferences))
        {
        }

        private class ClearCommand : BaseCommand
        {
            private readonly IPreferences _preferences;
            
            public ClearCommand(IPreferences preferences) : base("clear", "Clears ALL player preferences. Be careful!")
            {
                _preferences = preferences;
            }

            public override void Execute(Console console, string body)
            {
                _preferences.Clear();
            }
        }
        
        private class DeleteCommand : BaseCommandWithParameters<string>
        {
            private readonly IPreferences _preferences;
            
            public DeleteCommand(IPreferences preferences) : base("delete", "Deletes a key from the player preferences", new StringParameter("key", "The key to delete", true))
            {
                _preferences = preferences;
            }

            protected override void Execute(Console console, ParsedArguments arguments)
            {
                if (!_preferences.HasKey(arguments.Arg1))
                {
                    console.PrintError($"The key \"{arguments.Arg1}\" does not exist!");
                }
                else
                {
                    _preferences.DeleteKey(arguments.Arg1);
                }
            }
        }

        private class SaveCommand : BaseCommand
        {
            private readonly IPreferences _preferences;
            
            public SaveCommand(IPreferences preferences) : base("save", "Saves the PlayerPrefs.")
            {
                _preferences = preferences;
            }

            public override void Execute(Console console, string body)
            {
                _preferences.Save();
            }
        }

        private class GetterCommandGroup : BaseCommandGroup
        {
            private static BaseCommand[] CreateGetterCommands(IPreferences preferences) => new BaseCommand[] {
                new GetterCommand<string>("string", preferences.GetString),
                new GetterCommand<float>("float", preferences.GetFloat),
                new GetterCommand<int>("int", preferences.GetInt)
            };
            
            public GetterCommandGroup(IPreferences preferences) : base("get", CreateGetterCommands(preferences))
            {
            }

            private class GetterCommand<T> : BaseCommandWithParameters<string>
            {
                private readonly Func<string, T> _getter;

                public GetterCommand(string name, Func<string, T> getter) : base(name,
                    "getter",
                    new StringParameter("key", "The key of the item to get", true))
                {
                    _getter = getter;
                }

                protected override void Execute(Console console, ParsedArguments arguments)
                {
                    if (!PlayerPrefs.HasKey(arguments.Arg1))
                    {
                        console.PrintError($"The key \"{arguments.Arg1}\" does not exist!");
                    }
                    else
                    {
                        var val = _getter.Invoke(arguments.Arg1);
                        console.Print(val.ToString());
                    }
                }
            }
        }

        private class SetterCommandGroup : BaseCommandGroup
        {
            
            private static BaseCommand[] CreateSetterCommands(IPreferences preferences) => new BaseCommand[]
            {
                new SetterCommand<string>("string", preferences.SetString,
                    new StringParameter("str", "The string to set", true)),
                new SetterCommand<float>("float", preferences.SetFloat,
                    new FloatParameter("float", "The float to set", true)),
                new SetterCommand<int>("int", preferences.SetInt, new IntegerParameter("int", "The int to set", true))
            };

            public SetterCommandGroup(IPreferences preferences) : base("set", CreateSetterCommands(preferences))
            {
            }

            private class SetterCommand<T> : BaseCommandWithParameters<string, T>
            {
                private readonly Action<string, T> _setter;

                public SetterCommand(string name, Action<string, T> setter, BaseParameter<T> valueParameter) : base(
                    name,
                    "setter",
                    new StringParameter("key", "The key of the item to set", true), valueParameter)
                {
                    _setter = setter;
                }

                protected override void Execute(Console console, ParsedArguments arguments)
                {
                    _setter.Invoke(arguments.Arg1, arguments.Arg2);
                }
            }
        }
        
        public interface IPreferences
        {
            void Clear();
            void Save();
            void DeleteKey(string key);
            bool HasKey(string key);
            string GetString(string key);
            float GetFloat(string key);
            int GetInt(string key);
            void SetString(string key, string val);
            void SetFloat(string key, float val);
            void SetInt(string key, int val);
        }

    }
}