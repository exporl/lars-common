using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdContinuousExponential : ThresholdAdaptiveProcedure 
{
    readonly float stepUp, stepDown, maximum;

    //dynamic variables
    int consecutiveSuccesses;
    float currentThreshold;

    /// <param name="stepUp">value smaller than one, yielding the strongest possible difficulty increase</param>
    /// <param name="stepDown">value bigger than one, yielding the strongest possible difficulty decrease</param>
    /// <param name="startingThreshold">initial value for the threshold</param>
    /// <param name="maximum">maximum value for the threshold</param>
    public ThresholdContinuousExponential(float stepUp, float stepDown, float startingThreshold, float maximum) {
        this.stepUp = stepUp;
        this.stepDown = stepDown;
        this.currentThreshold = startingThreshold;
        this.maximum = maximum;
    }

    public override float GetThreshold() {
        return currentThreshold;
    }
    
    public override void UpdateThreshold(float value) {
        float factor = stepUp + (1f - stepUp) * value / currentThreshold;
        factor = Mathf.Clamp(factor, stepUp, stepDown);
        currentThreshold = factor * currentThreshold;
        currentThreshold = Mathf.Clamp(currentThreshold, 0f, maximum);
    }

 
}
