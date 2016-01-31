// 
// Written by Ben L. sbgtrading@yahoo.com
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
	/// This indicator is a real-time indicator and does not plot against historical data. Plots the delta between buy and sell transaction volume.  Buy volume being volume trade at or above Ask, Sell volume at or below Bid.
	/// </summary>
	[Description("This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.")]
	public class DeltaBuySellVolume : Indicator
	{
		#region Variables
		private int activeBar = -1;
		private double buys = 0;
		private double sells = 0;
		private bool firstPaint = true;
		private string counterLocation="BR"; //"BottomRight" default value for CounterLocation
		private int CounterLoc=0;
		private double Delta, Total;

		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Red, 1), PlotStyle.Dot, "DeltaDown"));
			Add(new Plot(new Pen(Color.Green, 1), PlotStyle.Dot, "DeltaUp"));
			Add(new Plot(new Pen(Color.Orange, 2), PlotStyle.Line, "Cumulative"));
			Add(new Line(Color.FromKnownColor(KnownColor.DarkSalmon), 0, "Zero"));
			CalculateOnBarClose = false;
			DisplayInDataBox = false;
			PaintPriceMarkers = false;
			PlotsConfigurable = true;
			if(String.Compare(counterLocation,"NONE")==0) CounterLoc=0;
			if(String.Compare(counterLocation,"TR")==0) CounterLoc=1;
			if(String.Compare(counterLocation,"TL")==0) CounterLoc=2;
			if(String.Compare(counterLocation,"BR")==0) CounterLoc=3;
			if(String.Compare(counterLocation,"BL")==0) CounterLoc=4;
			Plots[0].Max = 0;
			Plots[1].Min = 0; 
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < activeBar)
			{return;
			}
			else if (CurrentBar != activeBar)
			{	//Print(Time[0].ToString()+" BST: "+buys.ToString("0")+"-"+sells.ToString("0")+" = "+(buys-sells).ToString("0")+" + "+Total.ToString("0")+" = "+(Total+buys-sells).ToString("0"));
				Total = Total+buys-sells;
				buys = 0;
				sells = 0;
				Delta=0.0;
				activeBar = CurrentBar;
			}
			
			if (firstPaint)
				firstPaint = false;
			else if (Historical) Total = 0.0;
			else
			{	if(!FirstTickOfBar) Delta = buys-sells;
			
				if(CounterLoc==1) DrawTextFixed("voldelta","D: "+Delta.ToString(),TextPosition.TopRight);
				else if(CounterLoc==2) DrawTextFixed("voldelta","D: "+Delta.ToString(),TextPosition.TopLeft);
				else if(CounterLoc==3) DrawTextFixed("voldelta","D: "+Delta.ToString(),TextPosition.BottomRight);
				else if(CounterLoc==4) DrawTextFixed("voldelta","D: "+Delta.ToString(),TextPosition.BottomLeft);

				DeltaUp.Set(Delta); 
				DeltaDown.Set(Delta);
				Cumulative.Set(Total+Delta);
			}
		}

        /// <summary>
        /// Called on each incoming real time market data event
        /// </summary>
        protected override void OnMarketData(MarketDataEventArgs e)
        {
            if (e.MarketDataType != MarketDataType.Last || e.MarketData.Ask == null || e.MarketData.Bid == null)
                return;

            if (e.Price >= e.MarketData.Ask.Price)
                buys += e.Volume;
            else if (e.Price <= e.MarketData.Bid.Price)
                sells += e.Volume;
        }
		#region Properties
		/// <summary>
		/// </summary>
        [Description("Counter location (NONE,TL,TR,BL,BR)")]
        [Category("Parameters")]
        public string CounterLocation
        {
            get { return counterLocation; }
            set { counterLocation = value.ToUpper(); }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DeltaDown
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DeltaUp
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Cumulative
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
        private DeltaBuySellVolume[] cacheDeltaBuySellVolume = null;

        private static DeltaBuySellVolume checkDeltaBuySellVolume = new DeltaBuySellVolume();

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public DeltaBuySellVolume DeltaBuySellVolume(string counterLocation)
        {
            return DeltaBuySellVolume(Input, counterLocation);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public DeltaBuySellVolume DeltaBuySellVolume(Data.IDataSeries input, string counterLocation)
        {
            if (cacheDeltaBuySellVolume != null)
                for (int idx = 0; idx < cacheDeltaBuySellVolume.Length; idx++)
                    if (cacheDeltaBuySellVolume[idx].CounterLocation == counterLocation && cacheDeltaBuySellVolume[idx].EqualsInput(input))
                        return cacheDeltaBuySellVolume[idx];

            lock (checkDeltaBuySellVolume)
            {
                checkDeltaBuySellVolume.CounterLocation = counterLocation;
                counterLocation = checkDeltaBuySellVolume.CounterLocation;

                if (cacheDeltaBuySellVolume != null)
                    for (int idx = 0; idx < cacheDeltaBuySellVolume.Length; idx++)
                        if (cacheDeltaBuySellVolume[idx].CounterLocation == counterLocation && cacheDeltaBuySellVolume[idx].EqualsInput(input))
                            return cacheDeltaBuySellVolume[idx];

                DeltaBuySellVolume indicator = new DeltaBuySellVolume();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CounterLocation = counterLocation;
                Indicators.Add(indicator);
                indicator.SetUp();

                DeltaBuySellVolume[] tmp = new DeltaBuySellVolume[cacheDeltaBuySellVolume == null ? 1 : cacheDeltaBuySellVolume.Length + 1];
                if (cacheDeltaBuySellVolume != null)
                    cacheDeltaBuySellVolume.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDeltaBuySellVolume = tmp;
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
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DeltaBuySellVolume DeltaBuySellVolume(string counterLocation)
        {
            return _indicator.DeltaBuySellVolume(Input, counterLocation);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.DeltaBuySellVolume DeltaBuySellVolume(Data.IDataSeries input, string counterLocation)
        {
            return _indicator.DeltaBuySellVolume(input, counterLocation);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DeltaBuySellVolume DeltaBuySellVolume(string counterLocation)
        {
            return _indicator.DeltaBuySellVolume(Input, counterLocation);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.DeltaBuySellVolume DeltaBuySellVolume(Data.IDataSeries input, string counterLocation)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DeltaBuySellVolume(input, counterLocation);
        }
    }
}
#endregion
