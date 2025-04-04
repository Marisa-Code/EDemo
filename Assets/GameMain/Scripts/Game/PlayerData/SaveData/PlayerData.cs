using System;
using UnityEngine;

namespace EdgeShimmer
{
    /// <summary>
    /// 玩家数据基础类,可以有别的数据类继承此类,根据不同类分块存储
    /// </summary>
    [Serializable]
    public class PlayerData : PlayerDataBase
    {
        [SerializeField]
        private string m_PlayerName = "";

        /// <summary>
        /// 存档升级版本后的迁移逻辑
        /// </summary>
        [SerializeField]
        private float m_Virsion = 1.0f;
        
        public string PlayerName { 
            get => m_PlayerName; 
            set => m_PlayerName = value;
        }
        public float Virsion { 
            get => m_Virsion; 
            set => m_Virsion = value;
        }
    }
}

