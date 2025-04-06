using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EdgeShimmer.Save
{
    public interface IDataController
    {
        public ISaveHelper SaveHelper { get; set; }
        
        public PlayerDataBase PlayerSaveData { get; set; }

        public Task SaveData();

        public Task LoadData();

        public Task DeleteData();

    }
}

