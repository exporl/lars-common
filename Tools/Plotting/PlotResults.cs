using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Lars.Pirates
{

    public class PlotResults : MonoBehaviour
    {
        public SimplestPlot.PlotType Type = SimplestPlot.PlotType.TimeSeries;
        //public int DataPoints = 17;

        private SimplestPlot SimplestPlotScript;

        [SerializeField]
        int trialsForSrt = 12;

        [SerializeField]
        private Color[] SeriesColors = new Color[2];

        [SerializeField]
        private Vector2 Resolution;

        [SerializeField]
        private Text leftSRT, rightSRT, leftStD, rightStD;

        void Awake()
        {
            
        }

        [EditorButton]
        public void PreparePlot()//List<Answer> answers)
        {
            SimplestPlotScript = GetComponent<SimplestPlot>();
            Resolution = new Vector2(1280, 800);

            //XValues = new float[DataPoints];
            //Y1Values = new float[DataPoints];
            //Y2Values = new float[DataPoints];

            SimplestPlotScript.SetResolution(Resolution);
            SimplestPlotScript.BackGroundColor = new Color(1,1,1,1);
            SimplestPlotScript.TextColor = Color.black;

            for (int i=0; i < 2; i++)
            {
                SimplestPlotScript.SeriesPlotY.Add(new SimplestPlot.SeriesClass());
                SimplestPlotScript.SeriesPlotY[i].MyColor = SeriesColors[i];
            }
        }

        [EditorButton]
        public void DrawPlot(PirateResults results)
        {
            //EXTRACT VALS FROM ANSWERS
            //Y1Values = new float[] { -2, -4, -6, -8, -10, -12, -10, -10.825197896f, -10.317062458793869f, -9.808927021524136f, -10.634124917587737f, -11.459322813651339f, -10.951187376381606f, -11.776385272445207f, -9.93491650184214f, -10.760114397905742f, -11.585312293969343f, -11.07717685669961f };
            //Y2Values = new float[] { -2, -4, -6, -8, -10, -12, -14, -12.158f, -9.8089f, -10.63412f, -10.1259f, -9.61785f, -10.44305f, -8.60158f, -9.42678f, -7.58531f, -8.415101f, -9.42678f };

            //SimplestPlotScript.SeriesPlotY[0].YValues = Y1Values;
            //SimplestPlotScript.SeriesPlotY[1].YValues = Y2Values;
            //float[] leftVals = results.leftAnswers.Select(x => (float)x.snr).ToArray();
            //float[] rightVals = results.rightAnswers.Select(x => (float)x.snr).ToArray();

            List<double> leftValsD = results.leftAnswers.Select(x => x.snr).ToList<double>();
            List<double> rightValsD = results.rightAnswers.Select(x => x.snr).ToList<double>();

            SimplestPlotScript.SeriesPlotY[0].YValues = leftValsD.ConvertAll(x => (float)x).ToArray(); 
            SimplestPlotScript.SeriesPlotY[1].YValues = rightValsD.ConvertAll(x => (float)x).ToArray();

            SimplestPlotScript.UpdatePlot();

            // set text
            leftSRT.text = "SRT Left\n" + CalculateAverage(leftValsD).ToString("0.0");
            rightSRT.text = "SRT Right\n" + CalculateAverage(rightValsD).ToString("0.0");

            leftStD.text = "st.dev\n" + CalculateStdDev(leftValsD).ToString("0.0");
            rightStD.text = "st.dev\n" + CalculateStdDev(rightValsD).ToString("0.0");
        }

        /*
        public float CalculateAverage(float[] values, bool getStdev = false)
        {
            if (values.Length < 1)
                return 0;

            List<float> vals = new List<float>(values);

            // Average
            vals = vals.GetRange(vals.Count - trialsForSrt, trialsForSrt);

            float avg = vals.Sum() / (float)trialsForSrt;
            float stdsum = 0, stdev = 0;

            // Stdev
            if (getStdev)
            {
                for (int i = 0; i < vals.Count; i++)
                {
                    stdsum += System.Math.Pow(vals[i] - avg, 2);
                }
                stdev = Mathf.Sqrt(stdsum / vals.Count);
            }

            if (!getStdev)
                return avg;
            else
                return stdev;
        }
        */
        private double CalculateAverage(List<double> values)
        {
            double avg = 0;
            if (values.Count() > 0)
            {
                values = values.GetRange(values.Count - trialsForSrt, trialsForSrt);
                //Compute the Average      
                avg = values.Average();
            }
            return avg;
        }

        private double CalculateStdDev(List<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                values = values.GetRange(values.Count - trialsForSrt, trialsForSrt);
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => System.Math.Pow(d - avg, 2));
                //Put it all together      
                ret = System.Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        /*
        private double CalculateAverageStDev(List<double> values, bool std = false)
        {
            double ret = 0, avg = 0;
            if (values.Count() > 0)
            {
                values = values.GetRange(values.Count - trialsForSrt, trialsForSrt);   
                avg = values.Average();

                if(std)
                {
                    double sum = values.Sum(d => System.Math.Pow(d - avg, 2));
                    ret = System.Math.Sqrt((sum) / (values.Count() - 1));
                }
            }
            if (!std)
                return avg;
            else
                return ret;
        }*/

        //public float CalculateStDev(float[] vals, float avg)
    }

}