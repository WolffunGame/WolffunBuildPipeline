using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Wolffun.BuildPipeline
{
    public class WolffunAzureDevops
    {
        public static void PerformBuild()
        {
            //get commandline arguments -outputPath $(Build.BinariesDirectory)
            //-outputFileName $(XcodeProjectFolderName) -configuration $(Configuration) -buildNumber $(BuildNumber) -appversion $(AppVersion)
            string[] args = System.Environment.GetCommandLineArgs();
            //log all arguments joined by space
            Debug.Log(string.Join(" ", args));
            string buildTarget = "";
            string outputPath = "";
            string outputFileName = "";
            string configuration = "";
            string buildNumber = "";
            string appversion = "";
            string environment = "";
            string scriptingBackend = "";
            string outputExtension = "";
            string scriptDefinedSymbols = "";
            string splitApplicationBinary = "";
#if UNITY_IOS
            string buildXcodeAppend = "";
#endif
#if UNITY_ANDROID
            string buildAppBundle = "";
#endif

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-buildTarget")
                {
                    buildTarget = args[i + 1];
                }
                else if (args[i] == "-outputPath")
                {
                    outputPath = args[i + 1];
                }
                else if (args[i] == "-outputFileName")
                {
                    outputFileName = args[i + 1];
                }
                else if (args[i] == "-configuration")
                {
                    configuration = args[i + 1];
                }
                else if (args[i] == "-buildNumber")
                {
                    buildNumber = args[i + 1];
                }
                else if (args[i] == "-appversion")
                {
                    appversion = args[i + 1];
                }
                else if (args[i] == "-env")
                {
                    environment = args[i + 1];
                }
                else if (args[i] == "-scriptingBackend")
                {
                    scriptingBackend = args[i + 1];
                }
                else if (args[i] == "-outputExtension")
                {
                    outputExtension = args[i + 1];
                }
#if UNITY_ANDROID
                else if (args[i] == "-buildAppBundle")
                {
                    buildAppBundle = args[i + 1];
                }
                else if (args[i] == "-splitApplicationBinary")
                {
                    splitApplicationBinary = args[i + 1];
                }
#endif
                else if (args[i] == "-scriptDefinedSymbols")
                {
                    scriptDefinedSymbols = args[i + 1];
                }
#if UNITY_IOS
                else if (args[i] == "-buildXcodeAppend")
                {
                    buildXcodeAppend = args[i + 1];
                }
#endif
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //options
            BuildOptions buildOptions = BuildOptions.None;

            switch (scriptingBackend)
            {
                case "Mono":
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.Mono2x);
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                    break;
                case "IL2CPP":
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
                    break;
            }


#if UNITY_IOS
            var path = outputPath;
            buildPlayerOptions.locationPathName = path;
            var b = UnityEditor.BuildPipeline.BuildCanBeAppended(BuildTarget.iOS, path);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Low);
            
            if (buildXcodeAppend == "true")
            {
                switch (b)
                {
                    case CanAppendBuild.Yes:
                        Debug.Log("Can append build");
                        buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;
                        break;
                    case CanAppendBuild.No:
                        Debug.Log("Can't append build");
                        break;
                    case CanAppendBuild.Unsupported:
                        Debug.Log("Unknown build");
                        break;
                }
            }
            else
            {
                Debug.Log("Skip append build");
            }
#endif

#if UNITY_ANDROID
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.Android.useCustomKeystore = false;


            switch (buildAppBundle)
            {
                case "true":
                    PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

                    EditorUserBuildSettings.buildAppBundle = true;
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);

                    //split application binary
                    if (splitApplicationBinary == "true")
                    {
                        PlayerSettings.Android.useAPKExpansionFiles = true;
                    }
                    else
                    {
                        PlayerSettings.Android.useAPKExpansionFiles = false;
                    }

                    break;
                case "false":
                    PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;

                    EditorUserBuildSettings.buildAppBundle = false;
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);

                    //split application binary
                    PlayerSettings.Android.useAPKExpansionFiles = false;
                    //minify release

                    break;
            }

            //PlayerSettings.Android.minifyRelease = true;
            //PlayerSettings.Android.minifyWithR8 = true;

            Debug.Log("output: " + buildPlayerOptions.locationPathName);
#endif
            var config = GetBuildConfig();
            if (!config)
            {
                Debug.LogError("Cannot find cloud build config");
            }

            //configuration
            switch (configuration)
            {
                case "Debug":
                    buildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
                    break;
                case "Release":
                case "Development":
                    buildPlayerOptions.options |= BuildOptions.None;
                    break;
                default:
                    buildPlayerOptions.options |= BuildOptions.None;
                    break;
            }


            //environment
            switch (environment)
            {
                case "UAT":
                    config.SetEnvironment(Environment.UAT, scriptDefinedSymbols);
                    break;
                case "Production":
                    config.SetEnvironment(Environment.Production, scriptDefinedSymbols);
                    break;
                case "Staging":
                    config.SetEnvironment(Environment.Staging, scriptDefinedSymbols);
                    break;
            }

            //target
            switch (buildTarget)
            {
                case "Android":
                    buildPlayerOptions.target = BuildTarget.Android;
                    PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
                    PlayerSettings.bundleVersion = appversion;
                    break;
                case "iOS":
                    buildPlayerOptions.target = BuildTarget.iOS;
                    PlayerSettings.iOS.buildNumber = buildNumber;
                    PlayerSettings.bundleVersion = appversion;
                    break;
                case "StandaloneWindows":
                    buildPlayerOptions.target = BuildTarget.StandaloneWindows;
                    PlayerSettings.bundleVersion = appversion;
                    //windows 32 bit output file
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);
                    break;
                case "StandaloneWindows64":
                    buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
                    PlayerSettings.bundleVersion = appversion;
                    //windows 64 bit output file
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);

                    break;
                case "StandaloneOSX":
                    buildPlayerOptions.target = BuildTarget.StandaloneOSX;
                    PlayerSettings.macOS.buildNumber = buildNumber;
                    PlayerSettings.bundleVersion = appversion;
                    //mac output file
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);
                    break;
                case "WebGL":
                    buildPlayerOptions.target = BuildTarget.WebGL;
                    PlayerSettings.bundleVersion = appversion;
                    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
                    PlayerSettings.WebGL.memorySize = 512;
                    PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
                    PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
                    //webgl output file
                    buildPlayerOptions.locationPathName = Path.Combine(outputPath, outputFileName);

                    break;
                default:
                    buildPlayerOptions.target = BuildTarget.StandaloneWindows;
                    break;
            }

            AssetDatabase.SaveAssets();
#if UNITY_ANDROID
            Debug.Log("Android Bundle version code: " + PlayerSettings.Android.bundleVersionCode);
#endif

            buildPlayerOptions.options = buildOptions;
            //scenes
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();


            UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        public static ProjectBuildConfiguration GetBuildConfig()
        {
            //find build config
            var buildConfig = AssetDatabase.FindAssets("t:ProjectBuildConfiguration")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ProjectBuildConfiguration>)
                .FirstOrDefault();

            if (buildConfig == null)
                throw new Exception("Cannot find build config");

            return buildConfig;
        }
    }
}