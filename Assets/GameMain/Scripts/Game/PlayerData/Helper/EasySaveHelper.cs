using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameFramework.FileSystem;
using UnityEngine;
using FileInfo = GameFramework.FileSystem.FileInfo;

namespace EdgeShimmer.Save
{
    /// <summary>
    /// 存储存档文件
    /// </summary>
    public class EasySaveHelper : ISaveHelper
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="slot"></param>
        /// <typeparam name="T"></typeparam>
        public void InitSaveDataFile()
        {
            var fullPath = GetSlotFilePath();
            GameEntry.Setting.SetString("SaveFullPath",fullPath);
            if (SystemExists()) return;
            GameEntry.FileSystem.CreateFileSystem(fullPath, FileSystemAccess.ReadWrite, maxFileCount: 5,
                maxBlockCount: 32); 
        }
        
        /// <summary>
        /// 写入存档到系统文件
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void WriteFromSlot<T>(T data,string name) where T : PlayerDataBase
        {
            var fullPath = GameEntry.Setting.GetString("SaveFullPath");
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            ms.ReadTimeout = 0;
            ms.WriteTimeout = 0;
            ms.Capacity = 0;
            ms.Position = 0;
            bf.Serialize(ms, data);
            var bytes = ms.ToArray();
            var fs = GameEntry.FileSystem.GetFileSystem(fullPath);
            var name1 = GetSlotKey(name);
            GameEntry.Setting.SetString(name1,name1);
            fs.WriteFile(name1, bytes); // 写入虚拟文件系统
        }

        /// <summary>
        /// 从指定存档槽加载数据(反序列化)
        /// </summary>
        public T LoadFromSlot<T>(string name)where T : PlayerDataBase
        {
            var fullPath = GameEntry.Setting.GetString("SaveFullPath");
            var fs = GameEntry.FileSystem.GetFileSystem(fullPath);
            var bytes = fs.ReadFile(GetSlotKey(name));
            if (bytes.Length <= 0)
            {
                return null;
            }
            using var ms = new MemoryStream(bytes);
            return (T)new BinaryFormatter().Deserialize(ms);
        }

        /// <summary>
        /// 删除系文件
        /// </summary>
        public void DeleteSystem()
        {
            var fullPath = GameEntry.Setting.GetString("SaveFullPath");
            var fs = GameEntry.FileSystem.GetFileSystem(fullPath);
            GameEntry.FileSystem.DestroyFileSystem(fs, false);
        }       
        
        /// <summary>
        /// 删除指定存档槽的文件
        /// </summary>
        public void DeleteData(string name)
        {
            var fullPath = GameEntry.Setting.GetString("SaveFullPath");
            var fs = GameEntry.FileSystem.GetFileSystem(fullPath);
            fs.DeleteFile(GetSlotKey(name));
        }

        /// <summary>
        /// 检查系统是否存在
        /// </summary>
        private bool SystemExists()
        {
            var filePath = GameEntry.Setting.GetString("SaveFullPath");
            // 检查是否存在文件系统，参数要传递的是文件系统的完整路径
            return GameEntry.FileSystem.HasFileSystem(filePath);
        }

        /// <summary>
        /// 获取所有存档文件信息
        /// </summary>
        /// <returns></returns>
        public FileInfo[] GetAllSaveName()
        {
            var fullPath = GameEntry.Setting.GetString("SaveFullPath");
            var fs = GameEntry.FileSystem.GetFileSystem(fullPath);
            return  fs.GetAllFileInfos();
        }


        // 获取系统文件路径
        private string GetSlotFilePath()
        {
            var name = GameDataConfigs.SAVE_FOLDER + "System";
            return Path.Combine(Application.persistentDataPath, name);
        }

        private string GetSlotKey(string name)
        {
            return $"{GameDataConfigs.SAVE_FOLDER}save_slot_{name}";
        }
    }
}