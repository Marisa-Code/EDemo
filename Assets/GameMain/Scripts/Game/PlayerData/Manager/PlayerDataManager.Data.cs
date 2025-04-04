

using System;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace EdgeShimmer
{
    public partial class PlayerDataManager : GameFrameworkComponent
    {
        private ISaveHelper m_SaveHelper;

        private SaveDatas m_SaveDatas;


        protected override void Awake()
        {
            base.Awake();
            m_SaveDatas = new SaveDatas();
            m_SaveHelper = new EasySaveHelper();//后面可加云存档，根据云存档实例化
        }
        
        /// <summary>
        /// 进入游戏后加载存档或创建
        /// </summary>
        public void InitPlayerData()
        {
            m_SaveHelper.InitSaveDataFile();
            if (m_SaveHelper.GetAllSaveName().Length <= 0)
            {
                SavePlayerData();
                SavePlayerWeaponData();
            }
            else
            {
                LoadAllData();
            }
        }

        private async Task LoadAllData()
        {
            try {
                await Task.Run(() =>  m_SaveDatas.PlayerData = m_SaveHelper.LoadFromSlot<PlayerData>(GameDataConfigs.DATA_NAME));
                await Task.Run(() =>
                    m_SaveDatas.WeaponData = m_SaveHelper.LoadFromSlot<PlayerWeaponData>(GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }

        }
        public async Task SavePlayerData()
        {
            try {
                await Task.Run(() => m_SaveHelper.WriteFromSlot(m_SaveDatas.PlayerData, GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        public async Task SavePlayerDataAsync() {
            try {
                await Task.Run(() => m_SaveHelper.WriteFromSlot(m_SaveDatas.PlayerData, GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
        public async  Task DeletePlayerData()
        {
            try {
                m_SaveDatas.PlayerData = new PlayerData();
                await Task.Run(() => m_SaveHelper.WriteFromSlot(m_SaveDatas.PlayerData,GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

    }
}
