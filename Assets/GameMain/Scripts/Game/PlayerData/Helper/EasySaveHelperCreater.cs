using System;

namespace EdgeShimmer.Save
{
    public static class EasySaveHelperCreater
    {
        /// <summary>
        /// 创建辅助器。
        /// </summary>
        /// <typeparam name="T">要创建的辅助器类型。</typeparam>
        /// <returns>创建的辅助器。</returns>
        internal static ISaveHelper CreateHelper()
        {
            var isCloud = GameEntry.Setting.GetBool("isCloud");
            return isCloud ? Activator.CreateInstance<EasyCloudSaveHelper>() :Activator.CreateInstance<EasySaveHelper>();
        }        
    }
}