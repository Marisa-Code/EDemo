

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;

namespace EdgeShimmer.Save
{
    public class PlayerDataManager : GameFrameworkComponent
    {
        private List<IDataController> m_ControllerList;
        private ISaveHelper m_SaveHelper;
        protected override void Awake()
        {
            base.Awake();
            m_SaveHelper = EasySaveHelperCreater.CreateHelper();
            AddDataControllers();
            
        }

        private void AddDataControllers()
        {
            var interfaceType = typeof(IDataController);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            m_ControllerList = new List<IDataController>();
            foreach (var type in types)
            {
                m_ControllerList.Add((IDataController)Activator.CreateInstance(type,m_SaveHelper));
            }
        }
        
        /// <summary>
        /// 进入游戏后加载存档或创建
        /// </summary>
        public async Task InitPlayerData()
        {
            m_SaveHelper.InitSaveDataFile();
            if (m_SaveHelper.GetAllSaveName().Length <= 0)
            {
                foreach (var dataController in m_ControllerList)
                {
                    await dataController.SaveData();
                }
            }
            else
            {
                await LoadAllData();
            }
        }

        private async Task LoadAllData()
        {
            try
            {
                foreach (var dataController in m_ControllerList)
                {
                    await dataController.LoadData();
                }
            }
            catch (Exception e)
            {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }
        
        
        
        
        public async Task SaveData<T>() where T : IDataController
        {
            try
            {
                foreach (var dataController in m_ControllerList.Where(x => x.GetType() == typeof(T)))
                {
                    await dataController.SaveData();
                    break;
                }
            }
            catch (Exception e)
            {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

        internal async Task DeleteData<T>() where T : IDataController
        {
            try
            {
                foreach (var dataController in m_ControllerList.Where(x => x.GetType() == typeof(T)))
                {
                    await dataController.DeleteData();
                    break;
                }
            }
            catch (Exception e)
            {
                GameFrameworkLog.Error($"Save failed: {e.Message}");
            }
        }

    }
}
