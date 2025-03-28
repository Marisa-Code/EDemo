

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
    /// Զ����Դ��ַ��ѯ������
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


    // ��Դϵͳ����ģʽ

    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;



    //CDN��ַ

    public string DefaultHostServer = "http://192.168.3.3/Package/";

    public string FallbackHostServer = "http://192.168.3.3/Package/";



    //�ȸ��µ�dll����

    public string HotDllName = "Hotfix.dll";



    //��������,�˶���ǰΪAOT���е�Ԥ������󣬲������ȸ���

    public GameObject tx;


    //����Ԫ����dll���б�Yooasset�в���Ҫ����׺

    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()

  {

    "mscorlib.dll",

    "System.dll",

    "System.Core.dll",

    "UniTask.dll"

  };


    //��ȡ��Դ������

    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public static byte[] GetAssetData(string dllName)

    {

        return s_assetDatas[dllName];

    }


    void Start()

    {
        StartCoroutine(DownLoadAssetsByYooAssets(null));
    }


    #region Yooasset����

    /// <summary>

    /// ��ȡ������Ϣ

    /// </summary>

    /// <param name="onDownloadComplete"></param>

    /// <returns></returns>

    IEnumerator DownLoadAssetsByYooAssets(Action onDownloadComplete)

    {

        // 1.��ʼ����Դϵͳ

        YooAssets.Initialize();



        // ����Ĭ�ϵ���Դ��

        var package = YooAssets.CreatePackage("DefaultPackage");



        // ���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�

        YooAssets.SetDefaultPackage(package);



        if (PlayMode == EPlayMode.EditorSimulateMode)
        {
            //�༭��ģ��ģʽ
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("��Դ����ʼ���ɹ���");
            else
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");

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
                Debug.Log("��Դ����ʼ���ɹ���");
            else
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");

        }
        else if (PlayMode == EPlayMode.OfflinePlayMode)
        {
            //����ģʽ
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("��Դ����ʼ���ɹ���");
            else
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");

        }
        else if (PlayMode == EPlayMode.WebPlayMode)
        {
            //˵����RemoteServices�ඨ����ο���������ģʽ��
            IRemoteServices remoteServices = new RemoteServices(DefaultHostServer, FallbackHostServer);
            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //֧�ֿ�������

            var initParameters = new WebPlayModeParameters();
            initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
            initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;

            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("��Դ����ʼ���ɹ���");
            else
                Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");
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
            // ֱ�ӵ�����һ��Э�̲����ݲ���
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
            Debug.Log("�嵥���³ɹ�");
            yield return Download();
        }
        else
        {
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>

    /// ��ȡ���ص���Ϣ��С����ʾ������

    /// </summary>

    /// <returns></returns>

    IEnumerator Download()

    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //û����Ҫ���ص���Դ
        if (downloader.TotalDownloadCount == 0)
        {
            yield break;
        }

        //��Ҫ���ص��ļ��������ܴ�С
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //ע��ص�����
        downloader.DownloadFinishCallback = OnDownloadFinishFunction; //�����������������۳ɹ���ʧ�ܣ�
        downloader.DownloadErrorCallback = OnDownloadErrorFunction; //����������������
        downloader.DownloadUpdateCallback = OnDownloadUpdateFunction; //�����ؽ��ȷ����仯
        downloader.DownloadFileBeginCallback = OnDownloadFileBeginFunction; //����ʼ����ĳ���ļ�

        //��������
        downloader.BeginDownload();
        yield return downloader;

        //������ؽ��
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //���سɹ�
        }
        else
        {
            //����ʧ��
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
    /// �����ļ�ϵͳ���еĻ�����Դ�ļ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageAllCacheBundleFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearAllBundleFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //����ɹ�
        }
        else
        {
            //����ʧ��
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// �����ļ�ϵͳδʹ�õĻ�����Դ�ļ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageUnusedCacheBundleFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //����ɹ�
        }
        else
        {
            //����ʧ��
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// �����ļ�ϵͳָ����ǩ�Ļ�����Դ�ļ�
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
            //����ɹ�
        }
        else
        {
            //����ʧ��
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// �����ļ�ϵͳ���еĻ����嵥�ļ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageAllCacheManifestFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearAllManifestFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //����ɹ�
        }
        else
        {
            //����ʧ��
            Debug.LogError(operation.Error);
        }
    }

    /// <summary>
    /// �����ļ�ϵͳδʹ�õĻ����嵥�ļ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPackageUnusedCacheManifestFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedManifestFiles);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //����ɹ�
        }
        else
        {
            //����ʧ��
            Debug.LogError(operation.Error);
        }
    }
    #endregion

    #region ��Դж��

    // ж���������ü���Ϊ�����Դ����
    // �������л�����֮�������Դ�ͷŷ�������д��ʱ�����ʱ��ȥ�ͷš�
    private IEnumerator UnloadUnusedAssets()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadUnusedAssetsAsync();
        yield return operation;
    }

    // ǿ��ж��������Դ�����÷������ں��ʵ�ʱ�����á�
    // ע�⣺Package�����ٵ�ʱ��Ҳ���Զ����ø÷�����
    private IEnumerator ForceUnloadAllAssets()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadAllAssetsAsync();
        yield return operation;
    }

    // ����ж��ָ������Դ����
    // ע�⣺�������Դ���ڱ�ʹ�ã��÷�������Ч��
    private void TryUnloadUnusedAsset(string path)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        package.TryUnloadUnusedAsset(path);//"Assets/GameRes/Panel/login.prefab");
    }

    #endregion


    /// <summary>

    /// Ϊaot assembly����ԭʼmetadata�� ��������aot�����ȸ��¶��С�

    /// һ�����غ����AOT���ͺ�����Ӧnativeʵ�ֲ����ڣ����Զ��滻Ϊ����ģʽִ��

    /// </summary>

    private static void LoadMetadataForAOTAssemblies()

    {

        /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�

        /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���

        HomologousImageMode mode = HomologousImageMode.SuperSet;

        foreach (var aotDllName in AOTMetaAssemblyNames)

        {

            byte[] dllBytes = GetAssetData(aotDllName);

            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����

            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);

            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");

        }

    }

}



