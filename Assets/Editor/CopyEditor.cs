using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyEditor
{
#if UNITY_EDITOR
    private const string ScriptAssembliesAotDir = "HybridCLRData/AssembliesPostIl2CppStrip/";
    private const string ScriptAssembliesDir = "HybridCLRData/HotUpdateDlls/";
    private const string CodeDir = "Assets/AssetBundle/Dll/Code/";
    private const string MetadataCodeDir = "Assets/AssetBundle/Dll/MetadataCode/";
    private const string HotfixDll = "HotFix.dll";

    //[MenuItem("Tools/CopyHotDll")]
    static void CopyHotDll()
    {
        File.Copy(Path.Combine(ScriptAssembliesDir + EditorUserBuildSettings.activeBuildTarget, HotfixDll), Path.Combine(CodeDir, HotfixDll + ".bytes"), true);
        Debug.Log($"复制Hotfix.dll到Res/Code完成");
    }

    //[MenuItem("Tools/CopyAotHotDll")]
    static void CopyAotHotDll()
    {
        for (int i = 0; i < GameConfigApp.AotDllNameList.Count; i++)
        {
            File.Copy(Path.Combine(ScriptAssembliesAotDir + EditorUserBuildSettings.activeBuildTarget, GameConfigApp.AotDllNameList[i]), Path.Combine(MetadataCodeDir, GameConfigApp.AotDllNameList[i] + ".bytes"), true);
        }
        Debug.Log($"复制补充元数据的dll到Res/MetadataCode完成");
    }

    [MenuItem("Tools/CopyAllHotDll")]
    static void CopyAllHotDll()
    {
        CopyHotDll();
        CopyAotHotDll();
        AssetDatabase.Refresh();
    }
#endif
}
