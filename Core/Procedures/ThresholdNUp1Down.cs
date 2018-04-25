using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdNUp1Down : ThresholdAdaptiveProcedure 
{
    readonly int successesUntilStepUp;
    readonly float stepUp, stepDown, maximum;

    //dynamic variables
    int consecutiveSuccesses;
    float currentThreshold;
    int direction;
    int nbReversals;

    /// <param name="n">number of successes needed to step up</param>
    /// <param name="stepUp">value smaller than one, to increase difficulty</param>
    /// <param name="stepDown">value bigger than one, to lower difficulty</param>
    /// <param name="startingThreshold">initial value for the threshold</param>
    /// <param name="maximum">maximum value for the threshold</param>
    public ThresholdNUp1Down(int n, float stepUp, float stepDown, float startingThreshold, float maximum) {
        this.successesUntilStepUp = n;
        this.stepUp = stepUp;
        this.stepDown = stepDown;
        this.currentThreshold = startingThreshold;
        this.maximum = maximum;

        this.nbReversals = -1;
    }

    public override float GetThreshold() {
        return currentThreshold;
    }
    
    public override void UpdateThreshold(float value) {
        bool success = value < currentThreshold;
        if(success) {
            consecutiveSuccesses++;
            if(consecutiveSuccesses >= successesUntilStepUp) {
                consecutiveSuccesses = 0;
                currentThreshold *= stepUp;
                SetIncreasing(false);
            }
        }
        else {
            consecutiveSuccesses = 0;
            currentThreshold *= stepDown;
            SetIncreasing(true);
        }
        currentThreshold = Mathf.Clamp(currentThreshold, 0f, maximum);
    }

    private void SetIncreasing(bool value) {
        if(value && direction < 1) { 
            direction = 1;
            nbReversals++;
        }
        if(!value && direction > -1) {
            direction = -1;
            nbReversals++;
        }
    }

    public int GetNbReversals() {
        return nbReversals;
    }

    /// <summary>
    /// Manually change threshold value and reset procedure parameters
    /// </summary>
    public void SetThreshold(float value) {
        currentThreshold = value;
        nbReversals = -1;
        consecutiveSuccesses = 0;
        direction = 0;
    }
 
}
