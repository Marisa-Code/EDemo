using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

public class Lancher : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        GameFrameworkLog.Debug("≥ı º");
        SceneComponent scene = GameEntry.GetComponent<SceneComponent>();
        scene.LoadScene("Assets/Scenes/Demo2_Menu.unity", this);

        ChangeState<Demo2_ProcedureMenu>(procedureOwner);
    }
}
