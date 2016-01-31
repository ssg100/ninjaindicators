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
    /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
    /// </summary>
    [Description("Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator")]
    public class VolatilitySwitch : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int lookBack = 21; // Default setting for Lookback
        // User defined variables (add any user defined variables below)
			private DataSeries dailyChange;
			private DataSeries stdDev;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "VolSwitch"));
			Add(new Line(Color.DarkGray, 0.5, "HLine"));
            Overlay				= false;
			dailyChange			= new DataSeries (this);
			stdDev				= new DataSeries (this);
			Plots[0].Pen.Width	= 2;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 1) return;
			
			dailyChange	.Set((Close[0] - Close[1]) / ((Close[0] + Close[1]) / 2));
			stdDev		.Set(StdDev(dailyChange, LookBack)[0]);
			
			double trueCount = 0;
			for (int x = LookBack; x > 0; x--)
			{
				if(stdDev[x] <= stdDev[0])
					trueCount++;				
			}
			
			VolSwitch	.Set(trueCount / LookBack);
			
			if (VolSwitch[0] >= .5)
				PlotColors[0][0] = Color.Red;
			else
				PlotColors[0][0] = Color.Green;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VolSwitch
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LookBack
        {
            get { return lookBack; }
            set { lookBack = Math.Max(1, value); }
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
        private VolatilitySwitch[] cacheVolatilitySwitch = null;

        private static VolatilitySwitch checkVolatilitySwitch = new VolatilitySwitch();

        /// <summary>
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        public VolatilitySwitch VolatilitySwitch(int lookBack)
        {
            return VolatilitySwitch(Input, lookBack);
        }

        /// <summary>
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        public VolatilitySwitch VolatilitySwitch(Data.IDataSeries input, int lookBack)
        {
            if (cacheVolatilitySwitch != null)
                for (int idx = 0; idx < cacheVolatilitySwitch.Length; idx++)
                    if (cacheVolatilitySwitch[idx].LookBack == lookBack && cacheVolatilitySwitch[idx].EqualsInput(input))
                        return cacheVolatilitySwitch[idx];

            lock (checkVolatilitySwitch)
            {
                checkVolatilitySwitch.LookBack = lookBack;
                lookBack = checkVolatilitySwitch.LookBack;

                if (cacheVolatilitySwitch != null)
                    for (int idx = 0; idx < cacheVolatilitySwitch.Length; idx++)
                        if (cacheVolatilitySwitch[idx].LookBack == lookBack && cacheVolatilitySwitch[idx].EqualsInput(input))
                            return cacheVolatilitySwitch[idx];

                VolatilitySwitch indicator = new VolatilitySwitch();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LookBack = lookBack;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolatilitySwitch[] tmp = new VolatilitySwitch[cacheVolatilitySwitch == null ? 1 : cacheVolatilitySwitch.Length + 1];
                if (cacheVolatilitySwitch != null)
                    cacheVolatilitySwitch.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolatilitySwitch = tmp;
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
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolatilitySwitch VolatilitySwitch(int lookBack)
        {
            return _indicator.VolatilitySwitch(Input, lookBack);
        }

        /// <summary>
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VolatilitySwitch VolatilitySwitch(Data.IDataSeries input, int lookBack)
        {
            return _indicator.VolatilitySwitch(input, lookBack);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolatilitySwitch VolatilitySwitch(int lookBack)
        {
            return _indicator.VolatilitySwitch(Input, lookBack);
        }

        /// <summary>
        /// Stocks and Commodities - February 2013 - The Volatility (Regime) Switch Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VolatilitySwitch VolatilitySwitch(Data.IDataSeries input, int lookBack)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolatilitySwitch(input, lookBack);
        }
    }
}
#endregion
