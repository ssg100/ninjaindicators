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
    /// MA slope
    /// </summary>
    [Description("MA slope")]
    public class ZZSlopeMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int mALen = 50; // Default setting for MALen
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "MASlope"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            MASlope.Set(Slope(SMA(50),4,0));
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MASlope
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MALen
        {
            get { return mALen; }
            set { mALen = Math.Max(1, value); }
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
        private ZZSlopeMA[] cacheZZSlopeMA = null;

        private static ZZSlopeMA checkZZSlopeMA = new ZZSlopeMA();

        /// <summary>
        /// MA slope
        /// </summary>
        /// <returns></returns>
        public ZZSlopeMA ZZSlopeMA(int mALen)
        {
            return ZZSlopeMA(Input, mALen);
        }

        /// <summary>
        /// MA slope
        /// </summary>
        /// <returns></returns>
        public ZZSlopeMA ZZSlopeMA(Data.IDataSeries input, int mALen)
        {
            if (cacheZZSlopeMA != null)
                for (int idx = 0; idx < cacheZZSlopeMA.Length; idx++)
                    if (cacheZZSlopeMA[idx].MALen == mALen && cacheZZSlopeMA[idx].EqualsInput(input))
                        return cacheZZSlopeMA[idx];

            lock (checkZZSlopeMA)
            {
                checkZZSlopeMA.MALen = mALen;
                mALen = checkZZSlopeMA.MALen;

                if (cacheZZSlopeMA != null)
                    for (int idx = 0; idx < cacheZZSlopeMA.Length; idx++)
                        if (cacheZZSlopeMA[idx].MALen == mALen && cacheZZSlopeMA[idx].EqualsInput(input))
                            return cacheZZSlopeMA[idx];

                ZZSlopeMA indicator = new ZZSlopeMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MALen = mALen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZSlopeMA[] tmp = new ZZSlopeMA[cacheZZSlopeMA == null ? 1 : cacheZZSlopeMA.Length + 1];
                if (cacheZZSlopeMA != null)
                    cacheZZSlopeMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZSlopeMA = tmp;
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
        /// MA slope
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSlopeMA ZZSlopeMA(int mALen)
        {
            return _indicator.ZZSlopeMA(Input, mALen);
        }

        /// <summary>
        /// MA slope
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSlopeMA ZZSlopeMA(Data.IDataSeries input, int mALen)
        {
            return _indicator.ZZSlopeMA(input, mALen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MA slope
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSlopeMA ZZSlopeMA(int mALen)
        {
            return _indicator.ZZSlopeMA(Input, mALen);
        }

        /// <summary>
        /// MA slope
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSlopeMA ZZSlopeMA(Data.IDataSeries input, int mALen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZSlopeMA(input, mALen);
        }
    }
}
#endregion
