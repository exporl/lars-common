using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound {

    /// <summary>
    /// Interaural Time Difference filter
    /// Uses a directionalFactor (-1 = left, 0 = center, 1 = right) for deciding the amount of ITD
    /// Uses audioDirection to calculate ascending/descending ITD (or use Stationary for static ITD)
    /// </summary>
    public class ITDFilter : MovingFilter {

        //  The difference in time (in microseconds) at outer ends (left or right)
        public float MAX_ITD = 500;
        public bool directionsEnabled;

        private float bufferedITD;
        private float[] bufferedData;

        private int bufferSamples
        {
            get
            {
                float sampleOffset = ITDToSamples(Mathf.Abs(MAX_ITD) / 2f);
                return Mathf.CeilToInt(sampleOffset) + 1;
            }
        }

        private void InitializeBuffer() 
        {
            float sampleOffset = ITDToSamples(Mathf.Abs(MAX_ITD) / 2f);
            int largestPossibleOffset = Mathf.CeilToInt(sampleOffset) + bufferSamples;
            if(largestPossibleOffset > BLOCK_SIZE)
                Debug.LogError("MAX_ITD value is too large, cannot work within single block buffer!");

            //initialize block buffer
            bufferedData = new float[largestPossibleOffset*2];
        }

        void OnAudioFilterRead(float[] data, int channels) 
        {
            if(!ready)
                return;

            if(bufferedITD != MAX_ITD) {
                bufferedITD = MAX_ITD;
                InitializeBuffer();
            }

            int direction = 0;
            if(directionsEnabled) {
                if(audioDirection == Direction.Left)
                    direction = -1;
                else if(audioDirection == Direction.Right)
                    direction = 1;
            }

            //calculate boundaries
            float startValue = bias * MAX_ITD;
            float deltaFactor = deltaBias * direction;
            float endValue = startValue + deltaFactor * MAX_ITD;

            //save new values to store in the buffer after we're done.
            float[] newBuffer = new float[bufferedData.Length];
            for(int i = 0; i < bufferedData.Length; i++)
                newBuffer[i] = data[data.Length - bufferedData.Length + i];

            //apply filter
            ApplyToBlock(data, bufferedData, startValue, endValue, bufferSamples);

            //write new values to buffer
            for(int i = 0; i < bufferedData.Length; i++)
                bufferedData[i] = newBuffer[i];
        }

        public static void ApplyToBlock(float[] data, float[] bufferedData, float ITD_start, float ITD_end, int buffer) 
        {
            float stepSize = (ITD_end - ITD_start) / BLOCK_SIZE;

            for(int i = BLOCK_SIZE - 1; i >= 0; i--) {
                //calculate interpolated time shift values for left and right channel, for this sample
                float itd = ITD_start + (stepSize * i);
                float leftITD = -itd/2f;
                float rightITD = itd/2f;

                int baseIndex = i - buffer;

                int leftRelShift = Mathf.RoundToInt(ITDToSamples(leftITD));
                int rightRelShift = Mathf.RoundToInt(ITDToSamples(rightITD));
                
                data[2 * i] = GetSampleAtIndex(baseIndex + leftRelShift, false, data, bufferedData);
                data[(2 * i) + 1] = GetSampleAtIndex(baseIndex + rightRelShift, true, data, bufferedData);
            }
        }

        /// <summary>
        /// Returns sound value at the given sample index and channel, from the given data blocks.
        /// Negative indices are drawn from the previous data block as needed.
        /// </summary>
        private static float GetSampleAtIndex(int index, bool right, float[] data, float[] bufferedData) {
            bool previousBlock = index < 0;
            if(previousBlock)
                index += (bufferedData.Length / 2);
            int sample = (2 * index) + (right ? 1 : 0);
            return previousBlock ? bufferedData[sample] : data[sample];
        }

        /// <summary>
        /// Convert ITD value in micro seconds to 
        /// a fractional amount of samples.
        /// </summary>
        private static float ITDToSamples(float ITD)
        {
            return ITD * 1e-6f / (float)SECONDS_PER_SAMPLE;
        }
    }
}