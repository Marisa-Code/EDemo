using System.Collections;
using System.Collections.Generic;
using GameFramework.FileSystem;
using UnityEngine;

namespace  EdgeShimmer
{
    public interface ISaveHelper
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="slot"></param>
        /// <typeparam name="T"></typeparam>
        public void InitSaveDataFile();

        /// <summary>
        /// 写入存档到系统文件
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void WriteFromSlot<T>(T data, string name) where T : PlayerDataBase;

        /// <summary>
        /// 从指定存档槽加载数据(反序列化)
        /// </summary>
        public T LoadFromSlot<T>(string name) where T : PlayerDataBase;

        /// <summary>
        /// 删除系文件
        /// </summary>
        public void DeleteSystem();

        /// <summary>
        /// 删除指定存档槽的文件
        /// </summary>
        public void DeleteData(string name);

        /// <summary>
        /// 获取所有存档文件信息
        /// </summary>
        /// <returns></returns>
        public FileInfo[] GetAllSaveName();
    }

}
