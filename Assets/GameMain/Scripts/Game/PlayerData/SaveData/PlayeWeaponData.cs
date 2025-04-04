using System;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeShimmer.Save
{
    /// <summary>
    /// 玩家数据武器类
    /// </summary>
    [Serializable]
    public class PlayerWeaponData : PlayerDataBase
    {
        private List<WeaponData> m_weaponList;
    
        public List<WeaponData> WeaponList { 
            get => m_weaponList; 
            set => m_weaponList = value;
        }
    }

    [Serializable]
    public class WeaponData
    {
        /// <summary>
        /// 武器名
        /// </summary>
        public string weaponName;

        /// <summary>
        /// id
        /// </summary>
        public int weaponId;
    }
}

