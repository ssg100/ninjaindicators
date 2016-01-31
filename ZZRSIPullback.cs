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
    /// Signal buy or sell on pullback off RSI on 3 MAs
    /// </summary>
    [Description("Signal buy or sell on pullback off RSI on 3 MAs")]
    public class ZZRSIPullback : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int shortMA = 20; // Default setting for ShortMA
            private int medMA = 50; // Default setting for MedMA
            private int longMA = 100; // Default setting for LongMA
            private int rSIThres = 1; // Default setting for RSIThres
			private int alertOn = 0; // alert on or off
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkGreen), PlotStyle.TriangleUp, "Buy"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.TriangleDown, "Sell"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            
			bool condition1, condition2, condition3;
			
			//condition1 = (EMA(shortMA)[0] >= EMA(longMA)[0]) || (EMA(medMA)[0] >= EMA(longMA)[0]);
			condition1 = (EMA(shortMA)[0] >= EMA(longMA)[0]) && (EMA(medMA)[0] >= EMA(longMA)[0]);
			
			condition2 = RSI(2,2)[0] <15;
			
			condition3 = Close[0] > EMA(longMA)[0];
			
			if( condition1 && condition2 && condition3 )
			{
				if(alertOn>0)
					Alert("Long", NinjaTrader.Cbi.Priority.Medium, "BuySignal", "Alert1.wav", 300, Color.Black, Color.Yellow);
				
				Buy.Set(Low[0]);
            }
			
			bool condition4, condition5, condition6;
			
			condition4 = (EMA(shortMA)[0] <= EMA(longMA)[0]) && (EMA(medMA)[0] <= EMA(longMA)[0]);
			
			condition5 = RSI(2,2)[0] > 85;
			
			condition6 = Close[0] < EMA(longMA)[0];
			
			if( condition4 && condition5 && condition6 )
			{
				if(alertOn>0)
					Alert("Short", NinjaTrader.Cbi.Priority.Medium, "SellSignal", "Alert1.wav", 300, Color.Black, Color.Yellow);
				Sell.Set(High[0]);
        	}
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Buy
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Sell
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ShortMA
        {
            get { return shortMA; }
            set { shortMA = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int AlertOn
        {
            get { return alertOn; }
            set { alertOn = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int MedMA
        {
            get { return medMA; }
            set { medMA = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int LongMA
        {
            get { return longMA; }
            set { longMA = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int RSIThres
        {
            get { return rSIThres; }
            set { rSIThres = Math.Max(1, value); }
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
        private ZZRSIPullback[] cacheZZRSIPullback = null;

        private static ZZRSIPullback checkZZRSIPullback = new ZZRSIPullback();

        /// <summary>
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        public ZZRSIPullback ZZRSIPullback(int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            return ZZRSIPullback(Input, alertOn, longMA, medMA, rSIThres, shortMA);
        }

        /// <summary>
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        public ZZRSIPullback ZZRSIPullback(Data.IDataSeries input, int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            if (cacheZZRSIPullback != null)
                for (int idx = 0; idx < cacheZZRSIPullback.Length; idx++)
                    if (cacheZZRSIPullback[idx].AlertOn == alertOn && cacheZZRSIPullback[idx].LongMA == longMA && cacheZZRSIPullback[idx].MedMA == medMA && cacheZZRSIPullback[idx].RSIThres == rSIThres && cacheZZRSIPullback[idx].ShortMA == shortMA && cacheZZRSIPullback[idx].EqualsInput(input))
                        return cacheZZRSIPullback[idx];

            lock (checkZZRSIPullback)
            {
                checkZZRSIPullback.AlertOn = alertOn;
                alertOn = checkZZRSIPullback.AlertOn;
                checkZZRSIPullback.LongMA = longMA;
                longMA = checkZZRSIPullback.LongMA;
                checkZZRSIPullback.MedMA = medMA;
                medMA = checkZZRSIPullback.MedMA;
                checkZZRSIPullback.RSIThres = rSIThres;
                rSIThres = checkZZRSIPullback.RSIThres;
                checkZZRSIPullback.ShortMA = shortMA;
                shortMA = checkZZRSIPullback.ShortMA;

                if (cacheZZRSIPullback != null)
                    for (int idx = 0; idx < cacheZZRSIPullback.Length; idx++)
                        if (cacheZZRSIPullback[idx].AlertOn == alertOn && cacheZZRSIPullback[idx].LongMA == longMA && cacheZZRSIPullback[idx].MedMA == medMA && cacheZZRSIPullback[idx].RSIThres == rSIThres && cacheZZRSIPullback[idx].ShortMA == shortMA && cacheZZRSIPullback[idx].EqualsInput(input))
                            return cacheZZRSIPullback[idx];

                ZZRSIPullback indicator = new ZZRSIPullback();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertOn = alertOn;
                indicator.LongMA = longMA;
                indicator.MedMA = medMA;
                indicator.RSIThres = rSIThres;
                indicator.ShortMA = shortMA;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZRSIPullback[] tmp = new ZZRSIPullback[cacheZZRSIPullback == null ? 1 : cacheZZRSIPullback.Length + 1];
                if (cacheZZRSIPullback != null)
                    cacheZZRSIPullback.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZRSIPullback = tmp;
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
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZRSIPullback ZZRSIPullback(int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            return _indicator.ZZRSIPullback(Input, alertOn, longMA, medMA, rSIThres, shortMA);
        }

        /// <summary>
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZRSIPullback ZZRSIPullback(Data.IDataSeries input, int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            return _indicator.ZZRSIPullback(input, alertOn, longMA, medMA, rSIThres, shortMA);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZRSIPullback ZZRSIPullback(int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            return _indicator.ZZRSIPullback(Input, alertOn, longMA, medMA, rSIThres, shortMA);
        }

        /// <summary>
        /// Signal buy or sell on pullback off RSI on 3 MAs
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZRSIPullback ZZRSIPullback(Data.IDataSeries input, int alertOn, int longMA, int medMA, int rSIThres, int shortMA)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZRSIPullback(input, alertOn, longMA, medMA, rSIThres, shortMA);
        }
    }
}
#endregion
