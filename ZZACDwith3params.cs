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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
    /// </summary>
    [Description("ACD buy sell signal with 3 params, tickbuffer, OR time, and A time")]
    public class ZZACDwith3params : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int oRNumBars = 4; // Default setting for ORNumBars
            private int numBarAsignal = 2; // Default setting for NumBarAsignal
            private int tickBuffer = 2; // Default setting for TickBuffer
			private double ORHighestPrice = 0;
			private double ORLowestPrice = 0;
			private double ORRangePrice = 0;
			private bool ORCompleted = false;
			private bool ApivotFound = false;
			private bool CpivotFound = false;
			private bool PriceBreaksORHigh = false;
			private bool PriceBreaksORLow = false;
			private int currBar = 0;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Navy), PlotStyle.Line, "Signal"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
// if barsinceentry is bigger than oRNumBars, then we got the opening range done
			//      get highestbar since open
			//      get lowestbar since open
			//      if barsinceentry >= ornumbars + numbarasignal 
			//          if high is above ORhighestbar
			//             signal.set 1
			//          else if low is below orlowestbar
			//             signal.set -1
			//          else
			//             signal.set 0
			//  else
			//     signal.set 0
			// Reset every new session (market open)
			if( Bars.BarsSinceSession == 0 ){
				ORCompleted = false;
				ApivotFound = false;
				CpivotFound = false;
				PriceBreaksORHigh =false;
			}
			Signal.Set(0);
			
		   	if ( (Bars.BarsSinceSession  >= oRNumBars-1) && !ORCompleted ) {   // barssincesession is # bars before current bar
				//Print( "Hour = " + Time[0].Hour + "Min = " + Time[0].Minute + "Bars = " + Bars.BarsSinceSession );
				// opening range completes
				ORHighestPrice = High[HighestBar(High, Bars.BarsSinceSession+1 )];
				ORLowestPrice = Low[LowestBar(Low, Bars.BarsSinceSession+1 )];
				ORRangePrice = ORHighestPrice - ORLowestPrice;
				ORCompleted = true;
				
				//Print( "ORHighestPrice = " + ORHighestPrice + "ORLowestPrice = " + ORLowestPrice);
			}
			// if high crossabove ORhigh then the next numBarAsignal bars, the high must stay above orhigh
			if( ((High[0] > ORHighestPrice) || (Low[0] < ORLowestPrice)) && !PriceBreaksORHigh && ORCompleted ) {
				PriceBreaksORHigh = true;
				currBar = Bars.BarsSinceSession;
			}
			
			if( PriceBreaksORHigh ) {
				if( Bars.BarsSinceSession >= currBar + numBarAsignal -1 ) {
					if( (High[0] > ORHighestPrice + (tickBuffer * TickSize)) && !ApivotFound ) {
						//Print( " High[0] > OR  ");
						Signal.Set(10);
						ApivotFound = true;
					}
					else if( (Low[0] < ORLowestPrice - (tickBuffer * TickSize)) && !ApivotFound ) {
						//Print( " Low[0] > OR  ");
						Signal.Set(-10);
						ApivotFound = true;
					}
					else
						Signal.Set(0);
				
				}
			}			
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Signal
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ORNumBars
        {
            get { return oRNumBars; }
            set { oRNumBars = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int NumBarAsignal
        {
            get { return numBarAsignal; }
            set { numBarAsignal = Math.Max(1, value); }
        }

        [Description("Tick buffer to filter A pivot")]
        [GridCategory("Parameters")]
        public int TickBuffer
        {
            get { return tickBuffer; }
            set { tickBuffer = Math.Max(1, value); }
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
        private ZZACDwith3params[] cacheZZACDwith3params = null;

        private static ZZACDwith3params checkZZACDwith3params = new ZZACDwith3params();

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public ZZACDwith3params ZZACDwith3params(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return ZZACDwith3params(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public ZZACDwith3params ZZACDwith3params(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            if (cacheZZACDwith3params != null)
                for (int idx = 0; idx < cacheZZACDwith3params.Length; idx++)
                    if (cacheZZACDwith3params[idx].NumBarAsignal == numBarAsignal && cacheZZACDwith3params[idx].ORNumBars == oRNumBars && cacheZZACDwith3params[idx].TickBuffer == tickBuffer && cacheZZACDwith3params[idx].EqualsInput(input))
                        return cacheZZACDwith3params[idx];

            lock (checkZZACDwith3params)
            {
                checkZZACDwith3params.NumBarAsignal = numBarAsignal;
                numBarAsignal = checkZZACDwith3params.NumBarAsignal;
                checkZZACDwith3params.ORNumBars = oRNumBars;
                oRNumBars = checkZZACDwith3params.ORNumBars;
                checkZZACDwith3params.TickBuffer = tickBuffer;
                tickBuffer = checkZZACDwith3params.TickBuffer;

                if (cacheZZACDwith3params != null)
                    for (int idx = 0; idx < cacheZZACDwith3params.Length; idx++)
                        if (cacheZZACDwith3params[idx].NumBarAsignal == numBarAsignal && cacheZZACDwith3params[idx].ORNumBars == oRNumBars && cacheZZACDwith3params[idx].TickBuffer == tickBuffer && cacheZZACDwith3params[idx].EqualsInput(input))
                            return cacheZZACDwith3params[idx];

                ZZACDwith3params indicator = new ZZACDwith3params();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NumBarAsignal = numBarAsignal;
                indicator.ORNumBars = oRNumBars;
                indicator.TickBuffer = tickBuffer;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZACDwith3params[] tmp = new ZZACDwith3params[cacheZZACDwith3params == null ? 1 : cacheZZACDwith3params.Length + 1];
                if (cacheZZACDwith3params != null)
                    cacheZZACDwith3params.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZACDwith3params = tmp;
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
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZACDwith3params ZZACDwith3params(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3params(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACDwith3params ZZACDwith3params(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3params(input, numBarAsignal, oRNumBars, tickBuffer);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZACDwith3params ZZACDwith3params(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3params(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACDwith3params ZZACDwith3params(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZACDwith3params(input, numBarAsignal, oRNumBars, tickBuffer);
        }
    }
}
#endregion
