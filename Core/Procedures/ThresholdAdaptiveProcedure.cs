public abstract class ThresholdAdaptiveProcedure{

    /// <summary>
    /// Returns current threshold value
    /// </summary>
    public abstract float GetThreshold();
    
    /// <summary>
    /// Updates threshold value based on the given player attempt.
    /// If this value is smaller than the threshold, the attempt was successful.
    /// </summary>
    public abstract void UpdateThreshold(float value);
}
