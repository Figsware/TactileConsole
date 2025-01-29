using UnityEngine;

namespace Tactile.Console.Commands.Preferences
{
    [GlobalCommand]
    public class PlayerPreferencesCommand : BasePreferencesCommand
    {
        public PlayerPreferencesCommand() : base("prefs", new Preferences())
        {
        }

        private class Preferences : IPreferences
        {
            public void Clear()
            {
                PlayerPrefs.DeleteAll();
            }

            public void Save()
            {
                PlayerPrefs.Save();
            }

            public void DeleteKey(string key)
            {
                PlayerPrefs.DeleteKey(key);
            }

            public bool HasKey(string key)
            {
                return PlayerPrefs.HasKey(key);
            }

            public string GetString(string key)
            {
                return PlayerPrefs.GetString(key);
            }

            public float GetFloat(string key)
            {
                return PlayerPrefs.GetFloat(key);
            }

            public int GetInt(string key)
            {
                return PlayerPrefs.GetInt(key);
            }

            public void SetString(string key, string val)
            {
                PlayerPrefs.SetString(key, val);
            }

            public void SetFloat(string key, float val)
            {
                PlayerPrefs.SetFloat(key, val);
            }

            public void SetInt(string key, int val)
            {
                PlayerPrefs.SetInt(key, val);
            }
        }
    }
}