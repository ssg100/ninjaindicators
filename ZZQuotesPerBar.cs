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
    [Description("Tick per Second")]
    public class ZZQuotesPerBar : Indicator
    {
        #region Variables		
			private int tick_max = 0;
			private int	tick_count = 0;
			private double the_last_second = 0;
			public double quote_count = 0;
        #endregion
      
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
         protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Bar, "TicksPerSecond"));
            Add(new Line(Color.FromKnownColor(KnownColor.Transparent), 0, "ZeroLine"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
			tick_max = 0;
			tick_count = 0;
        }


        /// Called on each bar update event (incoming tick)
        protected override void OnBarUpdate()
        {
            if (Historical) return;
			
			TicksPerSecond.Set(quote_count);
			quote_count=0;
			
			/*
			if(DateTime.Now.Second != the_last_second)
			{
				if(tick_max < tick_count)
				{
				tick_max = tick_count;
				}

			tick_count = 0;
			the_last_second = DateTime.Now.Second;
			}

			tick_count++;			
			

        	*/
		}
		
		protected override void OnMarketDepth(MarketDepthEventArgs e)
		{
    		// Print some data to the Output window
    		if (e.MarketDataType == MarketDataType.Ask && e.Operation == Operation.Update){
				quote_count = quote_count +1;
			}
        	 //Print("The most recent ask change is " + e.Price + " " + e.Volume);
		}
 
 


        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TicksPerSecond
        {
            get { return Values[0]; }
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
        private ZZQuotesPerBar[] cacheZZQuotesPerBar = null;

        private static ZZQuotesPerBar checkZZQuotesPerBar = new ZZQuotesPerBar();

        /// <summary>
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        public ZZQuotesPerBar ZZQuotesPerBar()
        {
            return ZZQuotesPerBar(Input);
        }

        /// <summary>
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        public ZZQuotesPerBar ZZQuotesPerBar(Data.IDataSeries input)
        {
            if (cacheZZQuotesPerBar != null)
                for (int idx = 0; idx < cacheZZQuotesPerBar.Length; idx++)
                    if (cacheZZQuotesPerBar[idx].EqualsInput(input))
                        return cacheZZQuotesPerBar[idx];

            lock (checkZZQuotesPerBar)
            {
                if (cacheZZQuotesPerBar != null)
                    for (int idx = 0; idx < cacheZZQuotesPerBar.Length; idx++)
                        if (cacheZZQuotesPerBar[idx].EqualsInput(input))
                            return cacheZZQuotesPerBar[idx];

                ZZQuotesPerBar indicator = new ZZQuotesPerBar();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZQuotesPerBar[] tmp = new ZZQuotesPerBar[cacheZZQuotesPerBar == null ? 1 : cacheZZQuotesPerBar.Length + 1];
                if (cacheZZQuotesPerBar != null)
                    cacheZZQuotesPerBar.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZQuotesPerBar = tmp;
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
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZQuotesPerBar ZZQuotesPerBar()
        {
            return _indicator.ZZQuotesPerBar(Input);
        }

        /// <summary>
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZQuotesPerBar ZZQuotesPerBar(Data.IDataSeries input)
        {
            return _indicator.ZZQuotesPerBar(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZQuotesPerBar ZZQuotesPerBar()
        {
            return _indicator.ZZQuotesPerBar(Input);
        }

        /// <summary>
        /// Tick per Second
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZQuotesPerBar ZZQuotesPerBar(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZQuotesPerBar(input);
        }
    }
}
#endregion
