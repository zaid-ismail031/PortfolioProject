using UnityEditor;


using UnityEngine;
using UnityEngine.SceneManagement;

using DIGA7009A.Tools;

using Teams.Tools.HumanReadableAssetWriting;

namespace Teams.Tools.HumanReadbleAssetWriting
{
  internal class HumanReadableAssetWriterSettingsEditor : EditorWindow
  {
    private HumanReadableAssetWriterSettings _settings = HumanReadableAssetWriter.Settings;

    private OnValueChangeCallback<bool> _useRounding = null;
    private OnValueChangeCallback<bool> _fixJsonIndentation = null;
    private OnValueChangeCallback<bool> _resolveGuids = null;
    private OnValueChangeCallback<bool> _includeParticleSystems = null;
    private OnValueChangeCallback<bool> _change4IndentationTo2 = null;
    private OnValueChangeCallback<bool> _logFileWrites = null;
    private OnValueChangeCallback<string> _assetPathsToIgnore = null;

    private bool _isInitialised = false;

    [MenuItem("FMS/Tools/Human-Readable Asset Writer")]
    private static void Initialise()
    {
      var window = GetWindow<HumanReadableAssetWriterSettingsEditor>();
      window.titleContent = new GUIContent("Readable Asset Writer");
      window.minSize = new Vector2(800, 300);
      window.maxSize = new Vector2(800, 300);
      window.Show();
    }

    private void OnGUI()
    {
      if (!_isInitialised)
      {
        _settings = HumanReadableAssetWriter.Settings;

        CreateSettingChangedOnValueChangeCallbacks();

        _isInitialised = true;
      }

      float originalValue = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 250;

      GUILayout.Label("Settings", EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();
      _useRounding.Value = EditorGUILayout.Toggle(
        "Use rounding (slower)",
        _settings.RoundFloatingPointNumbers);
      EditorGUILayout.LabelField("Decimals will be rounded to improve readability.");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      _fixJsonIndentation.Value = EditorGUILayout.Toggle(
        "Fix indendation (slower)",
        _settings.FixJsonIndentation);
      EditorGUILayout.LabelField("JSON will always be correctly indented.");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      _resolveGuids.Value = EditorGUILayout.Toggle(
        "Resolve GUIDs (slower)",
        _settings.ResolveGuids);
      EditorGUILayout.LabelField("GUIDs will be resolved to asset path.");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      _includeParticleSystems.Value = EditorGUILayout.Toggle(
        "Include particle systems (slow)",
        _settings.IncludeParticleSystems);
      EditorGUILayout.LabelField("ParticleSystem type will be serialised (huge amount of data, very slow).");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      _change4IndentationTo2.Value = EditorGUILayout.Toggle(
        "Use 2 space indentation (slower)",
        _settings.ChangeIndentationFrom4To2Spaces);
      EditorGUILayout.LabelField("Two-space indendation will replace four-space to improve readability.");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      _logFileWrites.Value = EditorGUILayout.Toggle(
        "Log file writes (log spam)",
        _settings.LogFileWrites);
      EditorGUILayout.LabelField("Readable asset file writes will be logged (includes duration).");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.Separator();

      EditorGUILayout.LabelField("Asset paths to ignore (CSV):");
      _assetPathsToIgnore.Value = GUILayout.TextField(_settings.AssetPathsToIgnore);

      EditorGUILayout.Separator();

      EditorGUILayout.BeginHorizontal();
      bool generateActivePrefabReadable = GUILayout.Button("Generate Active Prefab 'Readable'");
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      bool generateActiveSceneReadable = GUILayout.Button("Generate Active Scene 'Readable'");
      EditorGUILayout.EndHorizontal();

      EditorGUIUtility.labelWidth = originalValue;

      // Generate active prefab readable?
      if (generateActivePrefabReadable)
      {
        string prefabPath = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage()?.assetPath;

        if (prefabPath != null)
        {
          Debug.Log($"HRAW: Generating readable for \"{prefabPath}\"...");

          HumanReadableAssetWriter.WriteHumanReadableAssetFile(prefabPath);
        }
        else
        {
          Debug.LogError("HRAW: No active Prefab found.");
        }
      }

      // Generate active scene readable?
      if (generateActiveSceneReadable)
      {
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene != null)
        {
          Debug.Log($"HRAW: Generating readable for \"{activeScene.path}\"...");

          HumanReadableAssetWriter.WriteHumanReadableAssetFile(activeScene.path);
        }
        else
        {
          Debug.LogError("HRAW: No active Scene found.");
        }
      }
    }

    private void CreateSettingChangedOnValueChangeCallbacks()
    {
      _useRounding = new OnValueChangeCallback<bool>(
        _settings.RoundFloatingPointNumbers,
        (x) =>
        {
          _settings.RoundFloatingPointNumbers = x;
          ApplySettings();
        });

      _fixJsonIndentation = new OnValueChangeCallback<bool>(
        _settings.FixJsonIndentation,
        (x) =>
        {
          _settings.FixJsonIndentation = x;
          ApplySettings();
        });

      _resolveGuids = new OnValueChangeCallback<bool>(
        _settings.FixJsonIndentation,
        (x) =>
        {
          _settings.ResolveGuids = x;
          ApplySettings();
        });

      _includeParticleSystems = new OnValueChangeCallback<bool>(
        _settings.FixJsonIndentation,
        (x) =>
        {
          _settings.IncludeParticleSystems = x;
          ApplySettings();
        });

      _change4IndentationTo2 = new OnValueChangeCallback<bool>(
        _settings.FixJsonIndentation,
        (x) =>
        {
          _settings.ChangeIndentationFrom4To2Spaces = x;
          ApplySettings();
        });

      _logFileWrites = new OnValueChangeCallback<bool>(
        _settings.FixJsonIndentation,
        (x) =>
        {
          _settings.LogFileWrites = x;
          ApplySettings();
        });

      _assetPathsToIgnore = new OnValueChangeCallback<string>(
        _settings.AssetPathsToIgnore,
        (x) =>
        {
          _settings.AssetPathsToIgnore = x;
          ApplySettings();
        });
    }

    private void ApplySettings()
    {
      if (!_isInitialised)
      {
        return;
      }  

      HumanReadableAssetWriterManager.ApplySettings(_settings);
    }
  }
}
