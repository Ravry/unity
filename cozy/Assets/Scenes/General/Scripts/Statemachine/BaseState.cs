using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<EStates> : IState<EStates> where EStates : Enum
{
    public abstract EStates CheckState();

    public virtual void EnterState()
    {
        // Debug.Log($"Entering {this}");
    }
    
    public virtual void Update()
    {
        // Debug.Log($"Updating {this}");
    }

    public virtual void FixedUpdate()
    {
        // Debug.Log($"FixedUpdating {this}");
    }

    public virtual void ExitState()
    {
        // Debug.Log($"Exiting {this}");
    }
}
