// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
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
using NinjaTrader.Data;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
	/// </summary>
	[Description("Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar")]
	public class ZZTapeMonitor : Indicator
	{
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Lime, 2), PlotStyle.Bar, "UpVolume"));
			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Bar, "DownVolume"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Close[0] >= Open[0])
			{
				//Values[0].Set(Volume[0]);
				//Values[1].Reset();
			}
			else
			{
				//Values[1].Set(Volume[0]);
				//Values[0].Reset();
			}
		}
		
		protected override void OnMarketDepth(MarketDepthEventArgs e)
		{/*
    		// Print some data to the Output window
    		if (e.MarketDataType == MarketDataType.Bid && e.Operation == Operation.Update)
			{
         		Print("The most recent bid change is " + e.Price + " " + e.Volume);
				//Values[0].Set(0);
			}
    		if (e.MarketDataType == MarketDataType.Bid && e.Operation == Operation.Insert)
			{
         		Print("The most recent bid insert is " + e.Price + " " + e.Volume);
				//Values[0].Set(0);
			}*/
		}
		
 
 

 

	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private ZZTapeMonitor[] cacheZZTapeMonitor = null;

        private static ZZTapeMonitor checkZZTapeMonitor = new ZZTapeMonitor();

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public ZZTapeMonitor ZZTapeMonitor()
        {
            return ZZTapeMonitor(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public ZZTapeMonitor ZZTapeMonitor(Data.IDataSeries input)
        {
            if (cacheZZTapeMonitor != null)
                for (int idx = 0; idx < cacheZZTapeMonitor.Length; idx++)
                    if (cacheZZTapeMonitor[idx].EqualsInput(input))
                        return cacheZZTapeMonitor[idx];

            lock (checkZZTapeMonitor)
            {
                if (cacheZZTapeMonitor != null)
                    for (int idx = 0; idx < cacheZZTapeMonitor.Length; idx++)
                        if (cacheZZTapeMonitor[idx].EqualsInput(input))
                            return cacheZZTapeMonitor[idx];

                ZZTapeMonitor indicator = new ZZTapeMonitor();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZTapeMonitor[] tmp = new ZZTapeMonitor[cacheZZTapeMonitor == null ? 1 : cacheZZTapeMonitor.Length + 1];
                if (cacheZZTapeMonitor != null)
                    cacheZZTapeMonitor.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZTapeMonitor = tmp;
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
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZTapeMonitor ZZTapeMonitor()
        {
            return _indicator.ZZTapeMonitor(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZTapeMonitor ZZTapeMonitor(Data.IDataSeries input)
        {
            return _indicator.ZZTapeMonitor(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZTapeMonitor ZZTapeMonitor()
        {
            return _indicator.ZZTapeMonitor(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZTapeMonitor ZZTapeMonitor(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZTapeMonitor(input);
        }
    }
}
#endregion
