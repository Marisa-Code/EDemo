

using System;
using System.Threading.Tasks;
using GameFramework;

namespace EdgeShimmer.Save
{
    public class WeaponDataController : IDataController
    {
        private WeaponDataContainer m_weaponData = new();
        public ISaveHelper SaveHelper { get; set; }
        public PLayerDataValue PLayerDataValue { get; set; }

        public WeaponDataController(ISaveHelper baseHelper)
        {
            SaveHelper = baseHelper;
            PLayerDataValue = PLayerDataValue.PlayerWeaponData;
        }
        public async Task SaveData()
        {
            try {
                await Task.Run(() => SaveHelper.WriteFromSlot(m_weaponData.WeaponData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }


        public async Task LoadData()
        {
            try {
                await Task.Run(() =>
                    m_weaponData.WeaponData = SaveHelper.LoadFromSlot<PlayerWeaponData>(GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        public async Task DeleteData()
        {
            try {
                
                m_weaponData.WeaponData = new PlayerWeaponData();
                await Task.Run(() => SaveHelper.WriteFromSlot(m_weaponData.WeaponData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
    }
}

