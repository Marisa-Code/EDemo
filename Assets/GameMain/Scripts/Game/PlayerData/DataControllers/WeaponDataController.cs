

using System;
using System.Threading.Tasks;
using GameFramework;

namespace EdgeShimmer.Save
{
    internal sealed class WeaponDataController : IDataController
    {
        public PlayerDataBase PlayerSaveData { get; set; }
        public ISaveHelper SaveHelper { get; set; }


        public WeaponDataController(ISaveHelper baseHelper)
        {
            SaveHelper = baseHelper;
            PlayerSaveData = new PlayerWeaponData();
        }
        public async Task SaveData()
        {
            try {
                await Task.Run(() => SaveHelper.WriteFromSlot(PlayerSaveData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }


        public async Task LoadData()
        {
            try {
                await Task.Run(() =>
                    PlayerSaveData = SaveHelper.LoadFromSlot<PlayerWeaponData>(GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        public async Task DeleteData()
        {
            try {
                
                PlayerSaveData = new PlayerWeaponData();
                await Task.Run(() => SaveHelper.WriteFromSlot(PlayerSaveData,GameDataConfigs.WEAPON_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
    }
}

