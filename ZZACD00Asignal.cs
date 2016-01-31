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
    /// A signal on ACD return 1, if signal true
    /// </summary>
    [Description("A signal on ACD return 1, if signal true")]
    public class ZZACD00Asignal : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int oRNumBars = 4; // Default setting for ORNumBars
            private int numBarAsignal = 2; // Default setting for NumBarAsignal
			private int ticksBuffer = 0;	// A buffer
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
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), PlotStyle.Line, "Signal"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), PlotStyle.Line, "Signal"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), PlotStyle.Line, "Signal"));
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
					if( (High[0] > ORHighestPrice + (ticksBuffer * TickSize)) && !ApivotFound ) {
						//Print( " High[0] > OR  ");
						Signal.Set(10);
						ApivotFound = true;
					}
					else if( (Low[0] < ORLowestPrice - (ticksBuffer * TickSize)) && !ApivotFound ) {
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

        [Description("Time range for opening range")]
        [GridCategory("Parameters")]
        public int ORNumBars
        {
            get { return oRNumBars; }
            set { oRNumBars = Math.Max(1, value); }
        }

        [Description("Time for price to stay above OR")]
        [GridCategory("Parameters")]
        public int NumBarAsignal
        {
            get { return numBarAsignal; }
            set { numBarAsignal = Math.Max(1, value); }
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
        private ZZACD00Asignal[] cacheZZACD00Asignal = null;

        private static ZZACD00Asignal checkZZACD00Asignal = new ZZACD00Asignal();

        /// <summary>
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        public ZZACD00Asignal ZZACD00Asignal(int numBarAsignal, int oRNumBars)
        {
            return ZZACD00Asignal(Input, numBarAsignal, oRNumBars);
        }

        /// <summary>
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        public ZZACD00Asignal ZZACD00Asignal(Data.IDataSeries input, int numBarAsignal, int oRNumBars)
        {
            if (cacheZZACD00Asignal != null)
                for (int idx = 0; idx < cacheZZACD00Asignal.Length; idx++)
                    if (cacheZZACD00Asignal[idx].NumBarAsignal == numBarAsignal && cacheZZACD00Asignal[idx].ORNumBars == oRNumBars && cacheZZACD00Asignal[idx].EqualsInput(input))
                        return cacheZZACD00Asignal[idx];

            lock (checkZZACD00Asignal)
            {
                checkZZACD00Asignal.NumBarAsignal = numBarAsignal;
                numBarAsignal = checkZZACD00Asignal.NumBarAsignal;
                checkZZACD00Asignal.ORNumBars = oRNumBars;
                oRNumBars = checkZZACD00Asignal.ORNumBars;

                if (cacheZZACD00Asignal != null)
                    for (int idx = 0; idx < cacheZZACD00Asignal.Length; idx++)
                        if (cacheZZACD00Asignal[idx].NumBarAsignal == numBarAsignal && cacheZZACD00Asignal[idx].ORNumBars == oRNumBars && cacheZZACD00Asignal[idx].EqualsInput(input))
                            return cacheZZACD00Asignal[idx];

                ZZACD00Asignal indicator = new ZZACD00Asignal();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NumBarAsignal = numBarAsignal;
                indicator.ORNumBars = oRNumBars;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZACD00Asignal[] tmp = new ZZACD00Asignal[cacheZZACD00Asignal == null ? 1 : cacheZZACD00Asignal.Length + 1];
                if (cacheZZACD00Asignal != null)
                    cacheZZACD00Asignal.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZACD00Asignal = tmp;
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
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZACD00Asignal ZZACD00Asignal(int numBarAsignal, int oRNumBars)
        {
            return _indicator.ZZACD00Asignal(Input, numBarAsignal, oRNumBars);
        }

        /// <summary>
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACD00Asignal ZZACD00Asignal(Data.IDataSeries input, int numBarAsignal, int oRNumBars)
        {
            return _indicator.ZZACD00Asignal(input, numBarAsignal, oRNumBars);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZACD00Asignal ZZACD00Asignal(int numBarAsignal, int oRNumBars)
        {
            return _indicator.ZZACD00Asignal(Input, numBarAsignal, oRNumBars);
        }

        /// <summary>
        /// A signal on ACD return 1, if signal true
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZACD00Asignal ZZACD00Asignal(Data.IDataSeries input, int numBarAsignal, int oRNumBars)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZACD00Asignal(input, numBarAsignal, oRNumBars);
        }
    }
}
#endregion
