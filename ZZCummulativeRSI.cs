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
    /// Cummulative RSI2 to be used with strategy
    /// </summary>
    [Description("Cummulative RSI2 to be used with strategy")]
    public class ZZCummulativeRSI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int rSILen = 2; // Default setting for RSILen
            private int cummRSIThres = 50; // Default setting for CummRSIThres
            private int numDaysX = 2; // Default setting for NumDaysX
			private double CummRSI = 0;
			private DataSeries CummRSI_DS;
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "BuySignal"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "CummRSIPlot"));
			
			CummRSI_DS = new DataSeries(this, MaximumBarsLookBack.Infinite);
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < rSILen + 10) 
			{
				return;
			}
			
			CummRSI_DS[1] = CummRSI;
			CummRSI=0;
			
			for( int i=0; i<numDaysX; i++ ){
			
				CummRSI = CummRSI + RSI(Close,2,1)[i]; 
			
			}
			
			CummRSI_DS[0]= CummRSI;
			
			//CummRSIPlot.Set(CummRSI);
			//Print("CummRSI = " + CummRSI + "  CummRSI_DS[1] = " + CummRSI_DS[1] + " CummRSI_DS[0] = " + CummRSI_DS[0]);
			if( (CummRSI_DS[1] >= cummRSIThres) && (CummRSI_DS[0] < cummRSIThres) )
			{
				BuySignal.Set(5);
			}
			else
			{
				BuySignal.Set(0);
			}
			
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BuySignal
        {
            get { return Values[0]; }
        }
        //public DataSeries CummRSIPlot
        //{
        //    get { return Values[0]; }
        //}

        [Description("RSI length")]
        [GridCategory("Parameters")]
        public int RSILen
        {
            get { return rSILen; }
            set { rSILen = Math.Max(1, value); }
        }

        [Description("Threshold before trigger a signal")]
        [GridCategory("Parameters")]
        public int CummRSIThres
        {
            get { return cummRSIThres; }
            set { cummRSIThres = Math.Max(1, value); }
        }

        [Description("X number of days")]
        [GridCategory("Parameters")]
        public int NumDaysX
        {
            get { return numDaysX; }
            set { numDaysX = Math.Max(1, value); }
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
        private ZZCummulativeRSI[] cacheZZCummulativeRSI = null;

        private static ZZCummulativeRSI checkZZCummulativeRSI = new ZZCummulativeRSI();

        /// <summary>
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        public ZZCummulativeRSI ZZCummulativeRSI(int cummRSIThres, int numDaysX, int rSILen)
        {
            return ZZCummulativeRSI(Input, cummRSIThres, numDaysX, rSILen);
        }

        /// <summary>
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        public ZZCummulativeRSI ZZCummulativeRSI(Data.IDataSeries input, int cummRSIThres, int numDaysX, int rSILen)
        {
            if (cacheZZCummulativeRSI != null)
                for (int idx = 0; idx < cacheZZCummulativeRSI.Length; idx++)
                    if (cacheZZCummulativeRSI[idx].CummRSIThres == cummRSIThres && cacheZZCummulativeRSI[idx].NumDaysX == numDaysX && cacheZZCummulativeRSI[idx].RSILen == rSILen && cacheZZCummulativeRSI[idx].EqualsInput(input))
                        return cacheZZCummulativeRSI[idx];

            lock (checkZZCummulativeRSI)
            {
                checkZZCummulativeRSI.CummRSIThres = cummRSIThres;
                cummRSIThres = checkZZCummulativeRSI.CummRSIThres;
                checkZZCummulativeRSI.NumDaysX = numDaysX;
                numDaysX = checkZZCummulativeRSI.NumDaysX;
                checkZZCummulativeRSI.RSILen = rSILen;
                rSILen = checkZZCummulativeRSI.RSILen;

                if (cacheZZCummulativeRSI != null)
                    for (int idx = 0; idx < cacheZZCummulativeRSI.Length; idx++)
                        if (cacheZZCummulativeRSI[idx].CummRSIThres == cummRSIThres && cacheZZCummulativeRSI[idx].NumDaysX == numDaysX && cacheZZCummulativeRSI[idx].RSILen == rSILen && cacheZZCummulativeRSI[idx].EqualsInput(input))
                            return cacheZZCummulativeRSI[idx];

                ZZCummulativeRSI indicator = new ZZCummulativeRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CummRSIThres = cummRSIThres;
                indicator.NumDaysX = numDaysX;
                indicator.RSILen = rSILen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZCummulativeRSI[] tmp = new ZZCummulativeRSI[cacheZZCummulativeRSI == null ? 1 : cacheZZCummulativeRSI.Length + 1];
                if (cacheZZCummulativeRSI != null)
                    cacheZZCummulativeRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZCummulativeRSI = tmp;
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
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZCummulativeRSI ZZCummulativeRSI(int cummRSIThres, int numDaysX, int rSILen)
        {
            return _indicator.ZZCummulativeRSI(Input, cummRSIThres, numDaysX, rSILen);
        }

        /// <summary>
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZCummulativeRSI ZZCummulativeRSI(Data.IDataSeries input, int cummRSIThres, int numDaysX, int rSILen)
        {
            return _indicator.ZZCummulativeRSI(input, cummRSIThres, numDaysX, rSILen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZCummulativeRSI ZZCummulativeRSI(int cummRSIThres, int numDaysX, int rSILen)
        {
            return _indicator.ZZCummulativeRSI(Input, cummRSIThres, numDaysX, rSILen);
        }

        /// <summary>
        /// Cummulative RSI2 to be used with strategy
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZCummulativeRSI ZZCummulativeRSI(Data.IDataSeries input, int cummRSIThres, int numDaysX, int rSILen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZCummulativeRSI(input, cummRSIThres, numDaysX, rSILen);
        }
    }
}
#endregion
