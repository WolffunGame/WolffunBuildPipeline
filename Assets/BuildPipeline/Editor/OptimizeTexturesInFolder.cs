using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class OptimizeTexturesInFolder : MonoBehaviour
{
    private static float androidScaleFactorMaxsize = 0.7f;
    private static float iOSScaleFactorMaxsize = 0.7f;
    private static float windowsScaleFactorMaxsize = 1f;
    private static float webglScaleFactorMaxsize = 0.5f;

    private static TextureImporterFormat androidFormat = TextureImporterFormat.ETC2_RGBA8Crunched;
    private static TextureImporterFormat iOSFormat = TextureImporterFormat.ASTC_4x4;
    private static TextureImporterFormat windowFormat = TextureImporterFormat.DXT5;
    private static TextureImporterFormat webGLFormat = TextureImporterFormat.DXT5;

    private static int  androidQuality = 50;
    private static int iOSQuality = 50;
    private static int windowQuality = 100;
    private static int webGLQuality = 0;



    [MenuItem("Tools/Optimize Textures in Folder")]
    public static void OptimizeTextures()
    {
        string folderPath = "";
        if (!string.IsNullOrEmpty(folderPath))
        {
            androidScaleFactorMaxsize = float.Parse(System.Environment.GetEnvironmentVariable("androidScaleFactorMaxsize"));
            iOSScaleFactorMaxsize = float.Parse(System.Environment.GetEnvironmentVariable("iOSScaleFactorMaxsize"));
            windowsScaleFactorMaxsize = float.Parse(System.Environment.GetEnvironmentVariable("windowsScaleFactorMaxsize"));
            webglScaleFactorMaxsize = float.Parse(System.Environment.GetEnvironmentVariable("webglScaleFactorMaxsize"));
            androidFormat = (TextureImporterFormat)System.Enum.Parse(typeof(TextureImporterFormat), System.Environment.GetEnvironmentVariable("androidFormat"));
            iOSFormat = (TextureImporterFormat)System.Enum.Parse(typeof(TextureImporterFormat), System.Environment.GetEnvironmentVariable("iOSFormat"));
            windowFormat = (TextureImporterFormat)System.Enum.Parse(typeof(TextureImporterFormat), System.Environment.GetEnvironmentVariable("windowFormat"));
            webGLFormat = (TextureImporterFormat)System.Enum.Parse(typeof(TextureImporterFormat), System.Environment.GetEnvironmentVariable("webGLFormat"));
            androidQuality = int.Parse(System.Environment.GetEnvironmentVariable("androidQuality"));
            iOSQuality = int.Parse(System.Environment.GetEnvironmentVariable("iOSQuality"));
            windowQuality = int.Parse(System.Environment.GetEnvironmentVariable("windowQuality"));
            webGLQuality = int.Parse(System.Environment.GetEnvironmentVariable("webGLQuality"));
            OptimizeAllTexturesInFolder(folderPath, androidScaleFactorMaxsize, iOSScaleFactorMaxsize, windowsScaleFactorMaxsize, webglScaleFactorMaxsize,androidFormat,iOSFormat,windowFormat,webGLFormat,androidQuality,iOSQuality,windowQuality,webGLQuality) ;
            Debug.Log("Texture optimization completed.");
        }
        else
        {
            Debug.LogWarning("No folder selected.");
        }
    }


    private static Dictionary<BuildTarget, float> platformScaleFactors = new Dictionary<BuildTarget, float>
    {
        { BuildTarget.Android, 0.5f },
        { BuildTarget.iOS, 0.7f },
        { BuildTarget.StandaloneWindows, 1f },
        { BuildTarget.WebGL, 0.35f }
    };
    private static Dictionary<BuildTarget, TextureImporterFormat> platformFormats = new Dictionary<BuildTarget, TextureImporterFormat>
    {
        { BuildTarget.Android, TextureImporterFormat.ETC2_RGBA8 },
        { BuildTarget.iOS, TextureImporterFormat.ASTC_8x8 },
        { BuildTarget.StandaloneWindows, TextureImporterFormat.DXT5 },
        { BuildTarget.WebGL, TextureImporterFormat.DXT5 }
    };

    private static Dictionary<BuildTarget, int> platformQualities = new Dictionary<BuildTarget, int>
    {
        { BuildTarget.Android, 50 },
        { BuildTarget.iOS, 75 },
        { BuildTarget.StandaloneWindows, 100 },
        { BuildTarget.WebGL, 75 }
    };
    public static void OptimizeAllTexturesInFolder(string folderPath, float androidFactor, float iOSFactor, float windowsFactor, float webglFactor, TextureImporterFormat androidFormat, TextureImporterFormat iOSFormat, TextureImporterFormat windowsFormat, TextureImporterFormat webglFormat, int androidQuality, int iOSQuality, int windowsQuality, int webglQuality)
    {

        platformScaleFactors[BuildTarget.Android] = androidFactor;
        platformScaleFactors[BuildTarget.iOS] = iOSFactor;
        platformScaleFactors[BuildTarget.StandaloneWindows] = windowsFactor;
        platformScaleFactors[BuildTarget.WebGL] = webglFactor;
        platformFormats[BuildTarget.Android] = androidFormat;
        platformFormats[BuildTarget.iOS] = iOSFormat;
        platformFormats[BuildTarget.StandaloneWindows] = windowsFormat;
        platformFormats[BuildTarget.WebGL] = webglFormat;

        platformQualities[BuildTarget.Android] = androidQuality;
        platformQualities[BuildTarget.iOS] = iOSQuality;
        platformQualities[BuildTarget.StandaloneWindows] = windowsQuality;
        platformQualities[BuildTarget.WebGL] = webglQuality;

        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg"))
            {
                OptimizeTexture(file);
            }
        }
        AssetDatabase.Refresh(); // Refresh the AssetDatabase after processing all textures
    }

    private static void OptimizeTexture(string texturePath)
    {
        // Convert absolute path to Unity relative path
      
        string relativePath = "Assets" + texturePath.Substring(Application.dataPath.Length).Replace("\\", "/");
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
        if (texture != null)
        {
            foreach (var kvp in platformScaleFactors)
            {
                if (texturePath.Contains("Plugins") || texturePath.Contains("Packages") || texturePath.Contains("Demigiant"))
                {
                    continue;
                }
                int originalWidth, originalHeight;
                GetTextureOriginalSize(texture, out originalWidth, out originalHeight);
                BuildTarget platform = kvp.Key;
                float scaleFactor = kvp.Value;
                int maxSize = GetOptimizedSize(originalWidth, originalHeight, scaleFactor);
                //Debug.Log($"Optimized Max Size for {relativePath} on {platform}: {maxSize}");

                // Adjust the texture import settings
                TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                if (importer != null)
                {
                    TextureImporterPlatformSettings platformSettings = importer.GetPlatformTextureSettings(platform.ToString());
                    platformSettings.overridden = true;
                    platformSettings.maxTextureSize = maxSize;

                    // Set additional settings
                    if (platformFormats.ContainsKey(platform))
                    {
                        platformSettings.format = platformFormats[platform];
                    }
                    if (platformQualities.ContainsKey(platform))
                    {
                        platformSettings.compressionQuality = platformQualities[platform];
                    }

                    importer.SetPlatformTextureSettings(platformSettings);
                }
            }
            TextureImporter defaultImporter = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (defaultImporter != null)
            {
                defaultImporter.SaveAndReimport(); // Save and reimport the texture to apply changes
            }
        }
        else
        {
            Debug.LogError("Texture not found at path: " + relativePath);
        }
    }


    private static int GetOptimizedSize(int width, int height, float scaleFactor)
    {

        int largerDimension = Mathf.Max(width, height);
        int scaledSize = Mathf.CeilToInt(largerDimension * scaleFactor);
       // Debug.Log("Demention"+largerDimension);
        Debug.Log("ScaleFacotr"+scaleFactor);
        // Check if scaledSize exceeds the maxTextureSize for the platform
        {
            // If scaledSize does not exceed the maxTextureSize, apply the scaling logic
            if (scaledSize <= 16)
                return 16;
            else if (scaledSize <= 32)
                return 32;
            else if (scaledSize <= 64)
                return 64;
            else if (scaledSize <= 128)
                return 128;
            else if (scaledSize <= 256)
                return 256;
            else if (scaledSize <= 512)
                return 512;
            else if (scaledSize <= 1024)
                return 1024;
            else if (scaledSize <= 2048)
                return 2048;
            else if (scaledSize <= 4096)
                return 4096;
            else if (scaledSize <= 8192)
                return 8192;
            else if (scaledSize <= 16384)
                return 16384;
            else
                return largerDimension; // fallback nếu lớn hơn 16384
        }
    }
    private static void GetTextureOriginalSize(Texture2D texture, out int width, out int height)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.isReadable = true; // Ensure texture is readable to get its data
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);

            // Load texture temporarily to get its original size
            Texture2D tempTexture = new Texture2D(2, 2);
            File.ReadAllBytes(path);
            tempTexture.LoadImage(File.ReadAllBytes(path));
            width = tempTexture.width;
            height = tempTexture.height;
            importer.isReadable = false;
            // Clean up the temporary texture
            UnityEngine.Object.DestroyImmediate(tempTexture);

            Debug.Log($"Original Size of {path} - Width: {width}, Height: {height}");
        }
        else
        {
            width = texture.width;
            height = texture.height;
        }
    }
}
