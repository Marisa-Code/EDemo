
namespace EdgeShimmer.Save
{
    public class PlayerDataContainer
    {
        /// <summary>
        /// 存档文件
        /// </summary>
        private PlayerData m_playerData;
        
        public PlayerData PlayerData
        {
            get { return m_playerData; }
            set { m_playerData = value; }
        }
        public PlayerDataContainer()
        {
            m_playerData = new PlayerData();
        }
    }
}
