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
    /// Signals when EMA bands contracting
    /// </summary>
    [Description("Signals when EMA bands contracting")]
    public class ZZTighteningMABands : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int eMAshortLen = 20; // Default setting for EMAshortLen
            private int eMAmedLen = 50; // Default setting for EMAmedLen
            private int eMAlongLen = 100; // Default setting for EMAlongLen
            private bool alertOn = false; // Default setting for AlertOn
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Crimson), PlotStyle.Line, "HighTightRange"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "LowTightRange"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double macd1, macd2,macd3;
			
			macd1 = MACD(Close,eMAshortLen,eMAmedLen,10)[0];
			
			macd2 = MACD(Close,eMAmedLen,eMAlongLen,10)[0];
			
			macd3 = MACD(Close,eMAshortLen,eMAlongLen,10)[0];
			
			if(  ((macd1 < 0.05) && (macd1 >= 0)) 
				/*&&  ((macd2 < 0.05) && (macd2 >= 0))  
				&& ((macd2 < 0.05) && (macd2 >= 0)) */   
				&& EMA(eMAmedLen)[0] > EMA(eMAlongLen)[0] 
				
				
				
				)
			{
				
				HighTightRange.Set(High[0]);
				LowTightRange.Set(Low[0]);
				
			}
			
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.

        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HighTightRange
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowTightRange
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMAshortLen
        {
            get { return eMAshortLen; }
            set { eMAshortLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMAmedLen
        {
            get { return eMAmedLen; }
            set { eMAmedLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMAlongLen
        {
            get { return eMAlongLen; }
            set { eMAlongLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public bool AlertOn
        {
            get { return alertOn; }
            set { alertOn = value; }
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
        private ZZTighteningMABands[] cacheZZTighteningMABands = null;

        private static ZZTighteningMABands checkZZTighteningMABands = new ZZTighteningMABands();

        /// <summary>
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        public ZZTighteningMABands ZZTighteningMABands(bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            return ZZTighteningMABands(Input, alertOn, eMAlongLen, eMAmedLen, eMAshortLen);
        }

        /// <summary>
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        public ZZTighteningMABands ZZTighteningMABands(Data.IDataSeries input, bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            if (cacheZZTighteningMABands != null)
                for (int idx = 0; idx < cacheZZTighteningMABands.Length; idx++)
                    if (cacheZZTighteningMABands[idx].AlertOn == alertOn && cacheZZTighteningMABands[idx].EMAlongLen == eMAlongLen && cacheZZTighteningMABands[idx].EMAmedLen == eMAmedLen && cacheZZTighteningMABands[idx].EMAshortLen == eMAshortLen && cacheZZTighteningMABands[idx].EqualsInput(input))
                        return cacheZZTighteningMABands[idx];

            lock (checkZZTighteningMABands)
            {
                checkZZTighteningMABands.AlertOn = alertOn;
                alertOn = checkZZTighteningMABands.AlertOn;
                checkZZTighteningMABands.EMAlongLen = eMAlongLen;
                eMAlongLen = checkZZTighteningMABands.EMAlongLen;
                checkZZTighteningMABands.EMAmedLen = eMAmedLen;
                eMAmedLen = checkZZTighteningMABands.EMAmedLen;
                checkZZTighteningMABands.EMAshortLen = eMAshortLen;
                eMAshortLen = checkZZTighteningMABands.EMAshortLen;

                if (cacheZZTighteningMABands != null)
                    for (int idx = 0; idx < cacheZZTighteningMABands.Length; idx++)
                        if (cacheZZTighteningMABands[idx].AlertOn == alertOn && cacheZZTighteningMABands[idx].EMAlongLen == eMAlongLen && cacheZZTighteningMABands[idx].EMAmedLen == eMAmedLen && cacheZZTighteningMABands[idx].EMAshortLen == eMAshortLen && cacheZZTighteningMABands[idx].EqualsInput(input))
                            return cacheZZTighteningMABands[idx];

                ZZTighteningMABands indicator = new ZZTighteningMABands();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertOn = alertOn;
                indicator.EMAlongLen = eMAlongLen;
                indicator.EMAmedLen = eMAmedLen;
                indicator.EMAshortLen = eMAshortLen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZTighteningMABands[] tmp = new ZZTighteningMABands[cacheZZTighteningMABands == null ? 1 : cacheZZTighteningMABands.Length + 1];
                if (cacheZZTighteningMABands != null)
                    cacheZZTighteningMABands.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZTighteningMABands = tmp;
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
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZTighteningMABands ZZTighteningMABands(bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            return _indicator.ZZTighteningMABands(Input, alertOn, eMAlongLen, eMAmedLen, eMAshortLen);
        }

        /// <summary>
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZTighteningMABands ZZTighteningMABands(Data.IDataSeries input, bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            return _indicator.ZZTighteningMABands(input, alertOn, eMAlongLen, eMAmedLen, eMAshortLen);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZTighteningMABands ZZTighteningMABands(bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            return _indicator.ZZTighteningMABands(Input, alertOn, eMAlongLen, eMAmedLen, eMAshortLen);
        }

        /// <summary>
        /// Signals when EMA bands contracting
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZTighteningMABands ZZTighteningMABands(Data.IDataSeries input, bool alertOn, int eMAlongLen, int eMAmedLen, int eMAshortLen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZTighteningMABands(input, alertOn, eMAlongLen, eMAmedLen, eMAshortLen);
        }
    }
}
#endregion
