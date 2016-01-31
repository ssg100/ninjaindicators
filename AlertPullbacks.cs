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
    /// Alert on any pullback met requirment of AS
    /// </summary>
    [Description("Alert on any pullback met requirment of AS")]
    public class AlertPullbacks : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool useMACD = true; // Default setting for UseMACD
            private bool useMomentum = true; // Default setting for UseMomentum
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DodgerBlue), PlotStyle.Dot, "LongSignal"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Dot, "ShortSignal"));
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 2)
        		return;
					
			double LongEMA = EMA(19)[0]; 
			double ShortEMA1 = EMA(8)[1];
            double ShortEMA = EMA(8)[0]; 
			
			// Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			//PriceType = PriceType.Close;
            //LongSignal.Set(Low[0]);
            //ShortSignal.Set(High[0]);
			
			// Prints the current value of a 20 period EMA using default price type 
			//Print("The current EMA value is " + ShortEMA.ToString());
			// Prints the current value of a 20 period EMA using default price type 
			
			//Print("ShortEMA " + ShortEMA.ToString());
			//Print("ShortEMA1 " + ShortEMA1.ToString());
			//Print("High[1] " + High[1].ToString());
			//Print("High[0] " + High[0].ToString());
			
			if( (High[1] < EMA(8)[1]) && (High[1] < EMA(19)[1]) ) 
			{
				if( (High[0] >= EMA(8)[0]) || (High[0] >= EMA(19)[0] ) )
				{
					ShortSignal.Set(High[0]);
					Print("We got Short alert at " + Instrument.FullName + " " + Low[0].ToString() );
					Alert("ShortAlert", NinjaTrader.Cbi.Priority.High, "Short ALERT!", "Alert4.wav", 50, Color.Black, Color.Yellow);
				}
			}
			
			if( (Low[1] > EMA(8)[1]) && (Low[1] > EMA(19)[1]) ) 
			{
				if( (Low[0] <= EMA(8)[0]) || (Low[0] <= EMA(19)[0] ) )
				{
					LongSignal.Set(Low[0]);
					Print("We got LONG alert at " + Instrument.FullName + " "+ High[0].ToString());
					Alert("LONGAlert", NinjaTrader.Cbi.Priority.High, "LONG ALERT!", "Alert4.wav", 50, Color.Black, Color.Yellow);
				}
			}
			
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongSignal
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortSignal
        {
            get { return Values[1]; }
        }

        [Description("If MACD is taken into consideration")]
        [Category("Parameters")]
        public bool UseMACD
        {
            get { return useMACD; }
            set { useMACD = value; }
        }

        [Description("If momemtum is taken into consideration")]
        [Category("Parameters")]
        public bool UseMomentum
        {
            get { return useMomentum; }
            set { useMomentum = value; }
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
        private AlertPullbacks[] cacheAlertPullbacks = null;

        private static AlertPullbacks checkAlertPullbacks = new AlertPullbacks();

        /// <summary>
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        public AlertPullbacks AlertPullbacks(bool useMACD, bool useMomentum)
        {
            return AlertPullbacks(Input, useMACD, useMomentum);
        }

        /// <summary>
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        public AlertPullbacks AlertPullbacks(Data.IDataSeries input, bool useMACD, bool useMomentum)
        {
            if (cacheAlertPullbacks != null)
                for (int idx = 0; idx < cacheAlertPullbacks.Length; idx++)
                    if (cacheAlertPullbacks[idx].UseMACD == useMACD && cacheAlertPullbacks[idx].UseMomentum == useMomentum && cacheAlertPullbacks[idx].EqualsInput(input))
                        return cacheAlertPullbacks[idx];

            lock (checkAlertPullbacks)
            {
                checkAlertPullbacks.UseMACD = useMACD;
                useMACD = checkAlertPullbacks.UseMACD;
                checkAlertPullbacks.UseMomentum = useMomentum;
                useMomentum = checkAlertPullbacks.UseMomentum;

                if (cacheAlertPullbacks != null)
                    for (int idx = 0; idx < cacheAlertPullbacks.Length; idx++)
                        if (cacheAlertPullbacks[idx].UseMACD == useMACD && cacheAlertPullbacks[idx].UseMomentum == useMomentum && cacheAlertPullbacks[idx].EqualsInput(input))
                            return cacheAlertPullbacks[idx];

                AlertPullbacks indicator = new AlertPullbacks();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.UseMACD = useMACD;
                indicator.UseMomentum = useMomentum;
                Indicators.Add(indicator);
                indicator.SetUp();

                AlertPullbacks[] tmp = new AlertPullbacks[cacheAlertPullbacks == null ? 1 : cacheAlertPullbacks.Length + 1];
                if (cacheAlertPullbacks != null)
                    cacheAlertPullbacks.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAlertPullbacks = tmp;
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
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AlertPullbacks AlertPullbacks(bool useMACD, bool useMomentum)
        {
            return _indicator.AlertPullbacks(Input, useMACD, useMomentum);
        }

        /// <summary>
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        public Indicator.AlertPullbacks AlertPullbacks(Data.IDataSeries input, bool useMACD, bool useMomentum)
        {
            return _indicator.AlertPullbacks(input, useMACD, useMomentum);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AlertPullbacks AlertPullbacks(bool useMACD, bool useMomentum)
        {
            return _indicator.AlertPullbacks(Input, useMACD, useMomentum);
        }

        /// <summary>
        /// Alert on any pullback met requirment of AS
        /// </summary>
        /// <returns></returns>
        public Indicator.AlertPullbacks AlertPullbacks(Data.IDataSeries input, bool useMACD, bool useMomentum)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AlertPullbacks(input, useMACD, useMomentum);
        }
    }
}
#endregion
