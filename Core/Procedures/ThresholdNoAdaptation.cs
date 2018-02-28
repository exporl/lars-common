using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdNoAdaptation : ThresholdAdaptiveProcedure 
{

    readonly float threshold;

    /// <param name="threshold">constant value for the threshold</param>
    public ThresholdNoAdaptation(float threshold) {
        this.threshold = threshold;
    }

    public override float GetThreshold() {
        return threshold;
    }
    
    public override void UpdateThreshold(float value) {}

 
}
