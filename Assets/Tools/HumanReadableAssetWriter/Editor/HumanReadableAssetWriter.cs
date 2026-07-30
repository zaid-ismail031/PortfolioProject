using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Unity.Plastic.Newtonsoft.Json;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

using Diagnostics = System.Diagnostics;

namespace Teams.Tools.HumanReadableAssetWriting
{
  public class HumanReadableAssetWriter
  {
    public static HumanReadableAssetWriterSettings Settings => _settings;

    // Unfortunately as long as we're relying on the EditorJsonUtility this has to be 4.
    private const int IndentationSize = 4;

    private static HumanReadableAssetWriterSettings _settings = HumanReadableAssetWriterSettings.DefaultSettings;

    private static Dictionary<int, string> _indentByIndentSize = new Dictionary<int, string>();

    public static void WriteHumanReadableAssetFile(
      in string path)
    {
      if (ShouldIgnoreAssetFile(path))
      {
        return;
      }

      var timer = new Diagnostics.Stopwatch();

      try
      {
        timer.Restart();

        IEnumerable<GameObject> gameObjects = GetAssetRootObjects(
          path,
          out bool abort);

        if (abort)
        {
          return;
        }

        if (gameObjects == null)
        {
          Debug.LogError($"HRAW: Failed to find root Game-Objects for \"{path}\".");
          return;
        }

        var stringBuilder = new StringBuilder();
        var stringWriter = new StringWriter(stringBuilder);

        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
          jsonWriter.Formatting = Formatting.Indented;
          jsonWriter.Culture = CultureInfo.InvariantCulture;
          jsonWriter.Indentation = IndentationSize;

          foreach (var gameObject in gameObjects)
          {
            BuildJsonRecursive(
              gameObject,
              jsonWriter,
              jsonWriter.Indentation);
          }

          jsonWriter.Flush();

          string serialisedContent = stringBuilder.ToString();

          if (_settings.ResolveGuids)
          {
            serialisedContent = ResolveGuids(serialisedContent);
          }

          if (_settings.ChangeIndentationFrom4To2Spaces)
          {
            serialisedContent = serialisedContent.Replace("    ", "  ");
          }

          string readableFilename = $"{path}.readable.json";

          bool isSerialisedContentDifferentToExistingFile = IsSerialisedContentDifferentToExistingFile(
            serialisedContent,
            readableFilename);

          if (isSerialisedContentDifferentToExistingFile)
          {
            byte[] hash = BitConverter.GetBytes(serialisedContent.GetHashCode());

            using (var fileStream = new FileStream(readableFilename, FileMode.Create))
            {
              using (var binaryWriter = new BinaryWriter(fileStream))
              {
                binaryWriter.Write(serialisedContent.GetHashCode());
              }
            }

            File.AppendAllText(
              readableFilename,
              serialisedContent);

            if (_settings.LogFileWrites)
            {
              Debug.Log($"HRAW: Wrote human readable asset file for \"{path}\" ({timer.ElapsedMilliseconds} ms).");
            }
          }
          else
          {
            if (_settings.LogFileWrites)
            {
              Debug.Log($"HRAW: Hashes match for \"{path}\" ({timer.ElapsedMilliseconds} ms), file will not be written.");
            }
          }
        }
      }
      catch (Exception ex)
      {
        Debug.LogError(
          $"HRAW: Error while writing human readable asset file for \"{path}\". Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
      }
    }

    public static void ApplySettings(
      in HumanReadableAssetWriterSettings settings)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));

      Debug.Log(
        $"HRAW: Human Readable Asset Writer settings applied: {JsonConvert.SerializeObject(_settings, Formatting.Indented)}");
    }

    private static IEnumerable<GameObject> GetAssetRootObjects(
      in string assetPath,
      out bool abort)
    {
      abort = false;

      if (assetPath.EndsWith(
        ".prefab",
        StringComparison.OrdinalIgnoreCase))
      {
        return new List<GameObject>
        {
            AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)
        };
      }
      else if (assetPath.EndsWith(
        ".unity",
        StringComparison.OrdinalIgnoreCase))
      {
        Scene activeScene = EditorSceneManager.GetActiveScene();

        bool isActiveSceneSameAsAssetPathScene =
          activeScene.path?.Equals(
            assetPath,
            StringComparison.OrdinalIgnoreCase) ?? false;

        if (isActiveSceneSameAsAssetPathScene)
        {
          return EditorSceneManager
            .GetActiveScene()
            .GetRootGameObjects();
        }
        else
        {
          abort = true;
          Debug.LogWarning($"HRAW: Active scene is not \"{assetPath}\", readable can't be written.");
        }
      }

      return null;
    }

    private static void BuildJsonRecursive(
      in GameObject gameObject,
      in JsonWriter jsonWriter,
      in int indentSize,
      in int recursionLevel = 0)
    {
      if (gameObject == null)
      {
        return;
      }

      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyName("Name");
      jsonWriter.WriteValue($"{gameObject.name ?? "<NULL>"} (GameObject)");
      jsonWriter.WritePropertyName("Active");
      jsonWriter.WriteValue($"{gameObject.activeSelf}");
      jsonWriter.WritePropertyName("Layer");
      jsonWriter.WriteValue(LayerMask.LayerToName(gameObject.layer));
      jsonWriter.WritePropertyName("Tag");
      jsonWriter.WriteValue(gameObject.tag);
      jsonWriter.WritePropertyName("Components");
      jsonWriter.WriteStartArray();

      foreach (var component in gameObject.GetComponents<Component>())
      {
        if (component == null)
        {
          continue;
        }

        if (!_settings.IncludeParticleSystems &&
            typeof(ParticleSystem).IsAssignableFrom(component.GetType()))
        {
          continue;
        }

        string rawJson =
          Indent(
            RoundFloatingPointNumbers(
              EditorJsonUtility.ToJson(component, true)),
            (recursionLevel + 1) * 3,
            indentSize);

        jsonWriter.WriteStartObject();
        jsonWriter.WritePropertyName("Name");
        jsonWriter.WriteValue($"{component.GetType().Name} (Component)");
        jsonWriter.WritePropertyName("Parameters");
        jsonWriter.WriteRawValue(rawJson);
        jsonWriter.WriteEndObject();
      }

      jsonWriter.WriteEnd();
      jsonWriter.WritePropertyName("Children");
      jsonWriter.WriteStartArray();

      for (var i = 0; i < gameObject.transform.childCount; i++)
      {
        BuildJsonRecursive(
          gameObject.transform.GetChild(i).gameObject,
          jsonWriter,
          indentSize,
          recursionLevel + 1);
      }

      jsonWriter.WriteEnd();
      jsonWriter.WriteEndObject();
    }

    private static string ResolveGuids(
      in string input)
    {
      const int LoopIterationBailLimit = 1000000;
      const int GuidLength = 32;

      var lastMatchIndex = 0;
      var lastMatchLength = 0;
      var iterationCount = 0;

      char[] inputChars = input.ToCharArray();

      var assetPathsByGuid = new Dictionary<string, string>();

      while (true)
      {
        if (iterationCount++ > LoopIterationBailLimit)
        {
          Debug.LogError("HRAW: Bailing... looks like we're stuck in an infinite loop.");
          break;
        }

        int searchStartIndex = lastMatchIndex + lastMatchLength;

        if (searchStartIndex >= input.Length)
        {
          break;
        }

        int matchIndex = input.IndexOf(
          $"\"guid\":",
          searchStartIndex);

        bool noMatchFound = matchIndex < 0;

        if (noMatchFound)
        {
          break;
        }

        lastMatchIndex = matchIndex;
        lastMatchLength = GuidLength;

        int guidStartIndex = matchIndex + $"\"guid\": \"".Length;

        string guid = input.Substring(guidStartIndex, GuidLength);

        if (!assetPathsByGuid.TryGetValue(
          guid,
          out string assetPath))
        {
          assetPath = AssetDatabase.GUIDToAssetPath(guid);

          assetPathsByGuid.Add(guid, assetPath);
        }

        // We write the asset path into the space previously used by the GUID - we're limited
        // to the GUID length as we don't want to expand the buffer size as this will be slow
        // We start at the end - which is the most meaningful part of the path.
        int guidLengthWithQuotes = GuidLength + 2;

        for (var i = 0; i < guidLengthWithQuotes; i++)
        {
          if (i < assetPath.Length)
          {
            inputChars[guidStartIndex + GuidLength - i] = assetPath[assetPath.Length - i - 1];
          }
          else
          {
            inputChars[guidStartIndex + GuidLength - i] = ' ';
          }
        }
      }

      return new string(inputChars);
    }

    private static string Indent(
      in string text,
      in int indentLevel,
      in int indentSize)
    {
      if (!_settings.FixJsonIndentation)
      {
        return text;
      }

      int totalIndentSize = indentLevel * indentSize;

      if (!_indentByIndentSize.TryGetValue(
        totalIndentSize,
        out string indent))
      {
        for (var i = 0; i < indentLevel * indentSize; i++)
        {
          indent += ' ';
        }

        _indentByIndentSize.Add(totalIndentSize, indent);
      }

      return text.Replace("\n", $"\n{indent}");
    }

    private static string RoundFloatingPointNumbers(
      in string input)
    {
      if (!_settings.RoundFloatingPointNumbers)
      {
        return input;
      }

      const char ToBeRemovedCharMarker = (char)200;
      const int LoopIterationBailLimit = 1000000;

      char[] inputChars = input.ToCharArray();

      var resumeSearchIndex = 0;
      var loopIterationCount = 0;

      while (true)
      {
        if (loopIterationCount > LoopIterationBailLimit)
        {
          Debug.LogError("HRAW: Bailing... looks like we're stuck in an infinite loop.");
          break;
        }

        if (resumeSearchIndex >= input.Length)
        {
          break;
        }

        var pointIndex = input.IndexOf('.', resumeSearchIndex);

        if (pointIndex < 0)
        {
          break;
        }

        var j = pointIndex + 1;
        var isNumeric = true;

        while (true)
        {
          char c = inputChars[j++];

          if (c == ',' || c == '\n')
          {
            break;
          }

          if (!"0123456789-Ee".Contains(c))
          {
            isNumeric = false;
            break;
          }
        }

        if (!isNumeric)
        {
          resumeSearchIndex = j + 1;
          continue;
        }

        int decimalPlacesCount = j - pointIndex - 2;

        if (decimalPlacesCount < 4)
        {
          resumeSearchIndex = j + 1;
          continue;
        }

        j = pointIndex - 1;
        isNumeric = true;

        while (true)
        {
          char c = inputChars[j--];

          if (c == ' ')
          {
            break;
          }

          if (!"0123456789-".Contains(c))
          {
            isNumeric = false;
            break;
          }
        }

        if (!isNumeric)
        {
          resumeSearchIndex = pointIndex + decimalPlacesCount;
          continue;
        }

        int floatStartIndex = j + 2;
        int floatLength = (pointIndex - floatStartIndex) + 1 + decimalPlacesCount;

        resumeSearchIndex = floatStartIndex + floatLength;

        string valueAsString = input.Substring(floatStartIndex, floatLength);

        if (!float.TryParse(valueAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
          Debug.LogError($"HRAW: Failed to parse float \"{valueAsString}\".");
          continue;
        }

        string roundedValue = value.ToString("0.0####", CultureInfo.InvariantCulture);

        if (roundedValue.Length >= floatLength)
        {
          continue;
        }

        for (var i = 0; i < floatLength; i++)
        {
          if (i < roundedValue.Length)
          {
            inputChars[floatStartIndex + i] = roundedValue[i];
          }
          else
          {
            inputChars[floatStartIndex + i] = ToBeRemovedCharMarker;
          }
        }
      }

      return new string(
        inputChars
            .ToList()
            .Where(c => c != ToBeRemovedCharMarker)
          .ToArray());
    }

    private static bool IsSerialisedContentDifferentToExistingFile(
      in string serialisedData,
      in string filename)
    {
      if (!File.Exists(filename))
      {
        return true;
      }

      using (FileStream stream = File.OpenRead(filename))
      {
        using (var binaryReader = new BinaryReader(stream))
        {
          try
          {
            int existingFileHash = binaryReader.ReadInt32();

            return existingFileHash != serialisedData.GetHashCode();
          }
          catch (EndOfStreamException) // File is probably empty.
          {
            return true;
          }
        }
      }
    }

    private static bool ShouldIgnoreAssetFile(
      in string assetPath)
    {
      if (string.IsNullOrWhiteSpace(assetPath))
      {
        return true;
      }

      string assetPathLocal = assetPath;

      string[] pathsToIgnore = _settings
        .AssetPathsToIgnore
        .Split(',');

      foreach (var path in pathsToIgnore)
      {
        if (path.Contains('*'))
        {
          string pathWithoutWildcard = path.Replace("*", string.Empty);

          bool shouldIgnore = assetPathLocal.ToLower().Contains(pathWithoutWildcard.ToLower());

          if (shouldIgnore)
          {
            return true;
          }
        }
        else
        {
          bool shouldIgnore = assetPathLocal.Equals(path, StringComparison.OrdinalIgnoreCase);

          if (shouldIgnore)
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}