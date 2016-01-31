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
    /// Measure the size of high or low with EMA difference
    /// </summary>
    [Description("Measure the size of high or low with EMA difference")]
    public class ZZEMASpreadSize : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int eMAlen = 20; // Default setting for EMAlen
			private double eMASpread = 0.0;//
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "EMASpread"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if( Close[0] > EMA(Close,eMAlen)[0])
			{
				eMASpread = High[0] - EMA(Close,eMAlen)[0];
			}
			else
				eMASpread = EMA(Close,eMAlen)[0] - Low[0];
			
            EMASpread.Set(eMASpread);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EMASpread
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMAlen
        {
            get { return eMAlen; }
            set { eMAlen = Math.Max(1, value); }
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
        private ZZEMASpreadSize[] cacheZZEMASpreadSize = null;

        private static ZZEMASpreadSize checkZZEMASpreadSize = new ZZEMASpreadSize();

        /// <summary>
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        public ZZEMASpreadSize ZZEMASpreadSize(int eMAlen)
        {
            return ZZEMASpreadSize(Input, eMAlen);
        }

        /// <summary>
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        public ZZEMASpreadSize ZZEMASpreadSize(Data.IDataSeries input, int eMAlen)
        {
            if (cacheZZEMASpreadSize != null)
                for (int idx = 0; idx < cacheZZEMASpreadSize.Length; idx++)
                    if (cacheZZEMASpreadSize[idx].EMAlen == eMAlen && cacheZZEMASpreadSize[idx].EqualsInput(input))
                        return cacheZZEMASpreadSize[idx];

            lock (checkZZEMASpreadSize)
            {
                checkZZEMASpreadSize.EMAlen = eMAlen;
                eMAlen = checkZZEMASpreadSize.EMAlen;

                if (cacheZZEMASpreadSize != null)
                    for (int idx = 0; idx < cacheZZEMASpreadSize.Length; idx++)
                        if (cacheZZEMASpreadSize[idx].EMAlen == eMAlen && cacheZZEMASpreadSize[idx].EqualsInput(input))
                            return cacheZZEMASpreadSize[idx];

                ZZEMASpreadSize indicator = new ZZEMASpreadSize();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMAlen = eMAlen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZEMASpreadSize[] tmp = new ZZEMASpreadSize[cacheZZEMASpreadSize == null ? 1 : cacheZZEMASpreadSize.Length + 1];
                if (cacheZZEMASpreadSize != null)
                    cacheZZEMASpreadSize.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZEMASpreadSize = tmp;
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
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZEMASpreadSize ZZEMASpreadSize(int eMAlen)
        {
            return _indicator.ZZEMASpreadSize(Input, eMAlen);
        }

        /// <summary>
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZEMASpreadSize ZZEMASpreadSize(Data.IDataSeries input, int eMAlen)
        {
            return _indicator.ZZEMASpreadSize(input, eMAlen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZEMASpreadSize ZZEMASpreadSize(int eMAlen)
        {
            return _indicator.ZZEMASpreadSize(Input, eMAlen);
        }

        /// <summary>
        /// Measure the size of high or low with EMA difference
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZEMASpreadSize ZZEMASpreadSize(Data.IDataSeries input, int eMAlen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZEMASpreadSize(input, eMAlen);
        }
    }
}
#endregion
