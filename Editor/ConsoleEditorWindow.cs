using Tactile.Console.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Tactile.Console.Editor
{
    public class ConsoleEditorWindow : EditorWindow
    {
        private IMGUIConsole _imguiConsole;
        
        private void OnEnable()
        {
            _imguiConsole = new IMGUIConsole();
            _imguiConsole.OnRepaint += Repaint;
        }

        [MenuItem("Tactile/Console")]
        private static void ShowWindow()
        {
            var window = GetWindow<ConsoleEditorWindow>();
            window.titleContent = new GUIContent("Tactile Console");
            window.Show();
        }

        private void OnGUI() => _imguiConsole.OnGUI();
    }
}