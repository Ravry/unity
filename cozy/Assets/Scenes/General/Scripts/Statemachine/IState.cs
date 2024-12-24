using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<EStates> where EStates : Enum
{
    public void EnterState();

    public EStates CheckState();
    public void Update();
    public void FixedUpdate();
    public void ExitState();
}
