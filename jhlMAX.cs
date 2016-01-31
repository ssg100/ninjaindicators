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
	// Returns maximum value within prior n periods.
	//   Used by FractalDimension.
	// This only has to iterate when the high bar is rolled off
	// this front.  Intrabar updates are fast and never iterate.
	public class MAX {
		private double[] value;
		private int period;
		private int hiBar;
		private int lastBar = -1;
		private double hi = double.MinValue;
		
		public MAX(int period)
		{
			this.period = Math.Max(period, 1);
			value = new double[this.period];
		}
		
		public double set(double curVal, int barNum)
		{
			if ( barNum == lastBar )
				// Intrabar (tick) update
				return Math.Max(value[barNum % period] = curVal, hi);
			
			if ( lastBar < 0 ) {
				// First call, initialization
				lastBar = barNum;
				return value[barNum % period] = curVal;
			}
			
			// New bar (either first tick of new bar, or COBC = false)
			if ( value[lastBar % period] >= hi ) {
				hi = value[lastBar % period];
				hiBar = lastBar;
			}
			
			if ( barNum - hiBar >= period ) {
				hi = double.MinValue;
				int i = barNum - (lastBar - hiBar);
				for ( int index = i % period; i < barNum; index = ++i % period ) {
					if ( value[index] >= hi ) {
						hi = value[index];
						hiBar = i;
					}
				}
			}
			
			lastBar = barNum;
			return Math.Max(value[barNum % period] = curVal, hi);
		}
	}
}

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Returns the high value of the past n periods.
    /// </summary>
    [Description("Returns the high value of the past n periods.")]
    public class jhlMAX : Indicator
    {
        #region Variables
           private int period = 14;
		   private JHL.Utility.MAX max;
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "Max"));
            Overlay	= true;
			PriceTypeSupported = true;
		 //	CalculateOnBarClose = false;			// This only has to iterate when a high bar rolls off the front.
        }											// Intrabar are fast and never iterate.
		
		protected override void OnStartUp()
		{
			max = new JHL.Utility.MAX(period);
		}

        protected override void OnBarUpdate()
        {
			Max.Set(max.set(Input[0], CurrentBar));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Max
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
        private jhlMAX[] cachejhlMAX = null;

        private static jhlMAX checkjhlMAX = new jhlMAX();

        /// <summary>
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public jhlMAX jhlMAX(int period)
        {
            return jhlMAX(Input, period);
        }

        /// <summary>
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public jhlMAX jhlMAX(Data.IDataSeries input, int period)
        {
            if (cachejhlMAX != null)
                for (int idx = 0; idx < cachejhlMAX.Length; idx++)
                    if (cachejhlMAX[idx].Period == period && cachejhlMAX[idx].EqualsInput(input))
                        return cachejhlMAX[idx];

            lock (checkjhlMAX)
            {
                checkjhlMAX.Period = period;
                period = checkjhlMAX.Period;

                if (cachejhlMAX != null)
                    for (int idx = 0; idx < cachejhlMAX.Length; idx++)
                        if (cachejhlMAX[idx].Period == period && cachejhlMAX[idx].EqualsInput(input))
                            return cachejhlMAX[idx];

                jhlMAX indicator = new jhlMAX();
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

                jhlMAX[] tmp = new jhlMAX[cachejhlMAX == null ? 1 : cachejhlMAX.Length + 1];
                if (cachejhlMAX != null)
                    cachejhlMAX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachejhlMAX = tmp;
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
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlMAX jhlMAX(int period)
        {
            return _indicator.jhlMAX(Input, period);
        }

        /// <summary>
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlMAX jhlMAX(Data.IDataSeries input, int period)
        {
            return _indicator.jhlMAX(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jhlMAX jhlMAX(int period)
        {
            return _indicator.jhlMAX(Input, period);
        }

        /// <summary>
        /// Returns the high value of the past n periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.jhlMAX jhlMAX(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.jhlMAX(input, period);
        }
    }
}
#endregion
