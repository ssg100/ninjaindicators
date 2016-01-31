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
    /// "Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart"
    /// </summary>
    [Description("Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart")]
    public class SampleMTFKama : Indicator
    {
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Min15_KAMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Min30_KAMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Min60_KAMA"));
            
			CalculateOnBarClose	= true;
            Overlay				= true;
			
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 3;
			
			Add(PeriodType.Minute, 15);
			Add(PeriodType.Minute, 30);
			Add(PeriodType.Minute, 60);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBars[0] < 0 || CurrentBars[1] < 0 || CurrentBars[2] < 0 || CurrentBars[3] < 0)
				return;
			
            Min15_KAMA.Set(KAMA(BarsArray[1], 2, 10, 30)[0]);
            Min30_KAMA.Set(KAMA(BarsArray[2], 2, 10, 30)[0]);
            Min60_KAMA.Set(KAMA(BarsArray[3], 2, 10, 30)[0]);
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Min15_KAMA
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Min30_KAMA
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Min60_KAMA
        {
            get { return Values[2]; }
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
        private SampleMTFKama[] cacheSampleMTFKama = null;

        private static SampleMTFKama checkSampleMTFKama = new SampleMTFKama();

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public SampleMTFKama SampleMTFKama()
        {
            return SampleMTFKama(Input);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public SampleMTFKama SampleMTFKama(Data.IDataSeries input)
        {
            if (cacheSampleMTFKama != null)
                for (int idx = 0; idx < cacheSampleMTFKama.Length; idx++)
                    if (cacheSampleMTFKama[idx].EqualsInput(input))
                        return cacheSampleMTFKama[idx];

            lock (checkSampleMTFKama)
            {
                if (cacheSampleMTFKama != null)
                    for (int idx = 0; idx < cacheSampleMTFKama.Length; idx++)
                        if (cacheSampleMTFKama[idx].EqualsInput(input))
                            return cacheSampleMTFKama[idx];

                SampleMTFKama indicator = new SampleMTFKama();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                SampleMTFKama[] tmp = new SampleMTFKama[cacheSampleMTFKama == null ? 1 : cacheSampleMTFKama.Length + 1];
                if (cacheSampleMTFKama != null)
                    cacheSampleMTFKama.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSampleMTFKama = tmp;
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
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleMTFKama SampleMTFKama()
        {
            return _indicator.SampleMTFKama(Input);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleMTFKama SampleMTFKama(Data.IDataSeries input)
        {
            return _indicator.SampleMTFKama(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleMTFKama SampleMTFKama()
        {
            return _indicator.SampleMTFKama(Input);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleMTFKama SampleMTFKama(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SampleMTFKama(input);
        }
    }
}
#endregion
