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
    [Description("Plots the RSI value of the pair.")]
    public class PairsRSI : Indicator
    {
        #region Variables
       
            private int rSIPeriod = 2;
           
            private string firstInstrument  = "YM 06-10";
			private string secondInstrument = "ES 06-10";
        
			private DataSeries CloseDiff;
		

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Red, "RSIUpper"));
			Add(new Plot(Color.Cyan, "RSIMiddle"));
			Add(new Plot(Color.Lime, "RSILower"));
			
			
			Add(new Line(System.Drawing.Color.DarkViolet, 5, "Lower"));
			Add(new Line(System.Drawing.Color.YellowGreen, 95, "Upper"));
			
			
			Add(FirstInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
   			Add(SecondInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
		
            CalculateOnBarClose	= false;
            Overlay				= false;
			DrawOnPricePanel    = false;
			DisplayInDataBox    = true; 
			

			CloseDiff = new DataSeries(this);
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
					CloseDiff.Set(Closes[1][0] - Closes[2][0]);
					
					Plots[0].Min = Lines[1].Value; 
					Plots[1].Max = Lines[1].Value;
					Plots[1].Min = Lines[0].Value;
					Plots[2].Max = Lines[0].Value; 	
						
					RSIUpper.Set(RSI(CloseDiff, RSIPeriod, 0)[0]);
					RSIMiddle.Set(RSI(CloseDiff, RSIPeriod, 0)[0]);
					RSILower.Set(RSI(CloseDiff, RSIPeriod, 0)[0]);
				}
			}
		}	
        #region LabelOverride
		public override string ToString()
		{
		
			return Name + "(RSI of " + FirstInstrument + " - " + SecondInstrument + " , " + RSIPeriod + ")";
		}
		#endregion
        
		#region Properties
        
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSIUpper
		{
			get { return Values[0]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSIMiddle
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSILower
		{
			get { return Values[2]; }
		}

        [Description("Number of bars used for RSI calculation")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
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
        private PairsRSI[] cachePairsRSI = null;

        private static PairsRSI checkPairsRSI = new PairsRSI();

        /// <summary>
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsRSI PairsRSI(string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            return PairsRSI(Input, firstInstrument, rSIPeriod, secondInstrument);
        }

        /// <summary>
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsRSI PairsRSI(Data.IDataSeries input, string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            if (cachePairsRSI != null)
                for (int idx = 0; idx < cachePairsRSI.Length; idx++)
                    if (cachePairsRSI[idx].FirstInstrument == firstInstrument && cachePairsRSI[idx].RSIPeriod == rSIPeriod && cachePairsRSI[idx].SecondInstrument == secondInstrument && cachePairsRSI[idx].EqualsInput(input))
                        return cachePairsRSI[idx];

            lock (checkPairsRSI)
            {
                checkPairsRSI.FirstInstrument = firstInstrument;
                firstInstrument = checkPairsRSI.FirstInstrument;
                checkPairsRSI.RSIPeriod = rSIPeriod;
                rSIPeriod = checkPairsRSI.RSIPeriod;
                checkPairsRSI.SecondInstrument = secondInstrument;
                secondInstrument = checkPairsRSI.SecondInstrument;

                if (cachePairsRSI != null)
                    for (int idx = 0; idx < cachePairsRSI.Length; idx++)
                        if (cachePairsRSI[idx].FirstInstrument == firstInstrument && cachePairsRSI[idx].RSIPeriod == rSIPeriod && cachePairsRSI[idx].SecondInstrument == secondInstrument && cachePairsRSI[idx].EqualsInput(input))
                            return cachePairsRSI[idx];

                PairsRSI indicator = new PairsRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FirstInstrument = firstInstrument;
                indicator.RSIPeriod = rSIPeriod;
                indicator.SecondInstrument = secondInstrument;
                Indicators.Add(indicator);
                indicator.SetUp();

                PairsRSI[] tmp = new PairsRSI[cachePairsRSI == null ? 1 : cachePairsRSI.Length + 1];
                if (cachePairsRSI != null)
                    cachePairsRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePairsRSI = tmp;
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
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsRSI PairsRSI(string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            return _indicator.PairsRSI(Input, firstInstrument, rSIPeriod, secondInstrument);
        }

        /// <summary>
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsRSI PairsRSI(Data.IDataSeries input, string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            return _indicator.PairsRSI(input, firstInstrument, rSIPeriod, secondInstrument);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsRSI PairsRSI(string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            return _indicator.PairsRSI(Input, firstInstrument, rSIPeriod, secondInstrument);
        }

        /// <summary>
        /// Plots the RSI value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsRSI PairsRSI(Data.IDataSeries input, string firstInstrument, int rSIPeriod, string secondInstrument)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PairsRSI(input, firstInstrument, rSIPeriod, secondInstrument);
        }
    }
}
#endregion
