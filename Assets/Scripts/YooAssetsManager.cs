

using HybridCLR;

using System;

using System.Collections;

using System.Collections.Generic;

using System.Linq;

using UnityEditor;

using UnityEngine;

using UnityEngine.UI;

using YooAsset;



public class LoadDll : MonoBehaviour

{
    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
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


    // 资源系统运行模式

    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;



    //CDN地址

    public string DefaultHostServer = "http://192.168.3.3/Package/";

    public string FallbackHostServer = "http://192.168.3.3/Package/";



    //热更新的dll名称

    public string HotDllName = "Hotfix.dll";



    //弹窗对象,此对象当前为AOT层中的预制体对象，不放入热更新

    public GameObject tx;


    //补充元数据dll的列表，Yooasset中不需要带后缀

    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()

  {

    "mscorlib.dll",

    "System.dll",

    "System.Core.dll",

    "UniTask.dll"

  };


    //获取资源二进制

    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public static byte[] GetAssetData(string dllName)

    {

        return s_assetDatas[dllName];

    }


    void Start()

    {
        StartCoroutine(DownLoadAssetsByYooAssets(null));
    }


    #region Yooasset下载

    /// <summary>

    /// 获取下载信息

    /// </summary>

    /// <param name="onDownloadComplete"></param>

    /// <returns></returns>

    IEnumerator DownLoadAssetsByYooAssets(Action onDownloadComplete)

    {

        // 1.初始化资源系统

        YooAssets.Initialize();



        // 创建默认的资源包

        var package = YooAssets.CreatePackage("DefaultPackage");



        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。

        YooAssets.SetDefaultPackage(package);



        if (PlayMode == EPlayMode.EditorSimulateMode)
        {
            //编辑器模拟模式
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");

        }

        else if (PlayMode == EPlayMode.HostPlayMode)

        {
            IRemoteServices remoteServices = new RemoteServices(DefaultHostServer, FallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            initParameters.CacheFileSystemParameters = cacheFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");

        }
        else if (PlayMode == EPlayMode.OfflinePlayMode)
        {
            //单机模式
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");

        }
        else if (PlayMode == EPlayMode.WebPlayMode)
        {
            //说明：RemoteServices类定义请参考联机运行模式！
            IRemoteServices remoteServices = new RemoteServices(DefaultHostServer, FallbackHostServer);
            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

            var initParameters = new WebPlayModeParameters();
            initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
            initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;

            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }

        StartCoroutine(RequestPackageVersion());







    }


    private IEnumerator RequestPackageVersion()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            Debug.Log($"Request package Version : {operation.PackageVersion}");
            // 直接调用下一个协程并传递参数
            yield return StartCoroutine(UpdatePackageManifest(operation.PackageVersion));
        }
        else
        {
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
            Debug.Log("清单更新成功");
            yield return Download();
        }
        else
        {
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>

    /// 获取下载的信息大小，显示弹窗上

    /// </summary>

    /// <returns></returns>

    IEnumerator Download()

    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            yield break;
        }

        //需要下载的文件总数和总大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //注册回调方法
        downloader.DownloadFinishCallback = OnDownloadFinishFunction; //当下载器结束（无论成功或失败）
        downloader.DownloadErrorCallback = OnDownloadErrorFunction; //当下载器发生错误
        downloader.DownloadUpdateCallback = OnDownloadUpdateFunction; //当下载进度发生变化
        downloader.DownloadFileBeginCallback = OnDownloadFileBeginFunction; //当开始下载某个文件

        //开启下载
        downloader.BeginDownload();
        yield return downloader;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
        }
        else
        {
            //下载失败
        }
    }

    private void OnDownloadFileBeginFunction(DownloadFileData data)
    {
    }

    private void OnDownloadUpdateFunction(DownloadUpdateData data)
    {
    }

    private void OnDownloadErrorFunction(DownloadErrorData data)
    {
    }

    private void OnDownloadFinishFunction(DownloaderFinishData data)
    {
    }








    #endregion


    #region clear
    /// <summary>
    /// 清理文件系统所有的缓存资源文件
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageAllCacheBundleFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearAllBundleFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //清理成功
        }
        else
        {
            //清理失败
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// 清理文件系统未使用的缓存资源文件
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageUnusedCacheBundleFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //清理成功
        }
        else
        {
            //清理失败
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// 清理文件系统指定标签的缓存资源文件
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    private IEnumerator ClearPackageCacheBundleFilesByTags(string[] tags)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearBundleFilesByTags, tags);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //清理成功
        }
        else
        {
            //清理失败
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// 清理文件系统所有的缓存清单文件
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageAllCacheManifestFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearAllManifestFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //清理成功
        }
        else
        {
            //清理失败
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// 清理文件系统未使用的缓存清单文件
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageUnusedCacheManifestFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedManifestFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //清理成功
        }
        else
        {
            //清理失败
            Debug.LogError(operation.Error);
        }
    }
    #endregion

    #region 资源卸载

    // 卸载所有引用计数为零的资源包。
    // 可以在切换场景之后调用资源释放方法或者写定时器间隔时间去释放。
    private IEnumerator UnloadUnusedAssets()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadUnusedAssetsAsync();
        yield return operation;
    }

    // 强制卸载所有资源包，该方法请在合适的时机调用。
    // 注意：Package在销毁的时候也会自动调用该方法。
    private IEnumerator ForceUnloadAllAssets()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadAllAssetsAsync();
        yield return operation;
    }

    // 尝试卸载指定的资源对象
    // 注意：如果该资源还在被使用，该方法会无效。
    private void TryUnloadUnusedAsset(string path)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        package.TryUnloadUnusedAsset(path);//"Assets/GameRes/Panel/login.prefab");
    }

    #endregion


    /// <summary>

    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。

    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行

    /// </summary>

    private static void LoadMetadataForAOTAssemblies()

    {

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。

        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误

        HomologousImageMode mode = HomologousImageMode.SuperSet;

        foreach (var aotDllName in AOTMetaAssemblyNames)

        {

            byte[] dllBytes = GetAssetData(aotDllName);

            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码

            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);

            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");

        }

    }

}



