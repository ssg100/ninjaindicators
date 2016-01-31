#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace JHL.Utility {
	// Calculates the fractal dimension
	//   Used by HurstExponent
	public class FractalDimension {
		private int period;
		private int bars = 0;
		private int lastBar = -1;
		private MIN min;
		private MAX max;
		private double[] value;
	    private double c1;
	    private double c2;
		
		public FractalDimension(int period)
		{
			this.period = Math.Max(1, period);
			min = new MIN(this.period);
			max = new MAX(this.period);
			value = min.value;
		}
		
		public double set(double curVal, int barNum)
		{
			double min, delta = max.set(curVal, barNum) - (min = this.min.set(curVal, barNum));
			
			if ( barNum != lastBar ) {
				lastBar = barNum;
				if ( bars < period ) {
					if ( ++bars > 1 ) {
						c1 = Math.Pow(1 / (bars - 1), 2);
						c2 = Math.Log(2 * (bars - 1));
					}
				}
			}
			
			if ( bars < 2 || (delta <= double.Epsilon && -delta <= double.Epsilon) )
				return 1;
			
			double length = 0, yPrev = (curVal - min) / delta;
			for ( int i = 1; i < bars; ++i ) {
				double y = (value[(barNum - i) % period] - min) / delta;
				length += Math.Sqrt(Math.Pow(y - yPrev, 2) + c1);
				yPrev = y;
			}
			
			return 1.0 + (Math.Log(length) + constants.log_2) / c2;
		}
	}
}

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Calculates the fractal dimension of an n period time series
    /// </summary>
    [Description("Calculates the fractal dimension of an n period time series")]
    public class jhlFractalDimension : Indicator
    {
        #region Variables
            private int period = 14;
		    private JHL.Utility.FractalDimension fd;
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkBlue), PlotStyle.Dot, "D"));
			Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 1.5, "Random"));
            Overlay	= false;
			PriceTypeSupported = true;
		 //	CalculateOnBarClose = true;		// This indicator supports COBC == false, but cannot be calculated without
        }									// iterating over past bars, so is not efficient to update per tick.
		
		protected override void OnStartUp()
		{
			fd = new JHL.Utility.FractalDimension(period);
		}

        protected override void OnBarUpdate()
        {
            D.Set(fd.set(Input[0], CurrentBar));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries D
        {
            get { return Values[0]; }
        }

        [Description("Number of samples in calculation")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private jhlFractalDimension[] cachejhlFractalDimension = null;

        private static jhlFractalDimension checkjhlFractalDimension = new jhlFractalDimension();

        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        public jhlFractalDimension jhlFractalDimension(int period)
        {
            return jhlFractalDimension(Input, period);
        }

        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        public jhlFractalDimension jhlFractalDimension(Data.IDataSeries input, int period)
        {
            if (cachejhlFractalDimension != null)
                for (int idx = 0; idx < cachejhlFractalDimension.Length; idx++)
                    if (cachejhlFractalDimension[idx].Period == period && cachejhlFractalDimension[idx].EqualsInput(input))
                        return cachejhlFractalDimension[idx];

            lock (checkjhlFractalDimension)
            {
                checkjhlFractalDimension.Period = period;
                period = checkjhlFractalDimension.Period;

                if (cachejhlFractalDimension != null)
                    for (int idx = 0; idx < cachejhlFractalDimension.Length; idx++)
                        if (cachejhlFractalDimension[idx].Period == period && cachejhlFractalDimension[idx].EqualsInput(input))
                            return cachejhlFractalDimension[idx];

                jhlFractalDimension indicator = new jhlFractalDimension();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                jhlFractalDimension[] tmp = new jhlFractalDimension[cachejhlFractalDimension == null ? 1 : cachejhlFractalDimension.Length + 1];
                if (cachejhlFractalDimension != null)
                    cachejhlFractalDimension.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachejhlFractalDimension = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlFractalDimension jhlFractalDimension(int period)
        {
            return _indicator.jhlFractalDimension(Input, period);
        }

        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlFractalDimension jhlFractalDimension(Data.IDataSeries input, int period)
        {
            return _indicator.jhlFractalDimension(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlFractalDimension jhlFractalDimension(int period)
        {
            return _indicator.jhlFractalDimension(Input, period);
        }

        /// <summary>
        /// Calculates the fractal dimension of an n period time series
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlFractalDimension jhlFractalDimension(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.jhlFractalDimension(input, period);
        }
    }
}
#endregion
