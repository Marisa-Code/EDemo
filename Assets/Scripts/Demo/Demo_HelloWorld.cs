using GameFramework;
using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class Demo_HelloWorld : ProcedureBase
{
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        string message = "HelloWorld!";
        GameFrameworkLog.Info(message);
        GameFrameworkLog.Warning(message);
        GameFrameworkLog.Error(message);
    }
}
