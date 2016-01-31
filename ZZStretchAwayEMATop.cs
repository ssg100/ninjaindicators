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
    /// Measure the stretch for above EMA
    /// </summary>
    [Description("Measure the stretch for above EMA")]
    public class ZZStretchAwayEMATop : Indicator
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
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Bar, "HistoTop"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            double emaDiff;
			double emaVal = EMA(20)[0];
			
			if( High[0] > emaVal)
				emaDiff = High[0]-emaVal ;
			else
				emaDiff = 0;
			
            HistoTop.Set(emaDiff);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HistoTop
        {
            get { return Values[0]; }
        }

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
        private ZZStretchAwayEMATop[] cacheZZStretchAwayEMATop = null;

        private static ZZStretchAwayEMATop checkZZStretchAwayEMATop = new ZZStretchAwayEMATop();

        /// <summary>
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        public ZZStretchAwayEMATop ZZStretchAwayEMATop(int eMALen)
        {
            return ZZStretchAwayEMATop(Input, eMALen);
        }

        /// <summary>
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        public ZZStretchAwayEMATop ZZStretchAwayEMATop(Data.IDataSeries input, int eMALen)
        {
            if (cacheZZStretchAwayEMATop != null)
                for (int idx = 0; idx < cacheZZStretchAwayEMATop.Length; idx++)
                    if (cacheZZStretchAwayEMATop[idx].EMALen == eMALen && cacheZZStretchAwayEMATop[idx].EqualsInput(input))
                        return cacheZZStretchAwayEMATop[idx];

            lock (checkZZStretchAwayEMATop)
            {
                checkZZStretchAwayEMATop.EMALen = eMALen;
                eMALen = checkZZStretchAwayEMATop.EMALen;

                if (cacheZZStretchAwayEMATop != null)
                    for (int idx = 0; idx < cacheZZStretchAwayEMATop.Length; idx++)
                        if (cacheZZStretchAwayEMATop[idx].EMALen == eMALen && cacheZZStretchAwayEMATop[idx].EqualsInput(input))
                            return cacheZZStretchAwayEMATop[idx];

                ZZStretchAwayEMATop indicator = new ZZStretchAwayEMATop();
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

                ZZStretchAwayEMATop[] tmp = new ZZStretchAwayEMATop[cacheZZStretchAwayEMATop == null ? 1 : cacheZZStretchAwayEMATop.Length + 1];
                if (cacheZZStretchAwayEMATop != null)
                    cacheZZStretchAwayEMATop.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZStretchAwayEMATop = tmp;
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
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZStretchAwayEMATop ZZStretchAwayEMATop(int eMALen)
        {
            return _indicator.ZZStretchAwayEMATop(Input, eMALen);
        }

        /// <summary>
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZStretchAwayEMATop ZZStretchAwayEMATop(Data.IDataSeries input, int eMALen)
        {
            return _indicator.ZZStretchAwayEMATop(input, eMALen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZStretchAwayEMATop ZZStretchAwayEMATop(int eMALen)
        {
            return _indicator.ZZStretchAwayEMATop(Input, eMALen);
        }

        /// <summary>
        /// Measure the stretch for above EMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZStretchAwayEMATop ZZStretchAwayEMATop(Data.IDataSeries input, int eMALen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZStretchAwayEMATop(input, eMALen);
        }
    }
}
#endregion
