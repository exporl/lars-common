using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AccuracyAdaptiveProcedure{

    /// <summary>
    /// Returns accuracy value for the next challenge
    /// </summary>
    public abstract float GetAccuracy();

    /// <summary>
    /// Updates accuracy value based whether the attempt was successful or not.
    /// </summary>
    public abstract void Attempt(bool success);
}
