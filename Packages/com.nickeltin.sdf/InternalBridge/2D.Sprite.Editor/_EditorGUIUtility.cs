using UnityEditor;
using UnityEngine;

namespace nickeltin.SDF.InternalBridge.Editor
{
    internal class _EditorGUIUtility
    {
        public static GUIContent TempContent(string t, Texture i)
        {
            return EditorGUIUtility.TempContent(t, i);
        }
    }
}