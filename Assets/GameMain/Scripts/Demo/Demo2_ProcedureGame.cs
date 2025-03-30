
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework;
public class Demo2_ProcedureGame : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        GameFrameworkLog.Debug("进入游戏流程，可以在这里处理游戏逻辑，这条日志不会打印，因为没有切换到Game流程");
    }
}
