using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNadir
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State _currentState;
        public void SetState(State inState) 
        {
            _currentState = inState;
            StartCoroutine(_currentState.Start());
        }
    }
}
