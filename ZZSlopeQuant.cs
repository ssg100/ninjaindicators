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
    /// Quantifying slope od EMA
    /// </summary>
    [Description("Quantifying slope od EMA")]
    public class ZZSlopeQuant : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int lookbackBars = 8; // Default setting for LookbackBars
            private int eMAPeriod = 20; // Default setting for EMAPeriod
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "Slope"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			 if (CurrentBar < lookbackBars + 2) 
			{
				return;
			}
            double tempSlope = 0.00;
			
			
			tempSlope = EMA(Close,eMAPeriod)[0] - EMA(Close, eMAPeriod)[lookbackBars];
			
			
            Slope.Set(tempSlope);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Slope
        {
            get { return Values[0]; }
        }

        [Description("Number of bars to look back for slope calculation")]
        [GridCategory("Parameters")]
        public int LookbackBars
        {
            get { return lookbackBars; }
            set { lookbackBars = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMAPeriod
        {
            get { return eMAPeriod; }
            set { eMAPeriod = Math.Max(1, value); }
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
        private ZZSlopeQuant[] cacheZZSlopeQuant = null;

        private static ZZSlopeQuant checkZZSlopeQuant = new ZZSlopeQuant();

        /// <summary>
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        public ZZSlopeQuant ZZSlopeQuant(int eMAPeriod, int lookbackBars)
        {
            return ZZSlopeQuant(Input, eMAPeriod, lookbackBars);
        }

        /// <summary>
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        public ZZSlopeQuant ZZSlopeQuant(Data.IDataSeries input, int eMAPeriod, int lookbackBars)
        {
            if (cacheZZSlopeQuant != null)
                for (int idx = 0; idx < cacheZZSlopeQuant.Length; idx++)
                    if (cacheZZSlopeQuant[idx].EMAPeriod == eMAPeriod && cacheZZSlopeQuant[idx].LookbackBars == lookbackBars && cacheZZSlopeQuant[idx].EqualsInput(input))
                        return cacheZZSlopeQuant[idx];

            lock (checkZZSlopeQuant)
            {
                checkZZSlopeQuant.EMAPeriod = eMAPeriod;
                eMAPeriod = checkZZSlopeQuant.EMAPeriod;
                checkZZSlopeQuant.LookbackBars = lookbackBars;
                lookbackBars = checkZZSlopeQuant.LookbackBars;

                if (cacheZZSlopeQuant != null)
                    for (int idx = 0; idx < cacheZZSlopeQuant.Length; idx++)
                        if (cacheZZSlopeQuant[idx].EMAPeriod == eMAPeriod && cacheZZSlopeQuant[idx].LookbackBars == lookbackBars && cacheZZSlopeQuant[idx].EqualsInput(input))
                            return cacheZZSlopeQuant[idx];

                ZZSlopeQuant indicator = new ZZSlopeQuant();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMAPeriod = eMAPeriod;
                indicator.LookbackBars = lookbackBars;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZSlopeQuant[] tmp = new ZZSlopeQuant[cacheZZSlopeQuant == null ? 1 : cacheZZSlopeQuant.Length + 1];
                if (cacheZZSlopeQuant != null)
                    cacheZZSlopeQuant.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZSlopeQuant = tmp;
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
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSlopeQuant ZZSlopeQuant(int eMAPeriod, int lookbackBars)
        {
            return _indicator.ZZSlopeQuant(Input, eMAPeriod, lookbackBars);
        }

        /// <summary>
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSlopeQuant ZZSlopeQuant(Data.IDataSeries input, int eMAPeriod, int lookbackBars)
        {
            return _indicator.ZZSlopeQuant(input, eMAPeriod, lookbackBars);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZSlopeQuant ZZSlopeQuant(int eMAPeriod, int lookbackBars)
        {
            return _indicator.ZZSlopeQuant(Input, eMAPeriod, lookbackBars);
        }

        /// <summary>
        /// Quantifying slope od EMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZSlopeQuant ZZSlopeQuant(Data.IDataSeries input, int eMAPeriod, int lookbackBars)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZSlopeQuant(input, eMAPeriod, lookbackBars);
        }
    }
}
#endregion
