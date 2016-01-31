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
using System.Collections;
using System.Text;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots difference between two user defined instruments.
    /// </summary>
    [Description("Plots the ratio value of the pair.")]
    public class PairsRatio : Indicator
    {
        #region Variables
           
            private string firstInstrument  = "YM 06-10";
			private string secondInstrument = "ES 06-10";
        
			private DataSeries CloseOfOne;
			private DataSeries CloseOfTwo;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "Ratio"));
			
			Add(FirstInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
   			Add(SecondInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
		
            CalculateOnBarClose	= false;
            Overlay				= false;
			DrawOnPricePanel    = false;
			DisplayInDataBox    = true; 

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBarArray[0] > 0 && CurrentBarArray[1] > 0 && CurrentBarArray[2] > 0)
			{
				if (BarsInProgress == 0) 
				{
					Ratio.Set(Closes[1][0] - Closes[2][0]);
				}
			}
		}	
        #region LabelOverride
		public override string ToString()
		{
		
			return Name + "(Ratio of " + FirstInstrument + " - " + SecondInstrument + ")";
		}
		#endregion
        
		#region Properties
        
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Ratio
		{
			get { return Values[0]; }
		}

		[Description("Symbol name of first instrument, usually the one with a currently higher price, include expiry for futures")]
        [GridCategory("Parameters")]
        public string FirstInstrument
        {
            get { return firstInstrument; }
            set { firstInstrument = value; }
        }
		
		
        [Description("Symbol name of second instrument, usually the one with a currently lower price, include expiry for futures")]
        [GridCategory("Parameters")]
        public string SecondInstrument
        {
            get { return secondInstrument; }
            set { secondInstrument = value; }
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
        private PairsRatio[] cachePairsRatio = null;

        private static PairsRatio checkPairsRatio = new PairsRatio();

        /// <summary>
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsRatio PairsRatio(string firstInstrument, string secondInstrument)
        {
            return PairsRatio(Input, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsRatio PairsRatio(Data.IDataSeries input, string firstInstrument, string secondInstrument)
        {
            if (cachePairsRatio != null)
                for (int idx = 0; idx < cachePairsRatio.Length; idx++)
                    if (cachePairsRatio[idx].FirstInstrument == firstInstrument && cachePairsRatio[idx].SecondInstrument == secondInstrument && cachePairsRatio[idx].EqualsInput(input))
                        return cachePairsRatio[idx];

            lock (checkPairsRatio)
            {
                checkPairsRatio.FirstInstrument = firstInstrument;
                firstInstrument = checkPairsRatio.FirstInstrument;
                checkPairsRatio.SecondInstrument = secondInstrument;
                secondInstrument = checkPairsRatio.SecondInstrument;

                if (cachePairsRatio != null)
                    for (int idx = 0; idx < cachePairsRatio.Length; idx++)
                        if (cachePairsRatio[idx].FirstInstrument == firstInstrument && cachePairsRatio[idx].SecondInstrument == secondInstrument && cachePairsRatio[idx].EqualsInput(input))
                            return cachePairsRatio[idx];

                PairsRatio indicator = new PairsRatio();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FirstInstrument = firstInstrument;
                indicator.SecondInstrument = secondInstrument;
                Indicators.Add(indicator);
                indicator.SetUp();

                PairsRatio[] tmp = new PairsRatio[cachePairsRatio == null ? 1 : cachePairsRatio.Length + 1];
                if (cachePairsRatio != null)
                    cachePairsRatio.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePairsRatio = tmp;
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
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsRatio PairsRatio(string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsRatio(Input, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsRatio PairsRatio(Data.IDataSeries input, string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsRatio(input, firstInstrument, secondInstrument);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsRatio PairsRatio(string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsRatio(Input, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the ratio value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsRatio PairsRatio(Data.IDataSeries input, string firstInstrument, string secondInstrument)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PairsRatio(input, firstInstrument, secondInstrument);
        }
    }
}
#endregion
