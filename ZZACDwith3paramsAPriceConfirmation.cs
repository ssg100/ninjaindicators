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
    public class ZZACDwith3paramsAPriceConfirmation : Indicator
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
			private int PriceStaysAbove = 0;	// check if price stays above A pivot for long or below for short
			private int PriceStaysBelow = 0;
        	private bool IsAupPotential = false;	// store direction 
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
				PriceStaysAbove = 0;
				PriceStaysBelow = 0;
				IsAupPotential = false;
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
			if( ((High[0] > ORHighestPrice + (tickBuffer * TickSize)) || (Low[0] < ORLowestPrice  - (tickBuffer * TickSize) )) && !PriceBreaksORHigh && ORCompleted ) {
				PriceBreaksORHigh = true;
				if( High[0] > ORHighestPrice + (tickBuffer * TickSize) ){
					//Signal.Set(10);
					IsAupPotential = true;
				}
				else {
					//Signal.Set(-10);
					IsAupPotential = false;
				}
				currBar = Bars.BarsSinceSession;
			}
			
			if( PriceBreaksORHigh & !ApivotFound ) {
				
				if( Bars.BarsSinceSession < currBar + numBarAsignal ) {
					if( (High[0] >= ORHighestPrice + (tickBuffer * TickSize)) ) {
						PriceStaysAbove = PriceStaysAbove + 1;				
					}
					
					if( (Low[0] <= ORLowestPrice - (tickBuffer * TickSize)) ) {
						PriceStaysBelow = PriceStaysBelow + 1;

					}
				}
				
				if( Bars.BarsSinceSession >= currBar + numBarAsignal  ) {
					if( PriceStaysAbove >= numBarAsignal && IsAupPotential )
						Signal.Set(10);
				
					else if( PriceStaysBelow >= numBarAsignal && !IsAupPotential)
						Signal.Set(-10);
					else
						Signal.Set(0);
					
					ApivotFound = true;
				}
				
				
				//Signal.Set(10);
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
        private ZZACDwith3paramsAPriceConfirmation[] cacheZZACDwith3paramsAPriceConfirmation = null;

        private static ZZACDwith3paramsAPriceConfirmation checkZZACDwith3paramsAPriceConfirmation = new ZZACDwith3paramsAPriceConfirmation();

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return ZZACDwith3paramsAPriceConfirmation(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            if (cacheZZACDwith3paramsAPriceConfirmation != null)
                for (int idx = 0; idx < cacheZZACDwith3paramsAPriceConfirmation.Length; idx++)
                    if (cacheZZACDwith3paramsAPriceConfirmation[idx].NumBarAsignal == numBarAsignal && cacheZZACDwith3paramsAPriceConfirmation[idx].ORNumBars == oRNumBars && cacheZZACDwith3paramsAPriceConfirmation[idx].TickBuffer == tickBuffer && cacheZZACDwith3paramsAPriceConfirmation[idx].EqualsInput(input))
                        return cacheZZACDwith3paramsAPriceConfirmation[idx];

            lock (checkZZACDwith3paramsAPriceConfirmation)
            {
                checkZZACDwith3paramsAPriceConfirmation.NumBarAsignal = numBarAsignal;
                numBarAsignal = checkZZACDwith3paramsAPriceConfirmation.NumBarAsignal;
                checkZZACDwith3paramsAPriceConfirmation.ORNumBars = oRNumBars;
                oRNumBars = checkZZACDwith3paramsAPriceConfirmation.ORNumBars;
                checkZZACDwith3paramsAPriceConfirmation.TickBuffer = tickBuffer;
                tickBuffer = checkZZACDwith3paramsAPriceConfirmation.TickBuffer;

                if (cacheZZACDwith3paramsAPriceConfirmation != null)
                    for (int idx = 0; idx < cacheZZACDwith3paramsAPriceConfirmation.Length; idx++)
                        if (cacheZZACDwith3paramsAPriceConfirmation[idx].NumBarAsignal == numBarAsignal && cacheZZACDwith3paramsAPriceConfirmation[idx].ORNumBars == oRNumBars && cacheZZACDwith3paramsAPriceConfirmation[idx].TickBuffer == tickBuffer && cacheZZACDwith3paramsAPriceConfirmation[idx].EqualsInput(input))
                            return cacheZZACDwith3paramsAPriceConfirmation[idx];

                ZZACDwith3paramsAPriceConfirmation indicator = new ZZACDwith3paramsAPriceConfirmation();
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

                ZZACDwith3paramsAPriceConfirmation[] tmp = new ZZACDwith3paramsAPriceConfirmation[cacheZZACDwith3paramsAPriceConfirmation == null ? 1 : cacheZZACDwith3paramsAPriceConfirmation.Length + 1];
                if (cacheZZACDwith3paramsAPriceConfirmation != null)
                    cacheZZACDwith3paramsAPriceConfirmation.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZACDwith3paramsAPriceConfirmation = tmp;
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
        public Indicator.ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3paramsAPriceConfirmation(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3paramsAPriceConfirmation(input, numBarAsignal, oRNumBars, tickBuffer);
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
        public Indicator.ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            return _indicator.ZZACDwith3paramsAPriceConfirmation(Input, numBarAsignal, oRNumBars, tickBuffer);
        }

        /// <summary>
        /// ACD buy sell signal with 3 params, tickbuffer, OR time, and A time
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACDwith3paramsAPriceConfirmation ZZACDwith3paramsAPriceConfirmation(Data.IDataSeries input, int numBarAsignal, int oRNumBars, int tickBuffer)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZACDwith3paramsAPriceConfirmation(input, numBarAsignal, oRNumBars, tickBuffer);
        }
    }
}
#endregion
