using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

public class Init : MonoBehaviour
{

    public EPlayMode PlayMode;

    ResourcePackage package;

    // Start is called before the first frame update
    void Start()
    {
        YooAssets.Initialize();

        package = YooAssets.CreatePackage("DefaultPackage");

        YooAssets.SetDefaultPackage(package);

        StartCoroutine(InitializeYooAsset());
    }

    private IEnumerator InitializeYooAsset()
    {
#if !UNITY_EDITOR
        PlayMode = EPlayMode.HostPlayMode;
#endif
        if (PlayMode == EPlayMode.HostPlayMode)
        {
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            initParameters.CacheFileSystemParameters = cacheFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                StartCoroutine(UpdatePackageVersion());
                Debug.Log("资源包初始化成功！");
            }
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
        else
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
            if (initOperation.Status == EOperationStatus.Succeed)
            {
                StartCoroutine(UpdatePackageVersion());
                Debug.Log("资源包初始化成功！");
            }

            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }

    }

    private string GetHostServerURL()
    {
        string headResPath = "https://luanshi-1300652558.cos.ap-guangzhou.myqcloud.com/Yoo";
        string gameResVersion = GameConfigApp.gameResVersion;

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{headResPath}/Android/{gameResVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{headResPath}/IPhone/{gameResVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{headResPath}/WebGL/{gameResVersion}";
        else
            return $"{headResPath}/PC/{gameResVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{headResPath}/Android/{gameResVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{headResPath}/IPhone/{gameResVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{headResPath}/WebGL/{gameResVersion}";
        else
            return $"{headResPath}/PC/{gameResVersion}";
#endif
    }


    private IEnumerator UpdatePackageVersion()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            string packageVersion = operation.PackageVersion;
#if UNITY_EDITOR
            StartCoroutine(UpdatePackageManifest(packageVersion));
#else
            StartCoroutine(UpdatePackageManifest(packageVersion));
#endif
            Debug.Log($"Request package Version : {packageVersion}");
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
        }
    }

    private IEnumerator UpdatePackageManifest(string packageVersion)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
#if UNITY_EDITOR
            StartCoroutine(RunHofixDllInit());
#else
            StartCoroutine(Download());
#endif
        }
        else
        {
            Debug.LogError(operation.Error);
        }
    }

    IEnumerator Download()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        if (downloader.TotalDownloadCount == 0)
        {
            StartCoroutine(RunHofixDllInit());
            yield break;
        }

        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        //downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        //downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        //downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        downloader.BeginDownload();
        yield return downloader;

        //������ؽ��
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //���سɹ�
            StartCoroutine(RunHofixDllInit());
        }
        else
        {
            //����ʧ��
        }
    }

    //private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
    //{
    //    throw new NotImplementedException();
    //}

    //private void OnDownloadOverFunction(bool isSucceed)
    //{
    //    throw new NotImplementedException();
    //}

    //private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    //{
    //    throw new NotImplementedException();
    //}

    //private void OnDownloadErrorFunction(string fileName, string error)
    //{
    //    throw new NotImplementedException();
    //}

    IEnumerator RunHofixDllInit()
    {
        var package = YooAssets.GetPackage("DefaultPackage");

        List<TextAsset> aotDllList = new List<TextAsset>();
        for (int i = 0; i < GameConfigApp.AotDllNameList.Count; i++)
        {
            //var s = "E:/Unity/MyUnityPros/LikeElin/LikeElin/Assets/AssetBundle/Dll/MetaDataCode/MetaCode_" + GameConfigApp.AotDllNameList[i];
            AssetHandle metadataCodehandle = package.LoadAssetAsync<TextAsset>("MetaCode_" + GameConfigApp.AotDllNameList[i]);
            yield return metadataCodehandle;
            TextAsset metadatacodeobj = metadataCodehandle.AssetObject as TextAsset;
            aotDllList.Add(metadatacodeobj);
        }
        LoadMetadataForAOTAssemblies(aotDllList);


#if !UNITY_EDITOR
        AssetHandle codehandle = package.LoadAssetAsync<TextAsset>("Code_Hotfix.dll");
        yield return codehandle;
        TextAsset codeobj = codehandle.AssetObject as TextAsset;
        byte[] codeBytes = codeobj.bytes;

        // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
        Assembly hotUpdateAss = Assembly.Load(codeBytes);
#else
        // Editor��������أ�ֱ�Ӳ��һ��HotUpdate����
        var ss = System.AppDomain.CurrentDomain.GetAssemblies();
        Assembly hotUpdateAss = null;
        foreach (var item in ss)
        {
            if(item.GetName().Name == "HotFix")
            {
                hotUpdateAss = item;
                break;
            }
        }
#endif

        Type type = hotUpdateAss.GetType("HotFixMain");
        type.GetMethod("Run").Invoke(null, null);
        yield return null;
    }

    private static void LoadMetadataForAOTAssemblies(List<TextAsset> aotDllList)
    {
        foreach (var aotDll in aotDllList)
        {
            byte[] dllBytes = aotDll.bytes;
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDll.name}. ret:{err}");
        }
    }



    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}
