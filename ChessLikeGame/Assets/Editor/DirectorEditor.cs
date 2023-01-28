using Chess.Control;
using Chess.Fen;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Director))]
    public class DirectorEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Director director = (Director) target;
            DrawDefaultInspector();
            EditorGUILayout.TextArea($"{director.fenStringEnumEnum.ToString()}\n{FenExamples.GetFenByName(director.fenStringEnumEnum)}\n{FenExamples.GetSummaryByName(director.fenStringEnumEnum)}",GUILayout.Height(200));
            // director.textToDisplay  = FenExamples.GetSummaryByName(director.fenStringEnumEnum);
            

        }
    }
}