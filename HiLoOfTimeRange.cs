#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

//Written by Ben, sbgtrading@yahoo.com, November 22, 2007
//
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots the high and low of a specific, prior time range
    /// </summary>
    [Description("Plots the high and low of a specific, past time range for the current day")]
    public class HiLoOfTimeRange : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int startTimeHr = 8; // Default setting for StartTimeHr
            private int startTimeMinute = 0; // Default setting for StartTimeMinute
            private int endTimeHr = 10; // Default setting for EndTimeHr
            private int endTimeMinute = 30; // Default setting for EndTimeMinute
        // User defined variables (add any user defined variables below)
			string id="x",previd="";
			double h=0.0;
			double l=0.0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSlateBlue), PlotStyle.Line, "TheHigh"));
            Add(new Plot(Color.FromKnownColor(KnownColor.HotPink), PlotStyle.Line, "TheLow"));
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			id=ToDay(Time[0]).ToString();
			if(String.Compare(id,previd)==0) 
			{	TheHigh.Set(h);
    	        TheLow.Set(l);
				return; //don't do the same day again, wait for new day
			}
			
			int BarDay = Time[0].Day;
			int BarMonth = Time[0].Month;
			int BarYear = Time[0].Year;
			int StartBar = CurrentBar - Bars.GetBar(new DateTime(BarYear, BarMonth, BarDay, startTimeHr, startTimeMinute, 0));
			int EndBar = CurrentBar - Bars.GetBar(new DateTime(BarYear, BarMonth, BarDay, endTimeHr, endTimeMinute, 0));

			if(StartBar == EndBar) //if the time range is not on the chart, exit
			{	TheHigh.Set(h); //carry forward the last known h and l values
    	        TheLow.Set(l);
				return;
			}

			if(EndBar>=0) //if the end of the timeframe has already arrived
			{	h=High[StartBar];
				l=Low[StartBar];
				for (int i=StartBar-1;i>=EndBar;i--)
				{	if(High[i]>h) h=High[i];
					if(Low[i]<l) l=Low[i];
				}
				if(StartBar<CurrentBar && EndBar<CurrentBar && StartBar>0)
				{   //DrawRectangle("R"+Time[0].ToString(),false,StartBar,h,EndBar,l,Color.Blue, Color.Transparent, 1);
					previd = id;
					if(pShowCrossingLines) {
						DrawLine("L1"+id,false,StartBar,h,EndBar,l,Color.Blue,DashStyle.Dot,1);
						DrawLine("L2"+id,false,StartBar,l,EndBar,h,Color.Blue,DashStyle.Dot,1);
					}
					DrawLine("L3"+id,false,EndBar,l,EndBar,h,Color.Green,DashStyle.Dot,3);
					DrawLine("L4"+id,false,StartBar,l,StartBar,h,Color.Red,DashStyle.Dot,3);
				}
			}
			TheHigh.Set(h);
            TheLow.Set(l);
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TheHigh
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TheLow
        {
            get { return Values[1]; }
        }

        [Description("MAKE SURE YOU USE 24hr TIME FORMAT!")]
        [Category("Parameters")]
        public int _StartTimeHr
        {
            get { return startTimeHr; }
            set { startTimeHr = Math.Max(0, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int _StartTimeMinute
        {
            get { return startTimeMinute; }
            set { startTimeMinute = Math.Max(0, value); }
        }

        [Description("MAKE SURE YOU USE 24hr TIME FORMAT!")]
        [Category("Parameters")]
        public int EndTimeHr
        {
            get { return endTimeHr; }
            set { endTimeHr = Math.Max(0, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int EndTimeMinute
        {
            get { return endTimeMinute; }
            set { endTimeMinute = Math.Max(0, value); }
        }
		
		private bool pShowCrossingLines = true;
        [Description("Show the crossing lines of the setup range?")]
        [Category("Parameters")]
        public bool ShowCrossingLines
        {
            get { return pShowCrossingLines; }
            set { pShowCrossingLines = value; }
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
        private HiLoOfTimeRange[] cacheHiLoOfTimeRange = null;

        private static HiLoOfTimeRange checkHiLoOfTimeRange = new HiLoOfTimeRange();

        /// <summary>
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        public HiLoOfTimeRange HiLoOfTimeRange(int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            return HiLoOfTimeRange(Input, _StartTimeHr, _StartTimeMinute, endTimeHr, endTimeMinute, showCrossingLines);
        }

        /// <summary>
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        public HiLoOfTimeRange HiLoOfTimeRange(Data.IDataSeries input, int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            if (cacheHiLoOfTimeRange != null)
                for (int idx = 0; idx < cacheHiLoOfTimeRange.Length; idx++)
                    if (cacheHiLoOfTimeRange[idx]._StartTimeHr == _StartTimeHr && cacheHiLoOfTimeRange[idx]._StartTimeMinute == _StartTimeMinute && cacheHiLoOfTimeRange[idx].EndTimeHr == endTimeHr && cacheHiLoOfTimeRange[idx].EndTimeMinute == endTimeMinute && cacheHiLoOfTimeRange[idx].ShowCrossingLines == showCrossingLines && cacheHiLoOfTimeRange[idx].EqualsInput(input))
                        return cacheHiLoOfTimeRange[idx];

            lock (checkHiLoOfTimeRange)
            {
                checkHiLoOfTimeRange._StartTimeHr = _StartTimeHr;
                _StartTimeHr = checkHiLoOfTimeRange._StartTimeHr;
                checkHiLoOfTimeRange._StartTimeMinute = _StartTimeMinute;
                _StartTimeMinute = checkHiLoOfTimeRange._StartTimeMinute;
                checkHiLoOfTimeRange.EndTimeHr = endTimeHr;
                endTimeHr = checkHiLoOfTimeRange.EndTimeHr;
                checkHiLoOfTimeRange.EndTimeMinute = endTimeMinute;
                endTimeMinute = checkHiLoOfTimeRange.EndTimeMinute;
                checkHiLoOfTimeRange.ShowCrossingLines = showCrossingLines;
                showCrossingLines = checkHiLoOfTimeRange.ShowCrossingLines;

                if (cacheHiLoOfTimeRange != null)
                    for (int idx = 0; idx < cacheHiLoOfTimeRange.Length; idx++)
                        if (cacheHiLoOfTimeRange[idx]._StartTimeHr == _StartTimeHr && cacheHiLoOfTimeRange[idx]._StartTimeMinute == _StartTimeMinute && cacheHiLoOfTimeRange[idx].EndTimeHr == endTimeHr && cacheHiLoOfTimeRange[idx].EndTimeMinute == endTimeMinute && cacheHiLoOfTimeRange[idx].ShowCrossingLines == showCrossingLines && cacheHiLoOfTimeRange[idx].EqualsInput(input))
                            return cacheHiLoOfTimeRange[idx];

                HiLoOfTimeRange indicator = new HiLoOfTimeRange();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator._StartTimeHr = _StartTimeHr;
                indicator._StartTimeMinute = _StartTimeMinute;
                indicator.EndTimeHr = endTimeHr;
                indicator.EndTimeMinute = endTimeMinute;
                indicator.ShowCrossingLines = showCrossingLines;
                Indicators.Add(indicator);
                indicator.SetUp();

                HiLoOfTimeRange[] tmp = new HiLoOfTimeRange[cacheHiLoOfTimeRange == null ? 1 : cacheHiLoOfTimeRange.Length + 1];
                if (cacheHiLoOfTimeRange != null)
                    cacheHiLoOfTimeRange.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHiLoOfTimeRange = tmp;
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
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HiLoOfTimeRange HiLoOfTimeRange(int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            return _indicator.HiLoOfTimeRange(Input, _StartTimeHr, _StartTimeMinute, endTimeHr, endTimeMinute, showCrossingLines);
        }

        /// <summary>
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        public Indicator.HiLoOfTimeRange HiLoOfTimeRange(Data.IDataSeries input, int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            return _indicator.HiLoOfTimeRange(input, _StartTimeHr, _StartTimeMinute, endTimeHr, endTimeMinute, showCrossingLines);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HiLoOfTimeRange HiLoOfTimeRange(int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            return _indicator.HiLoOfTimeRange(Input, _StartTimeHr, _StartTimeMinute, endTimeHr, endTimeMinute, showCrossingLines);
        }

        /// <summary>
        /// Plots the high and low of a specific, past time range for the current day
        /// </summary>
        /// <returns></returns>
        public Indicator.HiLoOfTimeRange HiLoOfTimeRange(Data.IDataSeries input, int _StartTimeHr, int _StartTimeMinute, int endTimeHr, int endTimeMinute, bool showCrossingLines)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HiLoOfTimeRange(input, _StartTimeHr, _StartTimeMinute, endTimeHr, endTimeMinute, showCrossingLines);
        }
    }
}
#endregion
