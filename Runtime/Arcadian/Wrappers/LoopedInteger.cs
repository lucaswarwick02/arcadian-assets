namespace Arcadian.Wrappers
{
    /// <summary>
    /// Wrapper for an Integer which only allows for increasing and decreasing by 1.
    /// If it either exceeds 0 or MaxValue it will loop to the other.
    /// </summary>
    public class LoopedInteger
    {
        /// <summary>
        /// Currently stored value.
        /// </summary>
        private int Value { set; get; }
        
        private int MaxValue { get; }

        /// <summary>
        /// Sets up the Looped Integer.
        /// </summary>
        /// <param name="maxValue">Max value before looping to 0 (inclusive)</param>
        /// <param name="value">Initial value. Defaults to 0.</param>
        public LoopedInteger(int maxValue, int value = 0)
        {
            MaxValue = maxValue;
            Value = value;
        }

        public void Increment()
        {
            Value++;

            if (Value > MaxValue) Value = 0;
        }

        public void Decrement()
        {
            Value--;

            if (Value < 0) Value = MaxValue;
        }

        public static implicit operator int(LoopedInteger loopedInteger)
        {
            return loopedInteger.Value;
        }
    }
}