
namespace EdgeShimmer
{
    public class SaveDatas
    {
        /// <summary>
        /// 存档文件
        /// </summary>
        private PlayerData _playerData;
        private PlayerWeaponData _weaponData;
        
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        public PlayerWeaponData WeaponData
        {
            get { return _weaponData; }
            set { _weaponData = value; }
        }

        public SaveDatas()
        {
            _playerData = new PlayerData();
            _weaponData = new PlayerWeaponData();
        }
    } 
}

