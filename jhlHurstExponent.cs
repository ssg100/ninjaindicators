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
	public class HurstExponent {
		private JHL.Utility.FractalDimension _fd;
		
		public HurstExponent(int period)
		{
			_fd = new JHL.Utility.FractalDimension(period);
		}
		
		public double set(double curVal, int barNum)
		{
			return -(_fd.set(curVal, barNum) - 2.0);
		}
	}
}

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
    /// </summary>
    [Description("Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.")]
    public class jhlHurstExponent : Indicator
    {
        #region Variables
            private int period = 14;
		    private JHL.Utility.HurstExponent h;
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Dot, "H"));
            Add(new Line(Color.DarkOliveGreen, 0.5, "Random"));
			PriceTypeSupported  = true;
            Overlay				= false;
			CalculateOnBarClose = true;				// This indicator supports COBC == false, but cannot be calculated without
        }											// iterating over past bars, so is not efficient to update per tick.
		
		protected override void OnStartUp()
		{
			h = new JHL.Utility.HurstExponent(period);
		}

        protected override void OnBarUpdate()
        {
            H.Set(h.set(Input[0], CurrentBar));
			//DrawTextFixed("tag1", h.set(Input[0], CurrentBar), TextPosition.BottomRight);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the property from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double value
        {
            get { this.Update(); return Values[0][0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries H
        {
            get { return Values[0]; }
        }

        [Description("Number of bars in calculation")]
        [GridCategory("Parameters")]
        public int Periods
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
        private jhlHurstExponent[] cachejhlHurstExponent = null;

        private static jhlHurstExponent checkjhlHurstExponent = new jhlHurstExponent();

        /// <summary>
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        public jhlHurstExponent jhlHurstExponent(int periods)
        {
            return jhlHurstExponent(Input, periods);
        }

        /// <summary>
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        public jhlHurstExponent jhlHurstExponent(Data.IDataSeries input, int periods)
        {
            if (cachejhlHurstExponent != null)
                for (int idx = 0; idx < cachejhlHurstExponent.Length; idx++)
                    if (cachejhlHurstExponent[idx].Periods == periods && cachejhlHurstExponent[idx].EqualsInput(input))
                        return cachejhlHurstExponent[idx];

            lock (checkjhlHurstExponent)
            {
                checkjhlHurstExponent.Periods = periods;
                periods = checkjhlHurstExponent.Periods;

                if (cachejhlHurstExponent != null)
                    for (int idx = 0; idx < cachejhlHurstExponent.Length; idx++)
                        if (cachejhlHurstExponent[idx].Periods == periods && cachejhlHurstExponent[idx].EqualsInput(input))
                            return cachejhlHurstExponent[idx];

                jhlHurstExponent indicator = new jhlHurstExponent();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Periods = periods;
                Indicators.Add(indicator);
                indicator.SetUp();

                jhlHurstExponent[] tmp = new jhlHurstExponent[cachejhlHurstExponent == null ? 1 : cachejhlHurstExponent.Length + 1];
                if (cachejhlHurstExponent != null)
                    cachejhlHurstExponent.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachejhlHurstExponent = tmp;
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
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlHurstExponent jhlHurstExponent(int periods)
        {
            return _indicator.jhlHurstExponent(Input, periods);
        }

        /// <summary>
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlHurstExponent jhlHurstExponent(Data.IDataSeries input, int periods)
        {
            return _indicator.jhlHurstExponent(input, periods);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlHurstExponent jhlHurstExponent(int periods)
        {
            return _indicator.jhlHurstExponent(Input, periods);
        }

        /// <summary>
        /// Calculates the Hurst Exponent (H) of a time series.  Interpretation is H < 0.5 => mean reverting, H == 0.5 => random, H > 0.5 => trending.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlHurstExponent jhlHurstExponent(Data.IDataSeries input, int periods)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.jhlHurstExponent(input, periods);
        }
    }
}
#endregion
