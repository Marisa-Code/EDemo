

using System;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace EdgeShimmer
{
    public partial class PlayerDataManager
    {
        public async Task SavePlayerWeaponData()
        {
            try {
                await Task.Run(() => m_SaveHelper.WriteFromSlot(m_SaveDatas.WeaponData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
        
        public async Task DeletePlayerWeaponData()
        {
            try {
                
                m_SaveDatas.WeaponData = new PlayerWeaponData();
                await Task.Run(() => m_SaveHelper.WriteFromSlot(m_SaveDatas.WeaponData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }

        }
    }
}
