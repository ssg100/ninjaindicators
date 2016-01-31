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
    /// Measures how far from EMA before snap back
    /// </summary>
    [Description("Measures how far from EMA before snap back")]
    public class ZZStretchAwayFromEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int eMALen = 20; // Default setting for EMALen
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Navy), PlotStyle.Bar, "Histogram"));
			//Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "HistogramTop"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "HistoOsc"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            double emaDiff;
			double emaVal = EMA(20)[0];
			
			if( Low[0] < emaVal)
				emaDiff = emaVal - Low[0];
			else
				emaDiff = 0;
			
			//Print(emaDiff);
			
            Histogram.Set(emaDiff);
						
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Histogram
        {
            get { return Values[0]; }
        }

		//public DataSeries HistogramTop
        //{
        //    get { return Values[0]; }
        //}
        [Description("")]
        [GridCategory("Parameters")]
        public int EMALen
        {
            get { return eMALen; }
            set { eMALen = Math.Max(1, value); }
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
        private ZZStretchAwayFromEMA[] cacheZZStretchAwayFromEMA = null;

        private static ZZStretchAwayFromEMA checkZZStretchAwayFromEMA = new ZZStretchAwayFromEMA();

        /// <summary>
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        public ZZStretchAwayFromEMA ZZStretchAwayFromEMA(int eMALen)
        {
            return ZZStretchAwayFromEMA(Input, eMALen);
        }

        /// <summary>
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        public ZZStretchAwayFromEMA ZZStretchAwayFromEMA(Data.IDataSeries input, int eMALen)
        {
            if (cacheZZStretchAwayFromEMA != null)
                for (int idx = 0; idx < cacheZZStretchAwayFromEMA.Length; idx++)
                    if (cacheZZStretchAwayFromEMA[idx].EMALen == eMALen && cacheZZStretchAwayFromEMA[idx].EqualsInput(input))
                        return cacheZZStretchAwayFromEMA[idx];

            lock (checkZZStretchAwayFromEMA)
            {
                checkZZStretchAwayFromEMA.EMALen = eMALen;
                eMALen = checkZZStretchAwayFromEMA.EMALen;

                if (cacheZZStretchAwayFromEMA != null)
                    for (int idx = 0; idx < cacheZZStretchAwayFromEMA.Length; idx++)
                        if (cacheZZStretchAwayFromEMA[idx].EMALen == eMALen && cacheZZStretchAwayFromEMA[idx].EqualsInput(input))
                            return cacheZZStretchAwayFromEMA[idx];

                ZZStretchAwayFromEMA indicator = new ZZStretchAwayFromEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMALen = eMALen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZStretchAwayFromEMA[] tmp = new ZZStretchAwayFromEMA[cacheZZStretchAwayFromEMA == null ? 1 : cacheZZStretchAwayFromEMA.Length + 1];
                if (cacheZZStretchAwayFromEMA != null)
                    cacheZZStretchAwayFromEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZStretchAwayFromEMA = tmp;
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
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZStretchAwayFromEMA ZZStretchAwayFromEMA(int eMALen)
        {
            return _indicator.ZZStretchAwayFromEMA(Input, eMALen);
        }

        /// <summary>
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZStretchAwayFromEMA ZZStretchAwayFromEMA(Data.IDataSeries input, int eMALen)
        {
            return _indicator.ZZStretchAwayFromEMA(input, eMALen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZStretchAwayFromEMA ZZStretchAwayFromEMA(int eMALen)
        {
            return _indicator.ZZStretchAwayFromEMA(Input, eMALen);
        }

        /// <summary>
        /// Measures how far from EMA before snap back
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZStretchAwayFromEMA ZZStretchAwayFromEMA(Data.IDataSeries input, int eMALen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZStretchAwayFromEMA(input, eMALen);
        }
    }
}
#endregion
