using Tactile.Console.Commands.Preferences;
using UnityEditor;

namespace Tactile.Console.Commands
{
    [GlobalCommand]
    public class EditorPreferencesCommand : BasePreferencesCommand
    {
        public EditorPreferencesCommand() : base("eprefs", new Preferences())
        {
        }

        private class Preferences : IPreferences
        {
            public void Clear()
            {
                EditorPrefs.DeleteAll();
            }

            public void Save()
            {
                
            }

            public void DeleteKey(string key)
            {
                EditorPrefs.DeleteKey(key);
            }

            public bool HasKey(string key)
            {
                return EditorPrefs.HasKey(key);
            }

            public string GetString(string key)
            {
                return EditorPrefs.GetString(key);
            }

            public float GetFloat(string key)
            {
                return EditorPrefs.GetFloat(key);
            }

            public int GetInt(string key)
            {
                return EditorPrefs.GetInt(key);
            }

            public void SetString(string key, string val)
            {
                EditorPrefs.SetString(key, val);
            }

            public void SetFloat(string key, float val)
            {
                EditorPrefs.SetFloat(key, val);
            }

            public void SetInt(string key, int val)
            {
                EditorPrefs.SetInt(key, val);
            }
        }
    }
}