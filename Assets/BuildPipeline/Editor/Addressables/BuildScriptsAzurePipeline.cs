using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildScriptsAzurePipeline.asset", menuName = "Addressables/Custom Build/BuildScriptsAzurePipeline")]
public class BuildScriptsAzurePipeline : BuildScriptPackedMode
{
    protected override string ProcessGroup(AddressableAssetGroup assetGroup, AddressableAssetsBuildContext aaContext)
    {
        if (assetGroup == null)
            return string.Empty;

        if (assetGroup.Schemas.Count == 0)
        {
            return string.Empty;
        }

        foreach (var schema in assetGroup.Schemas)
        {
            Debug.LogError(assetGroup.Name);
            var errorString = ProcessGroupSchema(schema, assetGroup, aaContext);
            if (!string.IsNullOrEmpty(errorString))
                return errorString;
        }

        return string.Empty;
    }
}
