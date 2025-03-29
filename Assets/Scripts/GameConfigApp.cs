using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfigApp
{
    public static List<string> AotDllNameList = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    "YooAsset.dll",
    "UnityScripts.dll",
    //"HotFix.dll",
        "UnityEngine.JSONSerializeModule.dll"
    };

    public static string gameResVersion = "1.0";
}
