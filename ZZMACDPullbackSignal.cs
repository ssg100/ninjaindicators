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
    /// MACD pulling back signal for strategy
    /// </summary>
    [Description("MACD pulling back signal for strategy")]
    public class ZZMACDPullbackSignal : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int mACDShortLen = 10; // Default setting for MACDShortLen
            private int mACDLongLen = 24; // Default setting for MACDLongLen
			private DataSeries macdDropUpperThres;
			private DataSeries macdCrossUpperThres;
			private DataSeries macdDropLowerThres;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.TriangleUp, "BuySignal"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.TriangleDown, "SellSignal"));
			macdDropUpperThres = new DataSeries(this, MaximumBarsLookBack.Infinite);
			macdCrossUpperThres = new DataSeries(this, MaximumBarsLookBack.Infinite);
			macdDropLowerThres = new DataSeries(this, MaximumBarsLookBack.Infinite);
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			double currentMacd;
			bool buySignal1;
			bool buySignal2;
			bool buySignal3;
			bool buySignal4;
			
			currentMacd = MACD(Close,mACDShortLen, mACDLongLen, 9)[0];
			
			if(CrossBelow(MACD(Close,mACDShortLen, mACDLongLen, 9), 0.03, 1))
				macdDropUpperThres[0] = 1;
			else
				macdDropUpperThres[0] = 0;
			
			if(CrossAbove(MACD(Close,mACDShortLen, mACDLongLen, 9), 0.02, 1))
				macdCrossUpperThres[0] = 1;
			else
				macdCrossUpperThres[0] = 0;
			
			
			buySignal3 = !CrossBelow(MACD(Close,mACDShortLen, mACDLongLen, 9), -0.06, 15);
			buySignal4 = EMA(Close,20)[0] > EMA(Close,100)[0];
			
			if( CrossAbove(macdDropUpperThres,0.5,10) && macdCrossUpperThres[0]==1)
				BuySignal.Set(Low[0]);
			//if(macdDropUpperThres[0]==1)
           // SellSignal.Set(Low[0]);
			
			
			
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BuySignal
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SellSignal
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDShortLen
        {
            get { return mACDShortLen; }
            set { mACDShortLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDLongLen
        {
            get { return mACDLongLen; }
            set { mACDLongLen = Math.Max(1, value); }
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
        private ZZMACDPullbackSignal[] cacheZZMACDPullbackSignal = null;

        private static ZZMACDPullbackSignal checkZZMACDPullbackSignal = new ZZMACDPullbackSignal();

        /// <summary>
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        public ZZMACDPullbackSignal ZZMACDPullbackSignal(int mACDLongLen, int mACDShortLen)
        {
            return ZZMACDPullbackSignal(Input, mACDLongLen, mACDShortLen);
        }

        /// <summary>
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        public ZZMACDPullbackSignal ZZMACDPullbackSignal(Data.IDataSeries input, int mACDLongLen, int mACDShortLen)
        {
            if (cacheZZMACDPullbackSignal != null)
                for (int idx = 0; idx < cacheZZMACDPullbackSignal.Length; idx++)
                    if (cacheZZMACDPullbackSignal[idx].MACDLongLen == mACDLongLen && cacheZZMACDPullbackSignal[idx].MACDShortLen == mACDShortLen && cacheZZMACDPullbackSignal[idx].EqualsInput(input))
                        return cacheZZMACDPullbackSignal[idx];

            lock (checkZZMACDPullbackSignal)
            {
                checkZZMACDPullbackSignal.MACDLongLen = mACDLongLen;
                mACDLongLen = checkZZMACDPullbackSignal.MACDLongLen;
                checkZZMACDPullbackSignal.MACDShortLen = mACDShortLen;
                mACDShortLen = checkZZMACDPullbackSignal.MACDShortLen;

                if (cacheZZMACDPullbackSignal != null)
                    for (int idx = 0; idx < cacheZZMACDPullbackSignal.Length; idx++)
                        if (cacheZZMACDPullbackSignal[idx].MACDLongLen == mACDLongLen && cacheZZMACDPullbackSignal[idx].MACDShortLen == mACDShortLen && cacheZZMACDPullbackSignal[idx].EqualsInput(input))
                            return cacheZZMACDPullbackSignal[idx];

                ZZMACDPullbackSignal indicator = new ZZMACDPullbackSignal();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MACDLongLen = mACDLongLen;
                indicator.MACDShortLen = mACDShortLen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZMACDPullbackSignal[] tmp = new ZZMACDPullbackSignal[cacheZZMACDPullbackSignal == null ? 1 : cacheZZMACDPullbackSignal.Length + 1];
                if (cacheZZMACDPullbackSignal != null)
                    cacheZZMACDPullbackSignal.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZMACDPullbackSignal = tmp;
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
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZMACDPullbackSignal ZZMACDPullbackSignal(int mACDLongLen, int mACDShortLen)
        {
            return _indicator.ZZMACDPullbackSignal(Input, mACDLongLen, mACDShortLen);
        }

        /// <summary>
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZMACDPullbackSignal ZZMACDPullbackSignal(Data.IDataSeries input, int mACDLongLen, int mACDShortLen)
        {
            return _indicator.ZZMACDPullbackSignal(input, mACDLongLen, mACDShortLen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZMACDPullbackSignal ZZMACDPullbackSignal(int mACDLongLen, int mACDShortLen)
        {
            return _indicator.ZZMACDPullbackSignal(Input, mACDLongLen, mACDShortLen);
        }

        /// <summary>
        /// MACD pulling back signal for strategy
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZMACDPullbackSignal ZZMACDPullbackSignal(Data.IDataSeries input, int mACDLongLen, int mACDShortLen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZMACDPullbackSignal(input, mACDLongLen, mACDShortLen);
        }
    }
}
#endregion
