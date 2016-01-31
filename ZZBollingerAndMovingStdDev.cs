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
	/// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
	/// </summary>
	[Description("ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.")]
	public class ZZBollingerAndMovingStdDev : Indicator
	{
		#region Variables
		private	double		numStdDev	= 20;
		private int			period		= 20;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "Upper band"));
			Add(new Plot(Color.Orange, "Middle band"));
			Add(new Plot(Color.Orange, "Lower band"));

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
		    double smaValue    = SMA(Period)[0];
		    double stdDevValue = StdDev(SMA(Range(), 20),period)[0];
			Print("stdDevValue = " + stdDevValue);
            Upper.Set(smaValue + NumStdDev * stdDevValue);
            Middle.Set(smaValue);
            Lower.Set(smaValue - NumStdDev * stdDevValue);
		}

		#region Properties
		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// Get the middle value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Middle
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# of std. dev.")]
		public double NumStdDev
		{
			get { return numStdDev; }
			set { numStdDev = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// Get the upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
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
        private ZZBollingerAndMovingStdDev[] cacheZZBollingerAndMovingStdDev = null;

        private static ZZBollingerAndMovingStdDev checkZZBollingerAndMovingStdDev = new ZZBollingerAndMovingStdDev();

        /// <summary>
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(double numStdDev, int period)
        {
            return ZZBollingerAndMovingStdDev(Input, numStdDev, period);
        }

        /// <summary>
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(Data.IDataSeries input, double numStdDev, int period)
        {
            if (cacheZZBollingerAndMovingStdDev != null)
                for (int idx = 0; idx < cacheZZBollingerAndMovingStdDev.Length; idx++)
                    if (Math.Abs(cacheZZBollingerAndMovingStdDev[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheZZBollingerAndMovingStdDev[idx].Period == period && cacheZZBollingerAndMovingStdDev[idx].EqualsInput(input))
                        return cacheZZBollingerAndMovingStdDev[idx];

            lock (checkZZBollingerAndMovingStdDev)
            {
                checkZZBollingerAndMovingStdDev.NumStdDev = numStdDev;
                numStdDev = checkZZBollingerAndMovingStdDev.NumStdDev;
                checkZZBollingerAndMovingStdDev.Period = period;
                period = checkZZBollingerAndMovingStdDev.Period;

                if (cacheZZBollingerAndMovingStdDev != null)
                    for (int idx = 0; idx < cacheZZBollingerAndMovingStdDev.Length; idx++)
                        if (Math.Abs(cacheZZBollingerAndMovingStdDev[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheZZBollingerAndMovingStdDev[idx].Period == period && cacheZZBollingerAndMovingStdDev[idx].EqualsInput(input))
                            return cacheZZBollingerAndMovingStdDev[idx];

                ZZBollingerAndMovingStdDev indicator = new ZZBollingerAndMovingStdDev();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NumStdDev = numStdDev;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZBollingerAndMovingStdDev[] tmp = new ZZBollingerAndMovingStdDev[cacheZZBollingerAndMovingStdDev == null ? 1 : cacheZZBollingerAndMovingStdDev.Length + 1];
                if (cacheZZBollingerAndMovingStdDev != null)
                    cacheZZBollingerAndMovingStdDev.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZBollingerAndMovingStdDev = tmp;
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
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(double numStdDev, int period)
        {
            return _indicator.ZZBollingerAndMovingStdDev(Input, numStdDev, period);
        }

        /// <summary>
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(Data.IDataSeries input, double numStdDev, int period)
        {
            return _indicator.ZZBollingerAndMovingStdDev(input, numStdDev, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(double numStdDev, int period)
        {
            return _indicator.ZZBollingerAndMovingStdDev(Input, numStdDev, period);
        }

        /// <summary>
        /// ZZBollingerAndMovingStdDev Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBollingerAndMovingStdDev ZZBollingerAndMovingStdDev(Data.IDataSeries input, double numStdDev, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZBollingerAndMovingStdDev(input, numStdDev, period);
        }
    }
}
#endregion
