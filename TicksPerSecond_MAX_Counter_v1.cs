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
    [Description("Tick per Second MAX")]
    public class TicksPerSecond_MAX_Counter_v1 : Indicator
    {
        #region Variables
		
			private int tick_max = 0;
			private int	tick_count = 0;
			private double the_last_second = 0;
        #endregion
      
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
         protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Bar, "TicksPerSecond"));
            Add(new Line(Color.FromKnownColor(KnownColor.Transparent), 0, "ZeroLine"));
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
			tick_max = 0;
			tick_count = 0;
        }


        /// Called on each bar update event (incoming tick)
        protected override void OnBarUpdate()
        {
            if (Historical) return;
			if (FirstTickOfBar)	
			{tick_max = 0;}
				
			if(DateTime.Now.Second != the_last_second)
			{
				if(tick_max < tick_count) 
				{tick_max = tick_count;}
				
			tick_count = 0;
			the_last_second = DateTime.Now.Second;
			}
			
			tick_count++;			
			
			TicksPerSecond.Set(tick_max);
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
        private TicksPerSecond_MAX_Counter_v1[] cacheTicksPerSecond_MAX_Counter_v1 = null;

        private static TicksPerSecond_MAX_Counter_v1 checkTicksPerSecond_MAX_Counter_v1 = new TicksPerSecond_MAX_Counter_v1();

        /// <summary>
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        public TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1()
        {
            return TicksPerSecond_MAX_Counter_v1(Input);
        }

        /// <summary>
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        public TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1(Data.IDataSeries input)
        {
            if (cacheTicksPerSecond_MAX_Counter_v1 != null)
                for (int idx = 0; idx < cacheTicksPerSecond_MAX_Counter_v1.Length; idx++)
                    if (cacheTicksPerSecond_MAX_Counter_v1[idx].EqualsInput(input))
                        return cacheTicksPerSecond_MAX_Counter_v1[idx];

            lock (checkTicksPerSecond_MAX_Counter_v1)
            {
                if (cacheTicksPerSecond_MAX_Counter_v1 != null)
                    for (int idx = 0; idx < cacheTicksPerSecond_MAX_Counter_v1.Length; idx++)
                        if (cacheTicksPerSecond_MAX_Counter_v1[idx].EqualsInput(input))
                            return cacheTicksPerSecond_MAX_Counter_v1[idx];

                TicksPerSecond_MAX_Counter_v1 indicator = new TicksPerSecond_MAX_Counter_v1();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                TicksPerSecond_MAX_Counter_v1[] tmp = new TicksPerSecond_MAX_Counter_v1[cacheTicksPerSecond_MAX_Counter_v1 == null ? 1 : cacheTicksPerSecond_MAX_Counter_v1.Length + 1];
                if (cacheTicksPerSecond_MAX_Counter_v1 != null)
                    cacheTicksPerSecond_MAX_Counter_v1.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTicksPerSecond_MAX_Counter_v1 = tmp;
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
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1()
        {
            return _indicator.TicksPerSecond_MAX_Counter_v1(Input);
        }

        /// <summary>
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        public Indicator.TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1(Data.IDataSeries input)
        {
            return _indicator.TicksPerSecond_MAX_Counter_v1(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1()
        {
            return _indicator.TicksPerSecond_MAX_Counter_v1(Input);
        }

        /// <summary>
        /// Tick per Second MAX
        /// </summary>
        /// <returns></returns>
        public Indicator.TicksPerSecond_MAX_Counter_v1 TicksPerSecond_MAX_Counter_v1(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TicksPerSecond_MAX_Counter_v1(input);
        }
    }
}
#endregion
