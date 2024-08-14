namespace Arcadian.Wrappers
{
    /// <summary>
    /// Wrapper for a boolean. Seems redundant but is used to pass
    /// booleans by reference, without using the <c>ref</c> keyword.
    /// </summary>
    public class WBool
    {
        /// <summary>
        /// Stored <c>float</c> value.
        /// </summary>
        public bool Value { get; set; }

        public WBool(bool value)
        {
            Value = value;
        }
    }
}