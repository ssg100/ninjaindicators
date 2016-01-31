// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots the open, high, low and close values from the session starting on the prior day.
    /// </summary>
    [Description("Plots the open, high, low and close values from the session starting on the prior day.")]
    public class ZZPriorDayOHLCAlerts : Indicator
    {
        #region Variables

        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private DateTime 	currentDate 	= Cbi.Globals.MinDate;
		private double		currentOpen		= 0;
        private double		currentHigh		= 0;
		private double		currentLow		= 0;
		private double		currentClose	= 0;
		private double		priordayOpen	= 0;
		private double		priordayHigh	= 0;
		private double		priordayLow		= 0;
		private double		priordayClose	= 0;
		private double		twodayagoHigh 	= 0;
		private double		twodayagoLow	= 0;
		private bool		showOpen		= true;
		private bool		showHigh		= true;
		private bool		showLow			= true;
		private bool		showClose		= true;
		private bool		alertTwoDaysAgo = true;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, PlotStyle.Hash, "Prior Open"));
            Add(new Plot(Color.Green, PlotStyle.Hash, "Prior High"));
            Add(new Plot(Color.Red, PlotStyle.Hash, "Prior Low"));
            Add(new Plot(Color.Firebrick, PlotStyle.Hash, "Prior Close"));

			Plots[0].Pen.DashStyle = DashStyle.Dash;
			Plots[3].Pen.DashStyle = DashStyle.Dash;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 2;
			
			AutoScale 			= false;
            Overlay				= true;	  // Plots the indicator on top of price
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Bars == null)
				return;

			if (!Bars.BarsType.IsIntraday)
			{
				DrawTextFixed("error msg", "ZZPriorDayOHLCAlerts only works on intraday intervals", TextPosition.BottomRight);
				return;
			}

			// If the current data is not the same date as the current bar then its a new session
			if (currentDate != Bars.GetTradingDayFromLocal(Time[0]) || currentOpen == 0)
			{
				// The current day OHLC values are now the prior days value so set
				// them to their respect indicator series for plotting
				if (currentOpen != 0)
				{
					twodayagoHigh	= priordayHigh;
					twodayagoLow	= priordayLow;
					priordayOpen	= currentOpen;
					priordayHigh	= currentHigh;
					priordayLow		= currentLow;
					priordayClose	= currentClose;

					if (ShowOpen)  PriorOpen.Set(priordayOpen);
					if (ShowHigh)  PriorHigh.Set(priordayHigh);
            		if (ShowLow)   PriorLow.Set(priordayLow);
            		if (ShowClose) PriorClose.Set(priordayClose);
				}
				
				// Initilize the current day settings to the new days data
				currentOpen 	= 	Open[0];
				currentHigh 	= 	High[0];
				currentLow		=	Low[0];
				currentClose	=	Close[0];

				currentDate 	= 	Bars.GetTradingDayFromLocal(Time[0]); 
			}
			else // The current day is the same day
			{
				// Set the current day OHLC values
				currentHigh 	= 	Math.Max(currentHigh, High[0]);
				currentLow		= 	Math.Min(currentLow, Low[0]);
				currentClose	=	Close[0];
				
				// Alert if near HLC
				//Print("priordayHigh = " + priordayHigh + " range = " + (priordayHigh+priordayHigh*0.001) + "  " + (priordayHigh-priordayHigh*0.001) );
				//Print("priordayLow = " + priordayLow + " range = " + (priordayLow-priordayLow*0.001) + " " +(priordayLow+priordayLow*0.001) );
				if(currentClose > (priordayHigh-priordayHigh*0.001) && currentClose < (priordayHigh+priordayHigh*0.001) ){
					Alert("NearPriorHigh", NinjaTrader.Cbi.Priority.High, "Near prior high", "Alert2.wav", 10, Color.Black, Color.Yellow);
				}
				if(currentClose > (priordayLow-priordayLow*0.001) && currentClose < (priordayLow+priordayLow*0.001) ){
					Alert("NearPriorLow", NinjaTrader.Cbi.Priority.High, "Near prior low", "Alert2.wav", 10, Color.Black, Color.Yellow);
				}
				
				// Alert if near two days ago high or low
				if(currentClose > (twodayagoHigh-twodayagoHigh*0.001) && currentClose < (twodayagoHigh+twodayagoHigh*0.001) ){
					Alert("NearTwoDayAgoHigh", NinjaTrader.Cbi.Priority.High, "Near 2-day Ago high", "Alert2.wav", 10, Color.Black, Color.Yellow);
				}
				if(currentClose > (twodayagoLow-twodayagoLow*0.001) && currentClose < (twodayagoLow+twodayagoLow*0.001) ){
					Alert("NearTwoDayAgoLow", NinjaTrader.Cbi.Priority.High, "Near 2-day Ago low", "Alert2.wav", 10, Color.Black, Color.Yellow);
				}
				
                if (ShowOpen) PriorOpen.Set(priordayOpen);
                if (ShowHigh) PriorHigh.Set(priordayHigh);
                if (ShowLow) PriorLow.Set(priordayLow);
                if (ShowClose) PriorClose.Set(priordayClose);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorOpen
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorHigh
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorLow
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorClose
        {
            get { return Values[3]; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show open")]
        public bool ShowOpen
        {
            get { return showOpen; }
			set { showOpen = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show high")]
        public bool ShowHigh
        {
            get { return showHigh; }
			set { showHigh = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show low")]
        public bool ShowLow
        {
            get { return showLow; }
			set { showLow = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show close")]
        public bool ShowClose
        {
            get { return showClose; }
			set { showClose = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Alert 2 days ago")]
        public bool AlertTwoDaysAgo
        {
            get { return alertTwoDaysAgo; }
			set { showClose = value; }
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
        private ZZPriorDayOHLCAlerts[] cacheZZPriorDayOHLCAlerts = null;

        private static ZZPriorDayOHLCAlerts checkZZPriorDayOHLCAlerts = new ZZPriorDayOHLCAlerts();

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts()
        {
            return ZZPriorDayOHLCAlerts(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts(Data.IDataSeries input)
        {
            if (cacheZZPriorDayOHLCAlerts != null)
                for (int idx = 0; idx < cacheZZPriorDayOHLCAlerts.Length; idx++)
                    if (cacheZZPriorDayOHLCAlerts[idx].EqualsInput(input))
                        return cacheZZPriorDayOHLCAlerts[idx];

            lock (checkZZPriorDayOHLCAlerts)
            {
                if (cacheZZPriorDayOHLCAlerts != null)
                    for (int idx = 0; idx < cacheZZPriorDayOHLCAlerts.Length; idx++)
                        if (cacheZZPriorDayOHLCAlerts[idx].EqualsInput(input))
                            return cacheZZPriorDayOHLCAlerts[idx];

                ZZPriorDayOHLCAlerts indicator = new ZZPriorDayOHLCAlerts();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZPriorDayOHLCAlerts[] tmp = new ZZPriorDayOHLCAlerts[cacheZZPriorDayOHLCAlerts == null ? 1 : cacheZZPriorDayOHLCAlerts.Length + 1];
                if (cacheZZPriorDayOHLCAlerts != null)
                    cacheZZPriorDayOHLCAlerts.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZPriorDayOHLCAlerts = tmp;
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
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts()
        {
            return _indicator.ZZPriorDayOHLCAlerts(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts(Data.IDataSeries input)
        {
            return _indicator.ZZPriorDayOHLCAlerts(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts()
        {
            return _indicator.ZZPriorDayOHLCAlerts(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZPriorDayOHLCAlerts ZZPriorDayOHLCAlerts(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZPriorDayOHLCAlerts(input);
        }
    }
}
#endregion
