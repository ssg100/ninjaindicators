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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Alerts you when there is a volume spike
    /// </summary>
    [Description("Alerts you when there is a volume spike")]
    public class VolumeSpike : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double spike = 0.80; // Default setting for Spike
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 1)
				return;
			
			if (Volume[0] >= Volume[1] * (1 + Spike))
			{
     			DrawDiamond("Volume Spike" + CurrentBar, true, 0, Low[0] - TickSize, Color.Gold);
        		// Generates an alert
				Alert("VolSpike", NinjaTrader.Cbi.Priority.High, "Vol Spiked!", "Alert1.wav", 10, Color.Black, Color.Yellow);
			}
		}
        #region Properties

        [Description("Percentage increase to be considered a spike")]
        [Category("Parameters")]
        public double Spike
        {
            get { return spike; }
            set { spike = Math.Max(0.0, value); }
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
        private VolumeSpike[] cacheVolumeSpike = null;

        private static VolumeSpike checkVolumeSpike = new VolumeSpike();

        /// <summary>
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        public VolumeSpike VolumeSpike(double spike)
        {
            return VolumeSpike(Input, spike);
        }

        /// <summary>
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        public VolumeSpike VolumeSpike(Data.IDataSeries input, double spike)
        {
            if (cacheVolumeSpike != null)
                for (int idx = 0; idx < cacheVolumeSpike.Length; idx++)
                    if (Math.Abs(cacheVolumeSpike[idx].Spike - spike) <= double.Epsilon && cacheVolumeSpike[idx].EqualsInput(input))
                        return cacheVolumeSpike[idx];

            lock (checkVolumeSpike)
            {
                checkVolumeSpike.Spike = spike;
                spike = checkVolumeSpike.Spike;

                if (cacheVolumeSpike != null)
                    for (int idx = 0; idx < cacheVolumeSpike.Length; idx++)
                        if (Math.Abs(cacheVolumeSpike[idx].Spike - spike) <= double.Epsilon && cacheVolumeSpike[idx].EqualsInput(input))
                            return cacheVolumeSpike[idx];

                VolumeSpike indicator = new VolumeSpike();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Spike = spike;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeSpike[] tmp = new VolumeSpike[cacheVolumeSpike == null ? 1 : cacheVolumeSpike.Length + 1];
                if (cacheVolumeSpike != null)
                    cacheVolumeSpike.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeSpike = tmp;
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
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeSpike VolumeSpike(double spike)
        {
            return _indicator.VolumeSpike(Input, spike);
        }

        /// <summary>
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeSpike VolumeSpike(Data.IDataSeries input, double spike)
        {
            return _indicator.VolumeSpike(input, spike);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeSpike VolumeSpike(double spike)
        {
            return _indicator.VolumeSpike(Input, spike);
        }

        /// <summary>
        /// Alerts you when there is a volume spike
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeSpike VolumeSpike(Data.IDataSeries input, double spike)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeSpike(input, spike);
        }
    }
}
#endregion
