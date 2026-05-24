namespace LucasWarwick02.UnityAssets
{
    /// <summary>
    /// The platform/device category detected at startup. Does not change at runtime.
    /// </summary>
    public enum PlatformType
    {
        Desktop,
        HandheldPC,  // Steam Deck, ROG Ally, Legion Go, etc.
        Console,
        Mobile
    }
}