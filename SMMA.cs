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

namespace NinjaTrader.Indicator
{
    [Description("The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
    [Gui.Design.DisplayName("SMMA (Smoothed Moving Average)")]
    public class SMMA : Indicator
    {
        #region Variables
		private int		period	= 14;
		private double	smma1	= 0;
		private double	sum1	= 0;
		private double	prevsum1 = 0;
		private double	prevsmma1 = 0;
        #endregion

        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "SMMA"));
			
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= true;
        }

        protected override void OnBarUpdate()
        {			
            if(CurrentBar == Period)
			{
				sum1 = SUM(Input,Period)[0];
				smma1 = sum1/Period;
				Value.Set(smma1);
			}
			else if (CurrentBar > Period)
			{
				if (FirstTickOfBar)
				{
					prevsum1 = sum1;
					prevsmma1 = smma1;
				}
				Value.Set((prevsum1-prevsmma1+Input[0])/Period);
				sum1 = prevsum1-prevsmma1+Input[0];
				smma1 = (sum1-prevsmma1+Input[0])/Period;
			}
        }

        #region Properties
		[Description("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private SMMA[] cacheSMMA = null;

        private static SMMA checkSMMA = new SMMA();

        /// <summary>
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public SMMA SMMA(int period)
        {
            return SMMA(Input, period);
        }

        /// <summary>
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public SMMA SMMA(Data.IDataSeries input, int period)
        {
            if (cacheSMMA != null)
                for (int idx = 0; idx < cacheSMMA.Length; idx++)
                    if (cacheSMMA[idx].Period == period && cacheSMMA[idx].EqualsInput(input))
                        return cacheSMMA[idx];

            lock (checkSMMA)
            {
                checkSMMA.Period = period;
                period = checkSMMA.Period;

                if (cacheSMMA != null)
                    for (int idx = 0; idx < cacheSMMA.Length; idx++)
                        if (cacheSMMA[idx].Period == period && cacheSMMA[idx].EqualsInput(input))
                            return cacheSMMA[idx];

                SMMA indicator = new SMMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                SMMA[] tmp = new SMMA[cacheSMMA == null ? 1 : cacheSMMA.Length + 1];
                if (cacheSMMA != null)
                    cacheSMMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSMMA = tmp;
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
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMMA SMMA(int period)
        {
            return _indicator.SMMA(Input, period);
        }

        /// <summary>
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMMA SMMA(Data.IDataSeries input, int period)
        {
            return _indicator.SMMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMMA SMMA(int period)
        {
            return _indicator.SMMA(Input, period);
        }

        /// <summary>
        /// The SMMA (Smoothed Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMMA SMMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SMMA(input, period);
        }
    }
}
#endregion
