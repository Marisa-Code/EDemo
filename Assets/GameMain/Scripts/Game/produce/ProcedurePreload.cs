using System.Collections;
using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

public class ProcedurePreload : ProcedureBase
{
    public readonly static string[] DataTableNames = new string[]{"Aircraft"};
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
    }
}
