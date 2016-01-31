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
    [Description("Bull 180")]
    public class ZZBull180 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool useMATrend = false; // Default setting for UseMATrend
            private double elephantSizePct = 50.000; // Default setting for ElephantSizePct
            private bool soundAlert = false; // Default setting for SoundAlert
			private DataSeries Range;
			private DataSeries ElephantBear;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.TriangleUp, "Bull180"));
            Overlay				= true;
			
			Range = new DataSeries(this);
			ElephantBear = new DataSeries(this);
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
			
			Range.Set(High[0]-Low[0]);
			range_avg = SMA(Range, 20)[0];
			
			is_elephant_bar = (Range[0] - ((elephantSizePct/100) * range_avg)) > range_avg;
			
			is_full_bull_bar = ( Close[0] > Open[0] ) && ( Close[0] > (High[0] - (0.20 * Range[0])) ) && (Open[0] < (Low[0] + (0.20 * Range[0])) ) ;
			
			//Print("range_avg=" + range_avg);
			
			
			if( is_full_bull_bar && is_elephant_bar )
	            Bull180.Set(Low[0] - (0.0002 * Low[0]));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bull180
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
        private ZZBull180[] cacheZZBull180 = null;

        private static ZZBull180 checkZZBull180 = new ZZBull180();

        /// <summary>
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        public ZZBull180 ZZBull180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return ZZBull180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        public ZZBull180 ZZBull180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            if (cacheZZBull180 != null)
                for (int idx = 0; idx < cacheZZBull180.Length; idx++)
                    if (Math.Abs(cacheZZBull180[idx].ElephantSizePct - elephantSizePct) <= double.Epsilon && cacheZZBull180[idx].SoundAlert == soundAlert && cacheZZBull180[idx].UseMATrend == useMATrend && cacheZZBull180[idx].EqualsInput(input))
                        return cacheZZBull180[idx];

            lock (checkZZBull180)
            {
                checkZZBull180.ElephantSizePct = elephantSizePct;
                elephantSizePct = checkZZBull180.ElephantSizePct;
                checkZZBull180.SoundAlert = soundAlert;
                soundAlert = checkZZBull180.SoundAlert;
                checkZZBull180.UseMATrend = useMATrend;
                useMATrend = checkZZBull180.UseMATrend;

                if (cacheZZBull180 != null)
                    for (int idx = 0; idx < cacheZZBull180.Length; idx++)
                        if (Math.Abs(cacheZZBull180[idx].ElephantSizePct - elephantSizePct) <= double.Epsilon && cacheZZBull180[idx].SoundAlert == soundAlert && cacheZZBull180[idx].UseMATrend == useMATrend && cacheZZBull180[idx].EqualsInput(input))
                            return cacheZZBull180[idx];

                ZZBull180 indicator = new ZZBull180();
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

                ZZBull180[] tmp = new ZZBull180[cacheZZBull180 == null ? 1 : cacheZZBull180.Length + 1];
                if (cacheZZBull180 != null)
                    cacheZZBull180.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZBull180 = tmp;
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
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBull180 ZZBull180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBull180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBull180 ZZBull180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBull180(input, elephantSizePct, soundAlert, useMATrend);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBull180 ZZBull180(double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            return _indicator.ZZBull180(Input, elephantSizePct, soundAlert, useMATrend);
        }

        /// <summary>
        /// Bull 180
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBull180 ZZBull180(Data.IDataSeries input, double elephantSizePct, bool soundAlert, bool useMATrend)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZBull180(input, elephantSizePct, soundAlert, useMATrend);
        }
    }
}
#endregion
