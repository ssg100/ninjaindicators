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
    /// Color trends off the 3MA bands
    /// </summary>
    [Description("Color trends off the 3MA bands")]
    public class ZZColorTrend : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int mALongLen = 100; // Default setting for MALongLen
            private int mAMedLen = 50; // Default setting for MAMedLen
            private int mAShortLen = 20; // Default setting for MAShortLen
			private Color			uptick 		= Color.Blue;
			private Color			downtick	= Color.Red;
		// User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Blue, 3), "Trendline"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			double currentEMALong = EMA(mALongLen)[0];
			double currentEMAMed = EMA(mAMedLen)[0];
			double currentEMAShort = EMA(mAShortLen)[0];
			
            if(((currentEMAMed > currentEMALong) || (currentEMAShort > currentEMALong)) )
			{
				Trendline.Set(currentEMALong);
				PlotColors[0][0] = uptick;
				//if(CrossBelow(EMA(mAShortLen),currentEMALong,7))
				if(currentEMAShort < currentEMALong)
					PlotColors[0][0] = downtick;
			}
			else if((currentEMAMed < currentEMALong) || (currentEMAShort < currentEMALong))
			{
				Trendline.Set(currentEMALong);
				PlotColors[0][0] = downtick;
			}
			else{
				Trendline.Set(currentEMALong);
				PlotColors[0][0] = Color.Green;
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Trendline
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MALongLen
        {
            get { return mALongLen; }
            set { mALongLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MAMedLen
        {
            get { return mAMedLen; }
            set { mAMedLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MAShortLen
        {
            get { return mAShortLen; }
            set { mAShortLen = Math.Max(1, value); }
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
        private ZZColorTrend[] cacheZZColorTrend = null;

        private static ZZColorTrend checkZZColorTrend = new ZZColorTrend();

        /// <summary>
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        public ZZColorTrend ZZColorTrend(int mALongLen, int mAMedLen, int mAShortLen)
        {
            return ZZColorTrend(Input, mALongLen, mAMedLen, mAShortLen);
        }

        /// <summary>
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        public ZZColorTrend ZZColorTrend(Data.IDataSeries input, int mALongLen, int mAMedLen, int mAShortLen)
        {
            if (cacheZZColorTrend != null)
                for (int idx = 0; idx < cacheZZColorTrend.Length; idx++)
                    if (cacheZZColorTrend[idx].MALongLen == mALongLen && cacheZZColorTrend[idx].MAMedLen == mAMedLen && cacheZZColorTrend[idx].MAShortLen == mAShortLen && cacheZZColorTrend[idx].EqualsInput(input))
                        return cacheZZColorTrend[idx];

            lock (checkZZColorTrend)
            {
                checkZZColorTrend.MALongLen = mALongLen;
                mALongLen = checkZZColorTrend.MALongLen;
                checkZZColorTrend.MAMedLen = mAMedLen;
                mAMedLen = checkZZColorTrend.MAMedLen;
                checkZZColorTrend.MAShortLen = mAShortLen;
                mAShortLen = checkZZColorTrend.MAShortLen;

                if (cacheZZColorTrend != null)
                    for (int idx = 0; idx < cacheZZColorTrend.Length; idx++)
                        if (cacheZZColorTrend[idx].MALongLen == mALongLen && cacheZZColorTrend[idx].MAMedLen == mAMedLen && cacheZZColorTrend[idx].MAShortLen == mAShortLen && cacheZZColorTrend[idx].EqualsInput(input))
                            return cacheZZColorTrend[idx];

                ZZColorTrend indicator = new ZZColorTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MALongLen = mALongLen;
                indicator.MAMedLen = mAMedLen;
                indicator.MAShortLen = mAShortLen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZColorTrend[] tmp = new ZZColorTrend[cacheZZColorTrend == null ? 1 : cacheZZColorTrend.Length + 1];
                if (cacheZZColorTrend != null)
                    cacheZZColorTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZColorTrend = tmp;
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
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZColorTrend ZZColorTrend(int mALongLen, int mAMedLen, int mAShortLen)
        {
            return _indicator.ZZColorTrend(Input, mALongLen, mAMedLen, mAShortLen);
        }

        /// <summary>
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZColorTrend ZZColorTrend(Data.IDataSeries input, int mALongLen, int mAMedLen, int mAShortLen)
        {
            return _indicator.ZZColorTrend(input, mALongLen, mAMedLen, mAShortLen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZColorTrend ZZColorTrend(int mALongLen, int mAMedLen, int mAShortLen)
        {
            return _indicator.ZZColorTrend(Input, mALongLen, mAMedLen, mAShortLen);
        }

        /// <summary>
        /// Color trends off the 3MA bands
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZColorTrend ZZColorTrend(Data.IDataSeries input, int mALongLen, int mAMedLen, int mAShortLen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZColorTrend(input, mALongLen, mAMedLen, mAShortLen);
        }
    }
}
#endregion
