
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework;
public class Demo2_ProcedureGame : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        GameFrameworkLog.Debug("������Ϸ���̣����������ﴦ����Ϸ�߼���������־�����ӡ����Ϊû���л���Game����");
    }
}
