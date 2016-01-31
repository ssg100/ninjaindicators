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
    public class ZZMATrendMultiTimeColor : Indicator
    {
		#region Variables
        // Wizard generated variables
			private int higherTimeFrame = 15;
		// User defined variables (add any user defined variables below)
        #endregion
		
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Min15_KAMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Min30_KAMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Min60_KAMA"));
            
			CalculateOnBarClose	= true;
            Overlay				= false;
			
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 3;
			
			Add(PeriodType.Minute, higherTimeFrame);
			Add(PeriodType.Minute, 30);
			Add(PeriodType.Minute, 60);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int mALongLen = 100; // Default setting for MALongLen
            int mAMedLen = 50; // Default setting for MAMedLen
            int mAShortLen = 20; // Default setting for MAShortLen
			Color			uptick 		= Color.Blue;
			Color			downtick	= Color.Red;
			double currentEMALong = EMA(BarsArray[0],mALongLen)[0];
			double currentEMAMed = EMA(BarsArray[0],mAMedLen)[0];
			double currentEMAShort = EMA(BarsArray[0],mAShortLen)[0];
			
			if (CurrentBars[0] < 0 || CurrentBars[1] < 0 || CurrentBars[2] < 0 || CurrentBars[3] < 0)
				return;
			
			// decide if current chart time frame is long or short
            if(((currentEMAMed > currentEMALong) || (currentEMAShort > currentEMALong)) )
			{
				
				Min15_KAMA.Set(10);
				PlotColors[0][0] = uptick;
				//if(CrossBelow(EMA(mAShortLen),currentEMALong,7))
				if(currentEMAShort < currentEMALong)
					PlotColors[0][0] = downtick;
			}
			else if((currentEMAMed < currentEMALong) || (currentEMAShort < currentEMALong))
			{
				Min15_KAMA.Set(10);
				PlotColors[0][0] = downtick;
			}
			else{
				Min15_KAMA.Set(10);
				PlotColors[0][0] = Color.Green;
			}
			
			currentEMAShort = EMA(BarsArray[1],mAShortLen)[0];
			currentEMAMed   = EMA(BarsArray[1],mAMedLen)[0];
			currentEMALong  = EMA(BarsArray[1],mALongLen)[0];
			
			// decide if 1 time frame higher is long or short
            if(((currentEMAMed > currentEMALong) || (currentEMAShort > currentEMALong)) )
			{
				Min30_KAMA.Set(0);
				PlotColors[1][0] = uptick;
				//if(CrossBelow(EMA(mAShortLen),currentEMALong,7))
				if(currentEMAShort < currentEMALong)
					PlotColors[1][0] = downtick;
			}
			else if((currentEMAMed < currentEMALong) || (currentEMAShort < currentEMALong))
			{
				Min30_KAMA.Set(0);
				PlotColors[1][0] = downtick;
			}
			else{
				Min30_KAMA.Set(0);
				PlotColors[1][0] = Color.Green;
			}
			
            
            //Min60_KAMA.Set(KAMA(BarsArray[3], 2, 10, 30)[0]);
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

		[Description("")]
        [GridCategory("Parameters")]
        public int HigherTimeFrame
        {
            get { return higherTimeFrame; }
            set { higherTimeFrame = Math.Max(1, value); }
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
        private ZZMATrendMultiTimeColor[] cacheZZMATrendMultiTimeColor = null;

        private static ZZMATrendMultiTimeColor checkZZMATrendMultiTimeColor = new ZZMATrendMultiTimeColor();

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(int higherTimeFrame)
        {
            return ZZMATrendMultiTimeColor(Input, higherTimeFrame);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(Data.IDataSeries input, int higherTimeFrame)
        {
            if (cacheZZMATrendMultiTimeColor != null)
                for (int idx = 0; idx < cacheZZMATrendMultiTimeColor.Length; idx++)
                    if (cacheZZMATrendMultiTimeColor[idx].HigherTimeFrame == higherTimeFrame && cacheZZMATrendMultiTimeColor[idx].EqualsInput(input))
                        return cacheZZMATrendMultiTimeColor[idx];

            lock (checkZZMATrendMultiTimeColor)
            {
                checkZZMATrendMultiTimeColor.HigherTimeFrame = higherTimeFrame;
                higherTimeFrame = checkZZMATrendMultiTimeColor.HigherTimeFrame;

                if (cacheZZMATrendMultiTimeColor != null)
                    for (int idx = 0; idx < cacheZZMATrendMultiTimeColor.Length; idx++)
                        if (cacheZZMATrendMultiTimeColor[idx].HigherTimeFrame == higherTimeFrame && cacheZZMATrendMultiTimeColor[idx].EqualsInput(input))
                            return cacheZZMATrendMultiTimeColor[idx];

                ZZMATrendMultiTimeColor indicator = new ZZMATrendMultiTimeColor();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.HigherTimeFrame = higherTimeFrame;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZMATrendMultiTimeColor[] tmp = new ZZMATrendMultiTimeColor[cacheZZMATrendMultiTimeColor == null ? 1 : cacheZZMATrendMultiTimeColor.Length + 1];
                if (cacheZZMATrendMultiTimeColor != null)
                    cacheZZMATrendMultiTimeColor.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZMATrendMultiTimeColor = tmp;
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
        public Indicator.ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(int higherTimeFrame)
        {
            return _indicator.ZZMATrendMultiTimeColor(Input, higherTimeFrame);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(Data.IDataSeries input, int higherTimeFrame)
        {
            return _indicator.ZZMATrendMultiTimeColor(input, higherTimeFrame);
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
        public Indicator.ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(int higherTimeFrame)
        {
            return _indicator.ZZMATrendMultiTimeColor(Input, higherTimeFrame);
        }

        /// <summary>
        /// Sample for basic 15/30/60 min chart intraday KAMA on NT7, best load on 5 min intraday chart
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZMATrendMultiTimeColor ZZMATrendMultiTimeColor(Data.IDataSeries input, int higherTimeFrame)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZMATrendMultiTimeColor(input, higherTimeFrame);
        }
    }
}
#endregion
