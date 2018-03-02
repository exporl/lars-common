using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Lars
{

    public abstract class PlotResults : MonoBehaviour
    {
        public SimplestPlot.PlotType Type = SimplestPlot.PlotType.TimeSeries;
        //public int DataPoints = 17;

        protected SimplestPlot SimplestPlotScript;

        [SerializeField]
        int trialsForSrt = 12;

        [SerializeField]
        private Color[] SeriesColors = new Color[2];

        [SerializeField]
        private Vector2 Resolution;

        [SerializeField]
        protected Text leftSRT, rightSRT, leftStD, rightStD;

        void Awake()
        {
            
        }

        [EditorButton]
        public virtual void PreparePlot()//List<Answer> answers)
        {
            SimplestPlotScript = GetComponent<SimplestPlot>();
            Resolution = new Vector2(1280, 800);

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
        public abstract void DrawPlot();

        protected double CalculateAverage(List<double> values)
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

        protected double CalculateStdDev(List<double> values)
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

    }

}