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
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
	/// </summary>
	[Description("Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar")]
	public class ZZVolumeSpikeAlert : Indicator
	{
		#region Variables
		private bool						alertenabled		= false;
		#endregion
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Lime, 2), PlotStyle.Bar, "UpVolume"));
			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Bar, "DownVolume"));
			Add(new Plot(new Pen(Color.Black,3),PlotStyle.Bar, "SpikeVolume"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			CalculateOnBarClose = true;
			
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double vol_ema;
			
			vol_ema = EMA(Volume,30)[0];
			
			if (Close[0] >= Open[0])
			{
				Values[0].Set(Volume[0]);
				Values[1].Reset();
				Values[2].Reset();
			}
			else
			{
				Values[1].Set(Volume[0]);
				Values[0].Reset();
				Values[2].Reset();
			}
			
			if (Volume[0] > 2* vol_ema)
			{
				if(alertenabled)
					Alert("VolSpikeAlert", NinjaTrader.Cbi.Priority.High, "VOLUME SPIKE!", "AutoChase.wav",
							10, Color.Black, Color.Yellow);
				
				Values[2].Set(Volume[0]);
				Values[1].Reset();
				Values[0].Reset();	
			}
		}
		
				#region Properties
		/// <summary>
		/// </summary>
		[Description("Enable alert or no")]
		[GridCategory("Parameters")]
		public bool AlertEnabled
		{
			get { return alertenabled; }
			set { alertenabled = value; }
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
        private ZZVolumeSpikeAlert[] cacheZZVolumeSpikeAlert = null;

        private static ZZVolumeSpikeAlert checkZZVolumeSpikeAlert = new ZZVolumeSpikeAlert();

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public ZZVolumeSpikeAlert ZZVolumeSpikeAlert(bool alertEnabled)
        {
            return ZZVolumeSpikeAlert(Input, alertEnabled);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public ZZVolumeSpikeAlert ZZVolumeSpikeAlert(Data.IDataSeries input, bool alertEnabled)
        {
            if (cacheZZVolumeSpikeAlert != null)
                for (int idx = 0; idx < cacheZZVolumeSpikeAlert.Length; idx++)
                    if (cacheZZVolumeSpikeAlert[idx].AlertEnabled == alertEnabled && cacheZZVolumeSpikeAlert[idx].EqualsInput(input))
                        return cacheZZVolumeSpikeAlert[idx];

            lock (checkZZVolumeSpikeAlert)
            {
                checkZZVolumeSpikeAlert.AlertEnabled = alertEnabled;
                alertEnabled = checkZZVolumeSpikeAlert.AlertEnabled;

                if (cacheZZVolumeSpikeAlert != null)
                    for (int idx = 0; idx < cacheZZVolumeSpikeAlert.Length; idx++)
                        if (cacheZZVolumeSpikeAlert[idx].AlertEnabled == alertEnabled && cacheZZVolumeSpikeAlert[idx].EqualsInput(input))
                            return cacheZZVolumeSpikeAlert[idx];

                ZZVolumeSpikeAlert indicator = new ZZVolumeSpikeAlert();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertEnabled = alertEnabled;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZVolumeSpikeAlert[] tmp = new ZZVolumeSpikeAlert[cacheZZVolumeSpikeAlert == null ? 1 : cacheZZVolumeSpikeAlert.Length + 1];
                if (cacheZZVolumeSpikeAlert != null)
                    cacheZZVolumeSpikeAlert.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZVolumeSpikeAlert = tmp;
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
        public Indicator.ZZVolumeSpikeAlert ZZVolumeSpikeAlert(bool alertEnabled)
        {
            return _indicator.ZZVolumeSpikeAlert(Input, alertEnabled);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVolumeSpikeAlert ZZVolumeSpikeAlert(Data.IDataSeries input, bool alertEnabled)
        {
            return _indicator.ZZVolumeSpikeAlert(input, alertEnabled);
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
        public Indicator.ZZVolumeSpikeAlert ZZVolumeSpikeAlert(bool alertEnabled)
        {
            return _indicator.ZZVolumeSpikeAlert(Input, alertEnabled);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVolumeSpikeAlert ZZVolumeSpikeAlert(Data.IDataSeries input, bool alertEnabled)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZVolumeSpikeAlert(input, alertEnabled);
        }
    }
}
#endregion
