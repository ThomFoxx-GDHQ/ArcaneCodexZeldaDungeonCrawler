using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueSequenceCreatorWindow : EditorWindow
{
    // ---- Persisted keys so designers don't have to re-pick every time ----
    private const string PrefsJsonPathKey = "DialogueSeqCreator.JsonPath";

    // Left column state
    private DialogueSequence _sequenceAsset; // your SO
    private string _newLineInput = string.Empty;

    // Right column state
    private Vector2 _scroll;

    // In-memory dictionary we export to JSON (no SO)
    private readonly DialogueDictionary _dict = new DialogueDictionary();

    // JSON file path to overwrite on Save
    private string _jsonPath;

    // ---------------- Menu ----------------
    [MenuItem("Tools/Dialogue/Sequence Creator")]
    private static void Open()
    {
        var w = GetWindow<DialogueSequenceCreatorWindow>("Dialogue Sequence Creator");
        w.minSize = new Vector2(800f, 480f);
        w.Show();
    }

    // ---------------- Lifecycle ----------------
    private void OnEnable()
    {
        _jsonPath = EditorPrefs.GetString(PrefsJsonPathKey, string.Empty);

        // Try loading JSON if we already have a saved path
        if (File.Exists(_jsonPath))
        {
            string json = File.ReadAllText(_jsonPath);
            _dict.FromJson(json);
        }
    }

    // ---------------- GUI ----------------
    private void OnGUI()
    {
        float leftWidth = 320f;

        using (new EditorGUILayout.HorizontalScope())
        {
            // LEFT COLUMN (SO field, New SO, Input, Cancel/Submit, Save JSON)
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(leftWidth)))
            {
                GUILayout.Space(8);
                DrawSequenceSelector();      // "SO Object File" + "New SO"
                GUILayout.Space(12);
                DrawDialogueInput();         // "Dialogue Input" box + Cancel/Submit
                GUILayout.Space(12);
                DrawSaveJson();              // "Save Dictionary to Json"
            }

            // RIGHT COLUMN (big scroll view of sequence lines)
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                GUILayout.Space(8);
                EditorGUILayout.LabelField("Index  Id  with Existing Lines of Dialogue", EditorStyles.boldLabel);

                using (var sv = new EditorGUILayout.ScrollViewScope(_scroll, GUILayout.ExpandHeight(true)))
                {
                    _scroll = sv.scrollPosition;
                    DrawSequencePreview();
                }
            }
        }
    }

    // ----- Left: SO Object File + New SO -----
    private void DrawSequenceSelector()
    {
        EditorGUILayout.LabelField("SO Object File", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        _sequenceAsset = (DialogueSequence)EditorGUILayout.ObjectField(_sequenceAsset, typeof(DialogueSequence), false);
        if (EditorGUI.EndChangeCheck())
        {
            // No extra action required; we operate directly on the SO
        }

        if (GUILayout.Button("New SO", GUILayout.Height(28)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Dialogue Sequence",
                "DialogueSequence",
                "asset",
                "Choose location for new DialogueSequence asset.");

            if (!string.IsNullOrEmpty(path))
            {
                var seq = ScriptableObject.CreateInstance<DialogueSequence>();
                AssetDatabase.CreateAsset(seq, path);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(seq);
                _sequenceAsset = seq;
            }
        }
    }

    // ----- Left: Dialogue Input + Cancel / Submit -----
    private void DrawDialogueInput()
    {
        EditorGUILayout.LabelField("Dialogue Input", EditorStyles.boldLabel);

        // Big rounded area look by boxing
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            _newLineInput = EditorGUILayout.TextArea(_newLineInput, GUILayout.MinHeight(120));

            using (new EditorGUILayout.HorizontalScope())
            {
                // Cancel
                if (GUILayout.Button("Cancel", GUILayout.Height(24), GUILayout.Width(120)))
                {
                    _newLineInput = string.Empty;
                    GUI.FocusControl(null);
                }

                GUILayout.FlexibleSpace();

                // Submit
                using (new EditorGUI.DisabledScope(_sequenceAsset == null || string.IsNullOrWhiteSpace(_newLineInput)))
                {
                    if (GUILayout.Button("Submit", GUILayout.Height(24), GUILayout.Width(160)))
                    {
                        SubmitLine(_newLineInput.Trim());
                        _newLineInput = string.Empty;
                        GUI.FocusControl(null);
                    }
                }
            }
        }
    }

    // ----- Left: Save Dictionary to Json -----
    private void DrawSaveJson()
    {
        if (GUILayout.Button("Save Dictionary to Json", GUILayout.Height(30)))
        {
            SaveDictionaryToJson();
        }

        // Small hint of current target file
        if (!string.IsNullOrEmpty(_jsonPath))
        {
            EditorGUILayout.HelpBox(_jsonPath, MessageType.None);
        }

        if (string.IsNullOrEmpty(_jsonPath))
            return;

        if (GUILayout.Button("Update Dictionary Json Location", GUILayout.Height(30)))
        {
            UpdateDictionaryLocation();
        }
    }

    // ----- Right: Sequence Preview (read-only) -----
    private void DrawSequencePreview()
    {
        if (_sequenceAsset == null)
        {
            EditorGUILayout.HelpBox("Pick or create a DialogueSequence ScriptableObject on the left.", MessageType.Info);
            return;
        }

        var list = _sequenceAsset.dialogueIDs;
        if (list == null || list.Count == 0)
        {
            EditorGUILayout.HelpBox("Sequence is empty. Add lines on the left.", MessageType.None);
            return;
        }

        // Header
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Index", GUILayout.Width(48));
            GUILayout.Label("ID", GUILayout.Width(48));
            GUILayout.Label("Line (preview from dictionary)", GUILayout.ExpandWidth(true));
        }

        EditorGUILayout.Space(4);

        // Rows
        for (int i = 0; i < list.Count; i++)
        {
            int id = list[i];
            _dict.TryGetLine(id, out string line);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(i.ToString(), GUILayout.Width(48));
                GUILayout.Label(id.ToString(), GUILayout.Width(48));
                GUILayout.Label(string.IsNullOrEmpty(line) ? "[Missing in dictionary]" : line, GUILayout.ExpandWidth(true));
            }
        }
    }

    // ---------------- Actions ----------------
    private void SubmitLine(string line)
    {
        try
        {
            int id = _dict.GetOrAdd(line); // case-sensitive match
            // Append id to Sequence SO
            Undo.RecordObject(_sequenceAsset, "Add Dialogue ID");
            _sequenceAsset.dialogueIDs.Add(id);
            EditorUtility.SetDirty(_sequenceAsset);
            AssetDatabase.SaveAssets();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to add line: {ex.Message}");
        }
    }

    private void UpdateDictionaryLocation()
    {
        // Ask for a location the first time; afterwards, overwrite silently.
        if (!string.IsNullOrEmpty(_jsonPath))
        {
            string suggested = Application.dataPath; // default into project
            string picked = EditorUtility.OpenFilePanel(
                "dialogue_en Dictionary JSON",
                suggested,
                "json");

            if (string.IsNullOrEmpty(picked))
                return;

            _jsonPath = picked;
            EditorPrefs.SetString(PrefsJsonPathKey, _jsonPath);
            string json = File.ReadAllText(_jsonPath);
            _dict.FromJson(json);
        }
    }

    private void SaveDictionaryToJson()
    {
        // Ask for a location the first time; afterwards, overwrite silently.
        if (string.IsNullOrEmpty(_jsonPath))
        {
            string suggested = Application.dataPath; // default into project
            string picked = EditorUtility.SaveFilePanel(
                "Save Dialogue Dictionary JSON",
                suggested,
                "dialogue_en",
                "json");

            if (string.IsNullOrEmpty(picked))
                return;

            _jsonPath = picked;
            EditorPrefs.SetString(PrefsJsonPathKey, _jsonPath);
        }

        // Overwrite existing file
        try
        {
            string json = _dict.ToJson(prettyPrint: true);
            File.WriteAllText(_jsonPath, json);
            EditorUtility.RevealInFinder(_jsonPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save JSON: {ex.Message}");
        }
    }
}
