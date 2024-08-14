using System;
using System.Linq;

namespace Arcadian.StateManagement
{
    public class StateMachine<T> where T : State
    {
        public T CurrentState { get; private set; }
        
        private T[] States { get; }
        public event Action onStateChange;

        public StateMachine(T[] states)
        {
            States = states;
        }

        public void SetState(Type type)
        {
            // No changes in state
            if (CurrentState != null && type == CurrentState.GetType()) return;

            CurrentState?.EndState();
            
            CurrentState = States.FirstOrDefault(state => state.GetType() == type);
            
            CurrentState?.StartState();
            onStateChange?.Invoke();
        }
    }
}