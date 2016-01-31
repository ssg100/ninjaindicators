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
    /// Bull 180
    /// </summary>
    [Description("Bear 180")]
    public class ZZBear180 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool useMATrend = false; // Default setting for UseMATrend
            private double elephantSizePct = 000; // Default setting for ElephantSizePct
            private bool soundAlert = false; // Default setting for SoundAlert
			private DataSeries Range;
			private DataSeries ElephantBear;
			private DataSeries BearBar;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.TriangleDown, "Bear180"));
            Overlay				= true;
			
			Range = new DataSeries(this);
			ElephantBear = new DataSeries(this);
			BearBar = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double range_avg;
			bool is_elephant_bar;
            bool is_full_bear_bar;
			bool is_full_bull_bar;
			bool is_elephant_bear;
			bool is_elephant_bull;
			bool is_bull180;
			
			Range.Set(High[0]-Low[0]);
			range_avg = SMA(Range, 20)[0];
			
			is_elephant_bar = (Range[0] - ((elephantSizePct/100.0) * range_avg)) > range_avg;
			
			is_full_bear_bar = ( Close[0] < Open[0] ) && ( Open[0] > (High[0] - (0.20 * Range[0])) ) && (Close[0] < (Low[0] + (0.20 * Range[0])) );
			is_full_bull_bar = ( Close[0] > Open[0] ) && ( Close[0] > (High[0] - (0.20 * Range[0])) ) && (Open[0] < (Low[0] + (0.20 * Range[0])) );
			
			if( is_full_bear_bar ) 
				BearBar.Set(1);
			else	
				BearBar.Set(0);
			
			//Print("range_avg=" + range_avg);
			
			is_bull180 = is_full_bull_bar && (BearBar[1] > 0);
			
			if( is_full_bear_bar )//&& is_elephant_bar )
	            Bear180.Set(High[0] + (0.0008 * High[0]));
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bear180
        {
            get { return Values[0]; }
        }

        [Description("use MAs for direction filtering")]
        [GridCategory("Parameters")]
        public bool UseMATrend
        {
            get { return useMATrend; }
            set { useMATrend = value; }
        }

        [Description("How many percent bigger than regular bar to be elephant")]
        [GridCategory("Parameters")]
        public double ElephantSizePct
        {
            get { return elephantSizePct; }
            set { elephantSizePct = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
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
        private ZZBear180[] cacheZZBear180 = null;

        private static ZZBear180 checkZZBear180 = new ZZBear180();

        /// <summary>
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        public ZZBear180 ZZBear180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return ZZBear180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        public ZZBear180 ZZBear180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            if (cacheZZBear180 != null)
                for (int idx = 0; idx < cacheZZBear180.Length; idx++)
                    if (Math.Abs(cacheZZBear180[idx].ElephantSizePct - elephantSizePct) <= double.Epsilon && cacheZZBear180[idx].SoundAlert == soundAlert && cacheZZBear180[idx].UseMATrend == useMATrend && cacheZZBear180[idx].EqualsInput(input))
                        return cacheZZBear180[idx];

            lock (checkZZBear180)
            {
                checkZZBear180.ElephantSizePct = elephantSizePct;
                elephantSizePct = checkZZBear180.ElephantSizePct;
                checkZZBear180.SoundAlert = soundAlert;
                soundAlert = checkZZBear180.SoundAlert;
                checkZZBear180.UseMATrend = useMATrend;
                useMATrend = checkZZBear180.UseMATrend;

                if (cacheZZBear180 != null)
                    for (int idx = 0; idx < cacheZZBear180.Length; idx++)
                        if (Math.Abs(cacheZZBear180[idx].ElephantSizePct - elephantSizePct) <= double.Epsilon && cacheZZBear180[idx].SoundAlert == soundAlert && cacheZZBear180[idx].UseMATrend == useMATrend && cacheZZBear180[idx].EqualsInput(input))
                            return cacheZZBear180[idx];

                ZZBear180 indicator = new ZZBear180();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ElephantSizePct = elephantSizePct;
                indicator.SoundAlert = soundAlert;
                indicator.UseMATrend = useMATrend;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZBear180[] tmp = new ZZBear180[cacheZZBear180 == null ? 1 : cacheZZBear180.Length + 1];
                if (cacheZZBear180 != null)
                    cacheZZBear180.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZBear180 = tmp;
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
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBear180 ZZBear180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBear180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBear180 ZZBear180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBear180(input, elephantSizePct, soundAlert, useMATrend);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBear180 ZZBear180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBear180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bear 180
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBear180 ZZBear180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZBear180(input, elephantSizePct, soundAlert, useMATrend);
        }
    }
}
#endregion
