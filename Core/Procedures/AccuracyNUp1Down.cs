using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyNUp1Down : AccuracyAdaptiveProcedure {

    readonly int successesUntilStepUp;
    readonly float[] stepSizes;
    readonly float minimum, maximum;

    //dynamic variables
    int step;
    int consecutiveSuccesses;
    float currentAccuracy;

    /// <param name="n">number of successes needed to step up</param>
    /// <param name="stepSize">value smaller than one, to increase difficulty</param>
    /// <param name="startingAccuracy">initial value for the threshold</param>
    /// <param name="maximum">maximum value for the accuracy</param>
    public AccuracyNUp1Down(int n, float[] stepSizes, float startingAccuracy, float minimum, float maximum) {
        this.successesUntilStepUp = n;
        this.stepSizes = stepSizes;
        this.currentAccuracy = startingAccuracy;
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public override void Attempt(bool success) {
        if(success) {
            consecutiveSuccesses++;
            if(consecutiveSuccesses >= successesUntilStepUp) {
                currentAccuracy -= GetCurrentStepSize();
                consecutiveSuccesses = 0;
                step++;
            }
        }
        else {
            consecutiveSuccesses = 0;
            currentAccuracy += GetCurrentStepSize();
            step++;
        }

        if(currentAccuracy > maximum)
            currentAccuracy = maximum;
        if(currentAccuracy < minimum)
            currentAccuracy = minimum;
    }

    private float GetCurrentStepSize() {
        int nbStepSizes = stepSizes.Length;
        if(step < nbStepSizes)
            return stepSizes[step];
        return stepSizes[nbStepSizes - 1];
    }

    public override float GetAccuracy() {
        return currentAccuracy;
    }
}
