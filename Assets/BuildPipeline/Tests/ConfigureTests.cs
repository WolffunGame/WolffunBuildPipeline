using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Wolffun.BuildPipeline;

public class ConfigureTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void ConfigureTestsSimplePasses()
    {
        //var configure = WolffunAzureDevops.GetBuildConfig();
        //Assert.IsNotNull(configure);
    }

    [Test]
    public void TestParseSceneList()
    {
        //get scene list
        var sceneList = AssetDatabase.FindAssets("t:Scene");
        //get scene names
        var scenePaths = new List<string>();
        foreach (var scene in sceneList)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(scene);
            //get file path without extensions
            //var sceneName = scenePath.Substring(0, scenePath.Length - 6);
            scenePaths.Add(scenePath);
            Debug.Log(scenePath);
        }

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
        for (int i = 0; i < scenePaths.Count; i++)
        {
            string sceneName = scenePaths[i];
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(new EditorBuildSettingsScene(sceneName, true)).ToArray();
        }
        
        Assert.AreEqual(scenePaths.Count, EditorBuildSettings.scenes.Length);
    }
}
