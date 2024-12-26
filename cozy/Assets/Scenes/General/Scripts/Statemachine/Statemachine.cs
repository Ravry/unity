using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statemachine<EStates> : MonoBehaviour where EStates : Enum
{
    protected Dictionary<EStates, IState<EStates>> states;
    protected IState<EStates> currentState;

    public abstract void SetupStates();
    public abstract void ReloadStates();
    public abstract void UpdateVars();


    void Awake() {
        SetupStates();
    }

    void OnEnable()
    {
        ReloadStates();
    }

    void Start()
    {
        currentState.EnterState();
    }

    void Update()
    {
        UpdateVars();
        EStates estate = currentState.CheckState();
        IState<EStates> istate = states[estate]; 
        if (istate != currentState)
            SwitchState(istate);
        currentState.Update();
    }

    void SwitchState(IState<EStates> state) {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState();
    }

    void FixedUpdate() 
    {
        currentState.FixedUpdate();
    }
}