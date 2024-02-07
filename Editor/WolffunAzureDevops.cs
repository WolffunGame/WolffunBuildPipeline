using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
#if ADDRESSABLES_ENABLED 
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;

namespace Wolffun.BuildPipeline
{
    using UnityEditor;
    public sealed class WolffunAzureDevops
    {
        static string buildTarget = "";
        static string outputPath = "";
        static string outputFileName = "";
        static string configuration = "Development";
        static string buildNumber = "0";
        static string appversion = "0.0.0";
        static string environment = "";
        static string scriptingBackend = "Mono";
        static string outputExtension = "";
        static string scriptDefinedSymbols = "";
        static string typeBundle = "Addressables";
        static string assetBundle = "false";
        static string screenMode = "FullScreenWindow";
        static string defaultScreenHeight = "1080";
        static string defaultScreenWidth = "1920";
        static string runInBackground = "false";
        static string forceSingleInstance = "false";
        static string addressableRule = "";
        static string enableAddressableRule = "false";
        private static string buildServer = "false";
#if UNITY_ANDROID
           static string splitApplicationBinary = "false";
           static string androidCreateSymbols = "false";
#endif
        
        
        static string il2cppCodegen = "OptimizeSpeed";
        static string buildAddressables = "true";
        

#if UNITY_IOS
          static  string xcodeBuildConfig = "Release";
          static  string buildXcodeAppend = "true";
#endif
#if UNITY_ANDROID
           static string buildAppBundle = "true";
#endif
        static string customScenesToBuild = "";
#region Addressable
        public static void AddRessableBuild()
        {
            
            Debug.Log("Start build Addressable");
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Can't find setting");
                PowerShellAndBashMethod.RunErrorBash();
            }
            var mode = ScriptableObject.CreateInstance<BuildScriptsAzurePipeline>();
            var res = mode.BuildData<AddressablesPlayerBuildResult>(new AddressablesDataBuilderInput(settings));
        
            if (!string.IsNullOrEmpty(res.Error))
            {
                Debug.LogError(res.Error);
                PowerShellAndBashMethod.RunErrorBash();
            }
        }
#endregion
#region BuildPlayer
        public static void PerformBuild()
        {
            //get commandline arguments -outputPath $(Build.BinariesDirectory)
            //-outputFileName $(XcodeProjectFolderName) -configuration $(Configuration) -buildNumber $(BuildNumber) -appversion $(AppVersion)
            string[] args = System.Environment.GetCommandLineArgs();
            //log all arguments joined by space
            Debug.Log(string.Join(" ", args));
            

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
                else if(args[i] == "-typeBundle")
                {
                    typeBundle = args[i + 1];
                }
                else if(args[i] == "-assetBundle")
                {
                    assetBundle = args[i + 1];
                }
                else if(args[i] == "-addressableRule")
                {
                    addressableRule = args[i + 1];
                }
                else if(args[i] == "-enableAddressableRule")
                {
                    enableAddressableRule = args[i + 1];
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
                else if (args[i] == "-androidCreateSymbols")
                {
                    androidCreateSymbols = args[i + 1];
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
                else if (args[i] == "-customScenesToBuild")
                {
                    customScenesToBuild = args[i + 1];
                }
#if UNITY_IOS
                else if (args[i] == "-xcodeBuildConfig")
                {
                    xcodeBuildConfig = args[i + 1];
                }
#endif
#if UNITY_STANDALONE

                else if (args[i] == "-screenMode")
                {
                    screenMode = args[i + 1];
                }
                else if (args[i] == "-runInBackground")
                {
                    runInBackground = args[i + 1];
                }
                else if (args[i] == "-foreSingleInstance")
                {
                    forceSingleInstance = args[i + 1];
                }
                else if (args[i] == "-defaultScreenHeight")
                {
                    defaultScreenHeight = args[i + 1];
                }
                else if (args[i] == "-defaultScreenWidth")
                {
                    defaultScreenWidth = args[i + 1];
                }
                
#endif            
                else if (args[i] == "-il2cppCodegen")
                {
                    il2cppCodegen = args[i + 1];
                }

                else if (args[i] == "-buildAddressables")
                {
                    buildAddressables = args[i + 1];
                }
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            PlayerSettings.resetResolutionOnWindowResize = true;
            //options
#if UNITY_STANDALONE

            switch (screenMode)
            {
                case "FullScreenWindow":
                    PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case "MaximizedWindow":
                    PlayerSettings.fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case "Windowed":
                    PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                    if (int.TryParse(defaultScreenHeight, out var defaultScreenHeightInt))
                    {
                        PlayerSettings.defaultScreenHeight = defaultScreenHeightInt;
                    }
                    if (int.TryParse(defaultScreenWidth, out var defaultScreenWidthInt))
                    {
                        PlayerSettings.defaultScreenWidth = defaultScreenWidthInt;
                    }
                    break;
            }

            PlayerSettings.runInBackground = runInBackground == "true";
            
            PlayerSettings.forceSingleInstance = forceSingleInstance == "true";
#endif       
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

            switch (il2cppCodegen)
            {
                case "OptimizeSize":
                    SetIl2CppCodeGeneration(buildTarget, Il2CppCodeGeneration.OptimizeSize);
                    break;
                case "OptimizeSpeed":
                    SetIl2CppCodeGeneration(buildTarget, Il2CppCodeGeneration.OptimizeSpeed);
                    break;
                default:
                    break;
            }

            try
            {
                if (typeBundle == "Addressables")
                {
                    var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
                    settings.BuildAddressablesWithPlayerBuild =
                        AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;
                    // Debug.Log("Start check Addressable");
                    // if (buildAddressables == "true")
                    // {
                    //     Debug.Log("Start check Addressable: build addressable");
                    //     //get AddressableAssetSettings
                    //     var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
                    //     settings.BuildAddressablesWithPlayerBuild =
                    //         AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
                    // }
                    // else
                    // {
                    //     Debug.Log("Start check Addressable: dont build addressable");
                    //     var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
                    //     settings.BuildAddressablesWithPlayerBuild =
                    //         AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;
                    // }
                }
                else if(typeBundle == "AssetBundle")
                {
                    if (assetBundle == "true")
                    {
                        BuildBundle();  
                    }
                }
                
            }
            catch (Exception) 
            { 
                // ignored
            }

#if UNITY_IOS
            var path = outputPath;
            buildPlayerOptions.locationPathName = path;
            var b = UnityEditor.BuildPipeline.BuildCanBeAppended(BuildTarget.iOS, path);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Low);

            if(xcodeBuildConfig == "Release")
            {
                EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
            }
            else
            {
                EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Debug;
            }
            
            if (buildXcodeAppend == "true")
            {
                switch (b)
                {
                    case CanAppendBuild.Yes:
                        Debug.Log("Can append build");
                        buildPlayerOptions.options |= BuildOptions.AcceptExternalModificationsToPlayer;
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

            //create symbols zip
            if (androidCreateSymbols == "true")
            {
                EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Public;
            }
            else
            {
                EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
            }

            switch (buildAppBundle)
            {
                case "true":
                    PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

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
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

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
                    buildPlayerOptions.options |= BuildOptions.Development | BuildOptions.AllowDebugging;
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

            if (buildServer == "true")
            {
                buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
                EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Server;
                PlayerSettings.bundleVersion = appversion;
                //linux output file
                buildPlayerOptions.locationPathName =
                    Path.Combine(outputPath, outputFileName + "." + outputExtension);
            }
            else
            { 
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
                case "LinuxServer64":
                    buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
                    EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Server;
                    PlayerSettings.bundleVersion = appversion;
                    //linux output file
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);

                    break;
                //Windows Server
                case "WindowsServer":
                    buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
                    EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Server;
                    PlayerSettings.bundleVersion = appversion;
                    //windows 64 bit output file
                    buildPlayerOptions.locationPathName =
                        Path.Combine(outputPath, outputFileName + "." + outputExtension);

                    break;

                default:
                    buildPlayerOptions.target = BuildTarget.StandaloneWindows;
                    break; 
                } 
            }
            //target
            

            //disable logo unity
            try
            {
                PlayerSettings.SplashScreen.showUnityLogo = false;
            }
            catch (Exception)
            {
                // ignored
            }

            SetupAddressableRule();
            SetupAddressableRule();
            AssetDatabase.SaveAssets();
#if UNITY_ANDROID
            Debug.Log("Android Bundle version code: " + PlayerSettings.Android.bundleVersionCode);
#endif


            //scenes
            try
            {
                if (string.IsNullOrEmpty(customScenesToBuild))
                {
                    buildPlayerOptions.scenes =
                        EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                }
                else
                {
                    var sceneList = customScenesToBuild.Split(',');
                    buildPlayerOptions.scenes = sceneList.Select(s => s.Trim()).ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing custom scenes to build: " + e.Message);
                buildPlayerOptions.scenes =
                    EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            }


            UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        public static void SetIl2CppCodeGeneration(string targetName, Il2CppCodeGeneration codeGeneration)
        {
#if UNITY_2022_1_OR_NEWER
            NamedBuildTarget target;
            BuildTargetGroup targetGroup = BuildTargetGroup.iOS;
            switch (targetName)
            {
                case "Android":
                    target = NamedBuildTarget.Android;
                    targetGroup = BuildTargetGroup.Android;
                    break;
                case "iOS":
                    target = NamedBuildTarget.iOS;
                    targetGroup = BuildTargetGroup.iOS;
                    break;
                case "StandaloneWindows":
                    target = NamedBuildTarget.Standalone;
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case "StandaloneWindows64":
                    target = NamedBuildTarget.Standalone;
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case "StandaloneOSX":
                    target = NamedBuildTarget.Standalone;
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case "WebGL":
                    target = NamedBuildTarget.WebGL;
                    targetGroup = BuildTargetGroup.WebGL;
                    break;
                case "LinuxServer64":
                    target = NamedBuildTarget.Server;
                    targetGroup = BuildTargetGroup.LinuxHeadlessSimulation;
                    break;
                case "WindowsServer":
                    target = NamedBuildTarget.Server;
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
            }


            if (PlayerSettings.GetScriptingBackend(targetGroup) != ScriptingImplementation.IL2CPP) return;

            PlayerSettings.SetIl2CppCodeGeneration(target, codeGeneration);
#else
            EditorUserBuildSettings.il2CppCodeGeneration = codeGeneration;
#endif
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
        public static void BuildBundle()
        {
            string outputPath = "Assets/StreamingAssets";
            BuildTarget target = BuildTarget.NoTarget;
            switch (buildTarget)
            {
                case "Android":
                    target = BuildTarget.Android;
                    break;
                case "iOS":
                    target = BuildTarget.iOS;
                    break;
                case "StandaloneWindows":
                    target = BuildTarget.StandaloneWindows;
                    break;
                case "StandaloneWindows64":
                    target = BuildTarget.StandaloneWindows64;
                    break;
                case "StandaloneOSX":
                    target = BuildTarget.StandaloneOSX;
                    break;
                case "WebGL":
                    target = BuildTarget.WebGL;
                    break;
                case "LinuxServer64":
                    target = BuildTarget.StandaloneLinux64;
                    break;
                case "WindowsServer":
                    target = BuildTarget.StandaloneWindows64;
                    break;
            }
            ClearFolder(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AssetBundleStripUnityVersion, target);
        }
        public static void SetupAddressableRule()
        {
            if (!string.IsNullOrEmpty(addressableRule))
            {
                if (enableAddressableRule == "true")
                {
                    var result = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(addressableRule);
                    if (result)
                    {
                        result.rulesEnabled = false;
                    }
                }
                return;
            }
            Debug.LogError("Addressable rule don't exist");
        }
        private static void ClearFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                // Lấy danh sách tất cả các tệp trong thư mục
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    File.Delete(file); // Xóa từng tệp
                }

                // Lấy danh sách tất cả các thư mục con
                string[] subfolders = Directory.GetDirectories(folderPath);
                foreach (string subfolder in subfolders)
                {
                    Directory.Delete(subfolder, true); // Xóa tất cả các thư mục con và nội dung của chúng
                }
            }
            else
            {
                // Nếu thư mục không tồn tại, tạo mới nój
                Directory.CreateDirectory(folderPath);
            }
        }
#endregion


    } 
}