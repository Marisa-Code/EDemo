
namespace EdgeShimmer.Save
{
    public class WeaponDataContainer
    {
        /// <summary>
        /// 存档文件
        /// </summary>
        private PlayerWeaponData m_WeaponData;
        public PlayerWeaponData WeaponData
        {
            get { return m_WeaponData; }
            set { m_WeaponData = value; }
        }
        
        public WeaponDataContainer()
        {
            m_WeaponData = new PlayerWeaponData();
        }
    }
}
