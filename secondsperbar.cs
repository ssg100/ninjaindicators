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
    /// number of seconds per bar
    /// </summary>
    [Description(" secondsperbar  number of seconds per bar  > input draws red bar")]
    public class secondsperbar : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int myInput0   = 600; // Default setting for MyInput0  10 minutes
            private int firstline = 300; // Default setting for MyInput0  10 minutes
            private int secondline = 60; // Default setting for MyInput0  10 minutes
			private int sMAPeriod=5;
			private bool drawSMA=true;
		
        // User defined variables (add any user defined variables below)
		int totalseconds;
        #endregion

		TimeSpan durationtime;
		DateTime starttime;
		private DataSeries secperbar;
		
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Add(new Plot(new Pen(Color.LimeGreen, 3), PlotStyle.Bar, "Plot1"));
			Add(new Line(Color.White, secondline, "2nd"));
			Add(new Line(Color.White, firstline, "1st"));

            Overlay				= false;
			secperbar = new DataSeries(this);
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
           // Plot0.Set(Close[0]);
			if( CurrentBar < 1 )
				return;
			
			starttime = Time[1];
			
			durationtime = Time[0] - starttime;
			totalseconds = (int)durationtime.TotalSeconds;
			
			
			if( totalseconds >  myInput0)
			{
				totalseconds=myInput0;
				secperbar.Set(totalseconds);			

				if(drawSMA)
				{
					Plot0.Set( SMA(secperbar,sMAPeriod)[0] );
				}				
				Plot1.Set(totalseconds);
				
//				PlotColors[0][0] = Color.Red;
				PlotColors[1][0] = Color.Red;
			}
 			else
			{
				secperbar.Set(totalseconds);			

				if(drawSMA)
				{
					Plot0.Set( SMA(secperbar,sMAPeriod)[0] );
				}
				
				Plot1.Set(totalseconds);
			}

			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot1
        {
            get { return Values[1]; }
        }


		
        [Description("Number seconds exceeded caused bar to be coloed red")]
		[Gui.Design.DisplayNameAttribute("Signal Seconds")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
		
        [Description("1st horizontal line ")]
		[Gui.Design.DisplayNameAttribute("1st HLine")]
        [GridCategory("Parameters")]
        public int Firstline
        {
            get { return firstline; }
            set { firstline = Math.Max(1, value); }
        }
		
        [Description("2nd horizontal line ")]
		[Gui.Design.DisplayNameAttribute("2nd HLine")]
        [GridCategory("Parameters")]
        public int Secondline
        {
            get { return secondline; }
            set { secondline = Math.Max(1, value); }
        }
		
        [Description("SMA moving average period ")]
		[Gui.Design.DisplayNameAttribute("SMA Period")]
        [GridCategory("Parameters")]
        public int SMAPeriod
        {
            get { return sMAPeriod; }
            set { sMAPeriod = Math.Max(1, value); }
        }
		

		/// <summary>
		/// </summary>
		[Description("True - Draw SMA   False - Do not draw SMA.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Draw SMA")]
		public bool DrawSMA
		{
			get { return drawSMA; }
			set { drawSMA = value; }
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
        private secondsperbar[] cachesecondsperbar = null;

        private static secondsperbar checksecondsperbar = new secondsperbar();

        /// <summary>
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        public secondsperbar secondsperbar(bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            return secondsperbar(Input, drawSMA, firstline, myInput0, secondline, sMAPeriod);
        }

        /// <summary>
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        public secondsperbar secondsperbar(Data.IDataSeries input, bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            if (cachesecondsperbar != null)
                for (int idx = 0; idx < cachesecondsperbar.Length; idx++)
                    if (cachesecondsperbar[idx].DrawSMA == drawSMA && cachesecondsperbar[idx].Firstline == firstline && cachesecondsperbar[idx].MyInput0 == myInput0 && cachesecondsperbar[idx].Secondline == secondline && cachesecondsperbar[idx].SMAPeriod == sMAPeriod && cachesecondsperbar[idx].EqualsInput(input))
                        return cachesecondsperbar[idx];

            lock (checksecondsperbar)
            {
                checksecondsperbar.DrawSMA = drawSMA;
                drawSMA = checksecondsperbar.DrawSMA;
                checksecondsperbar.Firstline = firstline;
                firstline = checksecondsperbar.Firstline;
                checksecondsperbar.MyInput0 = myInput0;
                myInput0 = checksecondsperbar.MyInput0;
                checksecondsperbar.Secondline = secondline;
                secondline = checksecondsperbar.Secondline;
                checksecondsperbar.SMAPeriod = sMAPeriod;
                sMAPeriod = checksecondsperbar.SMAPeriod;

                if (cachesecondsperbar != null)
                    for (int idx = 0; idx < cachesecondsperbar.Length; idx++)
                        if (cachesecondsperbar[idx].DrawSMA == drawSMA && cachesecondsperbar[idx].Firstline == firstline && cachesecondsperbar[idx].MyInput0 == myInput0 && cachesecondsperbar[idx].Secondline == secondline && cachesecondsperbar[idx].SMAPeriod == sMAPeriod && cachesecondsperbar[idx].EqualsInput(input))
                            return cachesecondsperbar[idx];

                secondsperbar indicator = new secondsperbar();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DrawSMA = drawSMA;
                indicator.Firstline = firstline;
                indicator.MyInput0 = myInput0;
                indicator.Secondline = secondline;
                indicator.SMAPeriod = sMAPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                secondsperbar[] tmp = new secondsperbar[cachesecondsperbar == null ? 1 : cachesecondsperbar.Length + 1];
                if (cachesecondsperbar != null)
                    cachesecondsperbar.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachesecondsperbar = tmp;
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
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.secondsperbar secondsperbar(bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            return _indicator.secondsperbar(Input, drawSMA, firstline, myInput0, secondline, sMAPeriod);
        }

        /// <summary>
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        public Indicator.secondsperbar secondsperbar(Data.IDataSeries input, bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            return _indicator.secondsperbar(input, drawSMA, firstline, myInput0, secondline, sMAPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.secondsperbar secondsperbar(bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            return _indicator.secondsperbar(Input, drawSMA, firstline, myInput0, secondline, sMAPeriod);
        }

        /// <summary>
        ///  secondsperbar  number of seconds per bar  > input draws red bar
        /// </summary>
        /// <returns></returns>
        public Indicator.secondsperbar secondsperbar(Data.IDataSeries input, bool drawSMA, int firstline, int myInput0, int secondline, int sMAPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.secondsperbar(input, drawSMA, firstline, myInput0, secondline, sMAPeriod);
        }
    }
}
#endregion
