namespace Arcadian.StateManagement
{
    public abstract class State
    {
        public virtual void StartState() { }

        public virtual void EndState() { }
    }
}