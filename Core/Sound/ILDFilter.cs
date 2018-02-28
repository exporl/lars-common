using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound
{

    /// <summary>
    /// Interaural Level Difference filter
    /// Uses a directionalFactor (-1 = left, 0 = center, 1 = right) for deciding the amount of ILD
    /// Uses audioDirection to calculate ascending/descending ILD (or use Stationary for static ILD)
    /// </summary>
    public class ILDFilter : MovingFilter
    {
        //  The difference in dB at outer ends (left or right)
        public float MAX_ILD = 20;
        public bool directionsEnabled;

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!ready) return;

            int direction = 0;
            if(directionsEnabled) {
                if(audioDirection == Direction.Left)
                    direction = -1;
                else if(audioDirection == Direction.Right)
                    direction = 1;
            }

            //calculate boundaries
            float startValue = bias * MAX_ILD;
            float deltaFactor = deltaBias * direction;
            float endValue = startValue + deltaFactor * MAX_ILD;

            //apply filter
            ApplyToBlock(data, startValue, endValue);
        }

        public static void ApplyToBlock(float[] data, float ILD_start, float ILD_end)
        {
            float stepSize = (ILD_end - ILD_start) / BLOCK_SIZE;

            for (int i = 0; i < BLOCK_SIZE; i++)
            {
                //calculated interpolated gain for left and right channel, for this sample
                int leftI = 2*i;
                int rightI = 2*i + 1;
                float rightGain = (ILD_start + stepSize * i)/2f;
                
                //apply the gain values
                data[leftI] = data[leftI] * Sound.Utils.DecibelToLinear(-rightGain);
                data[rightI] = data[rightI] * Sound.Utils.DecibelToLinear(rightGain);
            }
        }
    }
}