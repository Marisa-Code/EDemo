

using System;
using System.Threading.Tasks;
using GameFramework;

namespace EdgeShimmer.Save
{
    internal sealed class PlayerDataController : IDataController
    {

        public ISaveHelper SaveHelper { get; set; }
        public PlayerDataBase PlayerSaveData { get; set; }

        public PlayerDataController(ISaveHelper baseHelper)
        {
            SaveHelper = baseHelper;
            PlayerSaveData = new PlayerData();
        }

        public async Task SaveData()
        {
            try {
                await Task.Run(() => SaveHelper.WriteFromSlot(PlayerSaveData, GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }


        public async Task LoadData()
        {
            try {
                await Task.Run(() =>  PlayerSaveData = SaveHelper.LoadFromSlot<PlayerData>(GameDataConfigs.DATA_NAME));

            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        public async Task DeleteData()
        {
            try {
                PlayerSaveData = new PlayerData();
                await Task.Run(() => SaveHelper.WriteFromSlot(PlayerSaveData,GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
    }
}

