using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

public class Demo2_ProcedureMenu : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        GameFrameworkLog.Debug("进入菜单流程，可以在这里加载菜单UI。");

        var res = GameEntry.GetComponent<ResourceComponent>();
        res.InitResources(OnInitResourcesComplete);
    }
    private void OnInitResourcesComplete()
    {
        var ui = UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
        ui.OpenUIForm("Assets/GameMain/Resources/Prefab/UI_Menu.prefab", "DefaultGroup");
    }
}
