

using System;
using System.Threading.Tasks;
using GameFramework;

namespace EdgeShimmer.Save
{
    public class PlayerDataController : IDataController
    {
        private PlayerDataContainer m_playerData = new();
        public ISaveHelper SaveHelper { get; set; }
        public PLayerDataValue PLayerDataValue { get; set; }

        public PlayerDataController(ISaveHelper baseHelper)
        {
            SaveHelper = baseHelper;
            PLayerDataValue = PLayerDataValue.PlayerData;
        }

        public async Task SaveData()
        {
            try {
                await Task.Run(() => SaveHelper.WriteFromSlot(m_playerData.PlayerData, GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }


        public async Task LoadData()
        {
            try {
                await Task.Run(() =>  m_playerData.PlayerData = SaveHelper.LoadFromSlot<PlayerData>(GameDataConfigs.DATA_NAME));

            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        public async Task DeleteData()
        {
            try {
                m_playerData.PlayerData = new PlayerData();
                await Task.Run(() => SaveHelper.WriteFromSlot(m_playerData.PlayerData,GameDataConfigs.DATA_NAME));
            } catch (Exception e) {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
    }
}

