using UnityEngine;
using UnityGameFramework.Runtime;

namespace EdgeShimmer
{
    public partial class GameEntry : MonoBehaviour
    {
        public BaseComponent Base;
        public DataTableComponent DataTable;
        public DataNodeComponent DataNode;
        public DebuggerComponent Debugger;
        public DownloadComponent Download;
        public EntityComponent Entity;
        public EventComponent Event;
        public FsmComponent Fsm;
        public LocalizationComponent Localization;
        public NetworkComponent Network;
        public ObjectPoolComponent ObjectPool;
        public ProcedureComponent Procedure;
        public ResourceComponent Resource;
        public SceneComponent Scene;
        public SettingComponent Setting;
        public SoundComponent Sound;
        public UIComponent UI;
        public WebRequestComponent WebRequest;

         void InitGameFrameWorkComponents()
        {
            Base = UnityGameFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
            DataNode = UnityGameFramework.Runtime.GameEntry.GetComponent<DataNodeComponent>();
            DataTable = UnityGameFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
            Debugger = UnityGameFramework.Runtime.GameEntry.GetComponent<DebuggerComponent>();
            Download = UnityGameFramework.Runtime.GameEntry.GetComponent<DownloadComponent>();
            Entity = UnityGameFramework.Runtime.GameEntry.GetComponent<EntityComponent>();
            Event = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            Fsm = UnityGameFramework.Runtime.GameEntry.GetComponent<FsmComponent>();
            Localization = UnityGameFramework.Runtime.GameEntry.GetComponent<LocalizationComponent>();
            Network = UnityGameFramework.Runtime.GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = UnityGameFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = UnityGameFramework.Runtime.GameEntry.GetComponent<ProcedureComponent>();
            Resource = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            Scene = UnityGameFramework.Runtime.GameEntry.GetComponent<SceneComponent>();
            Setting = UnityGameFramework.Runtime.GameEntry.GetComponent<SettingComponent>();
            Sound = UnityGameFramework.Runtime.GameEntry.GetComponent<SoundComponent>();
            UI = UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
            WebRequest = UnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();
        }
    }
}

