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
    [Description("Plots the correlation value of the pair.")]
    public class PairsCorrelation : Indicator
    {
        #region Variables
       
            private int correlPeriod = 10;
           
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
			Add(new Plot(Color.Red, "Correlation"));
			
			Add(FirstInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
   			Add(SecondInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
		
            CalculateOnBarClose	= false;
            Overlay				= false;
			DrawOnPricePanel	= false;
			DisplayInDataBox 	= true; 
			
			CloseOfOne = new DataSeries(this);
			CloseOfTwo = new DataSeries(this);
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
					CloseOfOne.Set(Closes[1][0]);
					CloseOfTwo.Set(Closes[2][0]);
					
					double SumOfOne = 0;
					int x = 0; 
					while (x < CorrelPeriod) 
					{ 
						SumOfOne = SumOfOne + CloseOfOne[x];
						x = x + 1; 
					}
					
					double SumOfTwo = 0;
					int y = 0; 
					while (y < CorrelPeriod) 
					{ 
						SumOfTwo = SumOfTwo + CloseOfTwo[y];
						y = y + 1; 
					}					
				
					double SumOfOneSq = 0;
					int a = 0; 
					while (a < CorrelPeriod) 
					{ 
						SumOfOneSq = SumOfOneSq + (CloseOfOne[a] * CloseOfOne[a]);
						a = a + 1; 
					}
					
					double SumOfTwoSq = 0;
					int b = 0; 
					while (b < CorrelPeriod) 
					{ 
						SumOfTwoSq = SumOfTwoSq + (CloseOfTwo[b] * CloseOfTwo[b]);
						b = b + 1; 
					}
					
					double Product = 0;
					int z = 0; 
					while (z < CorrelPeriod) 
					{ 
						Product = Product + (CloseOfOne[z] * CloseOfTwo[z]);
						z = z + 1; 
					}					
					
					Correlation.Set(((CorrelPeriod * Product) - (SumOfOne * SumOfTwo)) / Math.Sqrt((((CorrelPeriod * SumOfOneSq) - (SumOfOne * SumOfOne)) * ((CorrelPeriod * SumOfTwoSq) - (SumOfTwo * SumOfTwo)))));
				}
			}
		}	
        #region LabelOverride
		public override string ToString()
		{
		
			return Name + "(Correlation of " + FirstInstrument + " - " + SecondInstrument + " , " + CorrelPeriod + ")";
		}
		#endregion
        
		#region Properties
        
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Correlation
		{
			get { return Values[0]; }
		}
		

        [Description("Number of bars used for Correlation calculation")]
        [GridCategory("Parameters")]
        public int CorrelPeriod
        {
            get { return correlPeriod; }
            set { correlPeriod = Math.Max(1, value); }
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
        private PairsCorrelation[] cachePairsCorrelation = null;

        private static PairsCorrelation checkPairsCorrelation = new PairsCorrelation();

        /// <summary>
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsCorrelation PairsCorrelation(int correlPeriod, string firstInstrument, string secondInstrument)
        {
            return PairsCorrelation(Input, correlPeriod, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        public PairsCorrelation PairsCorrelation(Data.IDataSeries input, int correlPeriod, string firstInstrument, string secondInstrument)
        {
            if (cachePairsCorrelation != null)
                for (int idx = 0; idx < cachePairsCorrelation.Length; idx++)
                    if (cachePairsCorrelation[idx].CorrelPeriod == correlPeriod && cachePairsCorrelation[idx].FirstInstrument == firstInstrument && cachePairsCorrelation[idx].SecondInstrument == secondInstrument && cachePairsCorrelation[idx].EqualsInput(input))
                        return cachePairsCorrelation[idx];

            lock (checkPairsCorrelation)
            {
                checkPairsCorrelation.CorrelPeriod = correlPeriod;
                correlPeriod = checkPairsCorrelation.CorrelPeriod;
                checkPairsCorrelation.FirstInstrument = firstInstrument;
                firstInstrument = checkPairsCorrelation.FirstInstrument;
                checkPairsCorrelation.SecondInstrument = secondInstrument;
                secondInstrument = checkPairsCorrelation.SecondInstrument;

                if (cachePairsCorrelation != null)
                    for (int idx = 0; idx < cachePairsCorrelation.Length; idx++)
                        if (cachePairsCorrelation[idx].CorrelPeriod == correlPeriod && cachePairsCorrelation[idx].FirstInstrument == firstInstrument && cachePairsCorrelation[idx].SecondInstrument == secondInstrument && cachePairsCorrelation[idx].EqualsInput(input))
                            return cachePairsCorrelation[idx];

                PairsCorrelation indicator = new PairsCorrelation();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CorrelPeriod = correlPeriod;
                indicator.FirstInstrument = firstInstrument;
                indicator.SecondInstrument = secondInstrument;
                Indicators.Add(indicator);
                indicator.SetUp();

                PairsCorrelation[] tmp = new PairsCorrelation[cachePairsCorrelation == null ? 1 : cachePairsCorrelation.Length + 1];
                if (cachePairsCorrelation != null)
                    cachePairsCorrelation.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePairsCorrelation = tmp;
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
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsCorrelation PairsCorrelation(int correlPeriod, string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsCorrelation(Input, correlPeriod, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsCorrelation PairsCorrelation(Data.IDataSeries input, int correlPeriod, string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsCorrelation(input, correlPeriod, firstInstrument, secondInstrument);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PairsCorrelation PairsCorrelation(int correlPeriod, string firstInstrument, string secondInstrument)
        {
            return _indicator.PairsCorrelation(Input, correlPeriod, firstInstrument, secondInstrument);
        }

        /// <summary>
        /// Plots the correlation value of the pair.
        /// </summary>
        /// <returns></returns>
        public Indicator.PairsCorrelation PairsCorrelation(Data.IDataSeries input, int correlPeriod, string firstInstrument, string secondInstrument)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PairsCorrelation(input, correlPeriod, firstInstrument, secondInstrument);
        }
    }
}
#endregion
