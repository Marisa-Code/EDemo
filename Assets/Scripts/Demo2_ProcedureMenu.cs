using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

public class Demo2_ProcedureMenu : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        GameFrameworkLog.Debug("����˵����̣�������������ز˵�UI��");
    }

}
