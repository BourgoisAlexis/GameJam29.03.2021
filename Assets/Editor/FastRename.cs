using UnityEngine;
using UnityEditor;

public class FastRename : EditorWindow
{
    #region Variables
    public GameObject Parent;
    public string NewName;

    private Editor editor;
    private Vector2 scrollPosition;
    #endregion

    [MenuItem("Tools/FastRename")]
    public static void ShowWindow()
    {
        GetWindow<FastRename>("Fast Rename");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        GUILayout.Space(20);

        if (GUILayout.Button("Rename", GUILayout.Width(150), GUILayout.Height(20)))
            Rename();

        EditorGUILayout.EndScrollView();
    }

    private void Rename()
    {
        Transform[] childs = Parent.GetComponentsInChildren<Transform>();

        if (childs.Length > 1)
            Parent.name = NewName + "s";
        else 
            Parent.name = NewName;

        for (int i = 1; i < childs.Length; i++)
        {
            string n = NewName + "_";
            if (i - 1 < 10)
                n += "0";

            n += i - 1;

            childs[i].gameObject.name = n;
        }
    }
}
