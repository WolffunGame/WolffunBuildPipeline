using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SetupAudioForWebGL : EditorWindow
{
    [MenuItem("Tools/Setup Audio for WebGL")]
    public static void SetupAudio()
    {
        string[] audioPaths = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets\\BuildPipeline\\Tests\\Audio" }); // Thay đổi đường dẫn tới thư mục audio của bạn
        foreach (var guid in audioPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            if (importer != null)
            {
                SetWebGLSettings(importer);
                Debug.Log($"Setup WebGL settings for {assetPath}");
            }
            else
            {
                Debug.LogWarning($"Failed to get importer for {assetPath}");
            }
        }
    }

    private static void SetWebGLSettings(AudioImporter importer)
    {
        AudioImporterSampleSettings settings = importer.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.CompressedInMemory; // Set loadType to CompressedInMemory
        settings.compressionFormat = AudioCompressionFormat.AAC; // Set compression format to Vorbis
        settings.quality = 0.4f; // Set quality (0.0f - 1.0f)

        importer.defaultSampleSettings = settings;

        // Enable override for WebGL
        importer.SetOverrideSampleSettings("WebGL", settings);

        // Save and reimport to apply changes
        importer.SaveAndReimport();
    }
}
