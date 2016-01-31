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
using JHL.Utility;
#endregion

// Written by Jonathan H Lundquist, http://fluxsmith.blogspot.com

namespace JHL.Utility {
	// Returns minimum value within prior n periods.
	//   Used by FractalDimension.
	// This only has to iterate when the low bar is rolled off
	// the front.  Intrabar updates are fast and never iterate.
	public class MIN {
		public double[] value { get; private set; }
		private int period;
		private int lowBar;
		private int lastBar = -1;
		private double low = double.MaxValue;
		
		public MIN(int period)
		{
			this.period = Math.Max(period, 1);
			value = new double[this.period];
		}
		
		public double set(double curVal, int barNum)
		{
			if ( barNum == lastBar )
				// Intrabar (tick) update
				return Math.Min(value[barNum % period] = curVal, low);
			
			if ( lastBar < 0 ) {
				// First call, initialization
				lastBar = barNum;
				return value[barNum % period] = curVal;
			}
			
			// New bar (either first tick of new bar, or COBC = false)
			if ( value[lastBar % period] <= low ) {
				low = value[lastBar % period];
				lowBar = lastBar;
			}

			if ( barNum - lowBar >= period ) {
				low = double.MaxValue;
				int i = barNum - (lastBar - lowBar);
				for ( int index = i % period; i < barNum; index = ++i % period ) {
					if ( value[index] <= low ) {
						low = value[index];
						lowBar = i;
					}
				}
			}
			
			lastBar = barNum;
			return Math.Min(value[barNum % period] = curVal, low);
		}
	}
}

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Returns the low value of the past n periods.
    /// </summary>
    [Description("Returns the low value of the past n periods.")]
    public class jhlMIN : Indicator
    {
        #region Variables
           private int period = 14;
		   private JHL.Utility.MIN min;
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "Min"));
            Overlay	= true;
			PriceTypeSupported = true;
		 //	CalculateOnBarClose = false;			// This only has to iterate when a low bar rolls off the front.
        }											// Intrabar are fast and never iterate.
		
		protected override void OnStartUp()
		{
			min = new JHL.Utility.MIN(period);
		}

        protected override void OnBarUpdate()
        {
			Min.Set(min.set(Input[0], CurrentBar));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Min
        {
            get { return Values[0]; }
        }

        [Description("Number of bars in calculation.")]
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
        private jhlMIN[] cachejhlMIN = null;

        private static jhlMIN checkjhlMIN = new jhlMIN();

        /// <summary>
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public jhlMIN jhlMIN(int period)
        {
            return jhlMIN(Input, period);
        }

        /// <summary>
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public jhlMIN jhlMIN(Data.IDataSeries input, int period)
        {
            if (cachejhlMIN != null)
                for (int idx = 0; idx < cachejhlMIN.Length; idx++)
                    if (cachejhlMIN[idx].Period == period && cachejhlMIN[idx].EqualsInput(input))
                        return cachejhlMIN[idx];

            lock (checkjhlMIN)
            {
                checkjhlMIN.Period = period;
                period = checkjhlMIN.Period;

                if (cachejhlMIN != null)
                    for (int idx = 0; idx < cachejhlMIN.Length; idx++)
                        if (cachejhlMIN[idx].Period == period && cachejhlMIN[idx].EqualsInput(input))
                            return cachejhlMIN[idx];

                jhlMIN indicator = new jhlMIN();
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

                jhlMIN[] tmp = new jhlMIN[cachejhlMIN == null ? 1 : cachejhlMIN.Length + 1];
                if (cachejhlMIN != null)
                    cachejhlMIN.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachejhlMIN = tmp;
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
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlMIN jhlMIN(int period)
        {
            return _indicator.jhlMIN(Input, period);
        }

        /// <summary>
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlMIN jhlMIN(Data.IDataSeries input, int period)
        {
            return _indicator.jhlMIN(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlMIN jhlMIN(int period)
        {
            return _indicator.jhlMIN(Input, period);
        }

        /// <summary>
        /// Returns the low value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlMIN jhlMIN(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.jhlMIN(input, period);
        }
    }
}
#endregion
