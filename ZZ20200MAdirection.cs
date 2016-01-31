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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class ZZ20200MAdirection : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int shortMA = 20; // Default setting for ShortMA
            private int longMA = 200; // Default setting for LongMA
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			if (SMA(Close, shortMA)[0] > SMA(Close, longMA)[0])
			{
				//ITextFixed textFixed = DrawTextFixed("tag1", "NO SHORT !! 20>200MA", TextPosition.BottomRight);
				DrawTextFixed("tag1", "NO SHORT !! 20>200MA", TextPosition.BottomRight, Color.Red, new Font("Arial", 14f), Color.Empty, Color.Empty, 10);
				//textFixed.TextColor = Color.Red;
			}
			else
			{
				DrawTextFixed("tag1", "NO LONG !! 20<200MA", TextPosition.BottomRight, Color.Red, new Font("Arial", 14f), Color.Empty, Color.Empty, 10);
			}

		}
		

        #region Properties

        [Description("short ma")]
        [GridCategory("Parameters")]
        public int ShortMA
        {
            get { return shortMA; }
            set { shortMA = Math.Max(1, value); }
        }

        [Description("long ma")]
        [GridCategory("Parameters")]
        public int LongMA
        {
            get { return longMA; }
            set { longMA = Math.Max(1, value); }
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
        private ZZ20200MAdirection[] cacheZZ20200MAdirection = null;

        private static ZZ20200MAdirection checkZZ20200MAdirection = new ZZ20200MAdirection();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ZZ20200MAdirection ZZ20200MAdirection(int longMA, int shortMA)
        {
            return ZZ20200MAdirection(Input, longMA, shortMA);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ZZ20200MAdirection ZZ20200MAdirection(Data.IDataSeries input, int longMA, int shortMA)
        {
            if (cacheZZ20200MAdirection != null)
                for (int idx = 0; idx < cacheZZ20200MAdirection.Length; idx++)
                    if (cacheZZ20200MAdirection[idx].LongMA == longMA && cacheZZ20200MAdirection[idx].ShortMA == shortMA && cacheZZ20200MAdirection[idx].EqualsInput(input))
                        return cacheZZ20200MAdirection[idx];

            lock (checkZZ20200MAdirection)
            {
                checkZZ20200MAdirection.LongMA = longMA;
                longMA = checkZZ20200MAdirection.LongMA;
                checkZZ20200MAdirection.ShortMA = shortMA;
                shortMA = checkZZ20200MAdirection.ShortMA;

                if (cacheZZ20200MAdirection != null)
                    for (int idx = 0; idx < cacheZZ20200MAdirection.Length; idx++)
                        if (cacheZZ20200MAdirection[idx].LongMA == longMA && cacheZZ20200MAdirection[idx].ShortMA == shortMA && cacheZZ20200MAdirection[idx].EqualsInput(input))
                            return cacheZZ20200MAdirection[idx];

                ZZ20200MAdirection indicator = new ZZ20200MAdirection();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LongMA = longMA;
                indicator.ShortMA = shortMA;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZ20200MAdirection[] tmp = new ZZ20200MAdirection[cacheZZ20200MAdirection == null ? 1 : cacheZZ20200MAdirection.Length + 1];
                if (cacheZZ20200MAdirection != null)
                    cacheZZ20200MAdirection.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZ20200MAdirection = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZ20200MAdirection ZZ20200MAdirection(int longMA, int shortMA)
        {
            return _indicator.ZZ20200MAdirection(Input, longMA, shortMA);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZ20200MAdirection ZZ20200MAdirection(Data.IDataSeries input, int longMA, int shortMA)
        {
            return _indicator.ZZ20200MAdirection(input, longMA, shortMA);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZ20200MAdirection ZZ20200MAdirection(int longMA, int shortMA)
        {
            return _indicator.ZZ20200MAdirection(Input, longMA, shortMA);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZ20200MAdirection ZZ20200MAdirection(Data.IDataSeries input, int longMA, int shortMA)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZ20200MAdirection(input, longMA, shortMA);
        }
    }
}
#endregion
