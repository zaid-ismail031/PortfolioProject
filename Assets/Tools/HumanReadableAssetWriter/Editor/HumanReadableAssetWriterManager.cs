// This class is mostly necessary because Unity calls OnWillSaveAssets before the asset is
// actually updated - so, the value(s) are stale. This class simply tracks which assets have
// changed and then periodically writes the human readable asset files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Unity.Plastic.Newtonsoft.Json;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Teams.Tools.HumanReadableAssetWriting
{
  [InitializeOnLoad]
  internal class HumanReadableAssetWriterManager : UnityEditor.AssetModificationProcessor
  {
    // We don't want to wait too long as you may modify something and then switch over to
    // Plastic pretty quickly to commit.
    private const double UpdateRateS = 2.0;

    // After Unity tells us an asset has changed we need to wait a bit before writing the
    // asset file (or else we'll get stale values), this value is the min amount of time
    // we'll wait.
    private const double MinTimeToWaitAfterAssetHasChangedS = 0.05;

    private const string ReadableFileExtension = ".readable.json";

    private static string _settingsPath = $@"{Application.dataPath}\Editor\";
    private static string _settingsFullFilename = $@"{_settingsPath}HumanReadableAssetWriter.settings.json";

    private static double _lastTimeSinceStartup = 0.0;
    private static double _timeSinceLastUpdate = 0.0;

    private static List<string> _modifiedAssets = new List<string>();

    static HumanReadableAssetWriterManager()
    {
      HumanReadableAssetWriterSettings settings = LoadSettingsFromFile();

      HumanReadableAssetWriter.ApplySettings(settings);

      MarkAllExistingReadablesForUpdate();

      EditorApplication.update += Update;

      EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChangedInEditMode;
    }

    // This is a Unity method called just before assets are saved.
    public static string[] OnWillSaveAssets(
      string[] paths)
    {
      foreach (string path in paths)
      {
        bool isSupportedAssetType =
          path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) ||
          path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase);

        if (!isSupportedAssetType)
        {
          continue;
        }

        if (_modifiedAssets.Contains(
          path,
          StringComparer.OrdinalIgnoreCase))
        {
          continue;
        }

        _modifiedAssets.Add(path);
      }

      _timeSinceLastUpdate += MinTimeToWaitAfterAssetHasChangedS;

      return paths;
    }

    public static void ApplySettings(
      in HumanReadableAssetWriterSettings settings)
    {
      HumanReadableAssetWriter.ApplySettings(settings);

      SaveSettingsToFile(settings);
    }

    private static void Update()
    {
      double timeSinceStartup = EditorApplication.timeSinceStartup;
      double deltaTime = timeSinceStartup - _lastTimeSinceStartup;

      _lastTimeSinceStartup = timeSinceStartup;
      _timeSinceLastUpdate += deltaTime;

      if (_timeSinceLastUpdate < UpdateRateS)
      {
        return;
      }

      _timeSinceLastUpdate = 0.0;

      foreach (var modifiedAssetPath in _modifiedAssets)
      {
        try
        {
          HumanReadableAssetWriter.WriteHumanReadableAssetFile(modifiedAssetPath);
        }
        catch (Exception ex)
        {
          Debug.LogError(
            $"HRAW: Error while writing human readable asset file \"{modifiedAssetPath}\"... {ex.Message}");
        }
      }

      _modifiedAssets.Clear();
    }

    private static HumanReadableAssetWriterSettings LoadSettingsFromFile()
    {
      try
      {
        if (!File.Exists(_settingsFullFilename))
        {
          return HumanReadableAssetWriterSettings.DefaultSettings;
        }

        string fileContent = File.ReadAllText(_settingsFullFilename);

        return JsonConvert.DeserializeObject<HumanReadableAssetWriterSettings>(fileContent);
      }
      catch (Exception ex)
      {
        Debug.LogError(
          $"HRAW: Failed to load Human Readable Asset Writer settings from \"{_settingsFullFilename}\". Error: {ex.Message}");
      }

      return HumanReadableAssetWriterSettings.DefaultSettings;
    }

    private static void SaveSettingsToFile(
      in HumanReadableAssetWriterSettings settings)
    {
      try
      {
        if (!Directory.Exists(_settingsPath))
        {
          Directory.CreateDirectory(_settingsPath);
        }

        File.WriteAllText(
          _settingsFullFilename,
          JsonConvert.SerializeObject(
            settings,
            Formatting.Indented));
      }
      catch (Exception ex)
      {
        Debug.LogError(
          $"HRAW: Failed to save Human Readable Asset Writer settings to \"{_settingsFullFilename}\". Error: {ex.Message}");
      }
    }

    private static void MarkAllExistingReadablesForUpdate()
    {
      IEnumerable<string> assetPaths = GetAllAssetsWithReadablesInProject();

      _modifiedAssets.AddRange(assetPaths);
    }

    private static IEnumerable<string> GetAllAssetsWithReadablesInProject()
    {
      try
      {
        IEnumerable<string> readablesPaths = Directory.GetFiles(
            Application.dataPath,
            $"*{ReadableFileExtension}",
            SearchOption.AllDirectories);

        readablesPaths = RemoveReadablesWhereAssetNotFound(readablesPaths);

        IEnumerable<string> assetPaths = readablesPaths
          .Select(x =>
            x
              .Replace(ReadableFileExtension, string.Empty)
              .Replace(@"\", "/"));

        IEnumerable<string> relativeAssetPaths = assetPaths
          .Select(x =>
            x.Remove(0, x.ToLower().IndexOf("/assets/") + 1));

        return relativeAssetPaths;
      }
      catch (Exception ex)
      {
        Debug.LogError(
          $"HRAW: Error while searching for Assets with Readables. Error: {ex.Message}");
      }

      return new string[0];
    }

    private static IEnumerable<string> RemoveReadablesWhereAssetNotFound(
      in IEnumerable<string> readablePaths)
    {
      var readablePathsWithMissingAssetsRemoved = new List<string>();

      foreach (var path in readablePaths)
      {
        bool isAssetNotFound = !DoesAssetExistForReadable(path);

        if (isAssetNotFound)
        {
          Debug.LogWarning(
            $"HRAW: The following asset appears to have been moved or removed, this readable file should be removed: \"{path}\".");
          continue;
        }

        readablePathsWithMissingAssetsRemoved.Add(path);
      }

      return readablePathsWithMissingAssetsRemoved;
    }

    private static bool DoesAssetExistForReadable(
      in string readablePath)
    {
      string assetpath = readablePath.Replace(ReadableFileExtension, string.Empty);

      return File.Exists(assetpath);
    }

    private static void OnActiveSceneChangedInEditMode(
      Scene oldScene,
      Scene newScene)
    {
      if (_modifiedAssets.Contains(
       newScene.path,
       StringComparer.OrdinalIgnoreCase))
      {
        return;
      }

      _modifiedAssets.Add(newScene.path);
    }
  }
}