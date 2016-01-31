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
    [Description("Plots difference between two user defined instruments.")]
    public class Pairs : Indicator
    {
        #region Variables
       
            private int sMAPeriod = 10;
           
            private string firstInstrument  = "YM 06-10";
			private string secondInstrument = "ES 06-10";
        
			private DataSeries OpenDiff;
			private DataSeries CloseDiff;
			private DataSeries HighDiff;
			private DataSeries LowDiff;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.FromKnownColor(KnownColor.Yellow), PlotStyle.Line, "SMALine"));
			
			Add(FirstInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
   			Add(SecondInstrument, BarsPeriods[0].Id, BarsPeriods[0].Value);
		
            CalculateOnBarClose	= false;
            Overlay				= false;
			DrawOnPricePanel	= false;
			DisplayInDataBox	= true; 
			
			OpenDiff	= new DataSeries(this);
			CloseDiff 	= new DataSeries(this);
			HighDiff 	= new DataSeries(this);
			LowDiff 	= new DataSeries(this);
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
				OpenDiff.Set(Opens[1][0] - Opens[2][0]);
				CloseDiff.Set(Closes[1][0] - Closes[2][0]);
				LowDiff.Set(Lows[1][0] - Highs[2][0]);
				HighDiff.Set(Highs[1][0] - Lows[2][0]);					
	
				if (OpenDiff[0] > CloseDiff[0])
				{	
					DrawLine("DifferenceBarBody" + CurrentBar, true, 0, OpenDiff[0], 0, CloseDiff[0], Color.Red, DashStyle.Solid, 5); 
					DrawLine("DifferenceBarWickHigh" + CurrentBar, true, 0, OpenDiff[0], 0, HighDiff[0], Color.White, DashStyle.Solid, 1); 	
					DrawLine("DifferenceBarWickLow" + CurrentBar, true, 0, CloseDiff[0], 0,LowDiff[0], Color.White, DashStyle.Solid, 1); 
				}
				else if (OpenDiff[0] < CloseDiff[0])
				{	
					DrawLine("DifferenceBarBody" + CurrentBar, true, 0, OpenDiff[0], 0, CloseDiff[0], Color.Lime, DashStyle.Solid, 5); 
					DrawLine("DifferenceBarWickHigh" + CurrentBar, true, 0, CloseDiff[0], 0, HighDiff[0], Color.White, DashStyle.Solid, 1); 	
					DrawLine("DifferenceBarWickLow" + CurrentBar, true, 0, OpenDiff[0], 0, LowDiff[0], Color.White, DashStyle.Solid, 1); 
				}
			
				SMALine.Set(SMA(CloseDiff, SMAPeriod)[0]);
				
				}
			}
		}	
        #region LabelOverride
		public override string ToString()
		{
		
			return Name + "(" + FirstInstrument + " - " + SecondInstrument + ", SMA = " + SMAPeriod + ")";
		}
		#endregion
        
		#region Properties
        
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries SMALine
        {
            get { return Values[0]; }
        }

        [Description("Number of bars used for SMA calculation")]
        [GridCategory("Parameters")]
        public int SMAPeriod
        {
            get { return sMAPeriod; }
            set { sMAPeriod = Math.Max(1, value); }
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
        private Pairs[] cachePairs = null;

        private static Pairs checkPairs = new Pairs();

        /// <summary>
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        public Pairs Pairs(string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            return Pairs(Input, firstInstrument, secondInstrument, sMAPeriod);
        }

        /// <summary>
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        public Pairs Pairs(Data.IDataSeries input, string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            if (cachePairs != null)
                for (int idx = 0; idx < cachePairs.Length; idx++)
                    if (cachePairs[idx].FirstInstrument == firstInstrument && cachePairs[idx].SecondInstrument == secondInstrument && cachePairs[idx].SMAPeriod == sMAPeriod && cachePairs[idx].EqualsInput(input))
                        return cachePairs[idx];

            lock (checkPairs)
            {
                checkPairs.FirstInstrument = firstInstrument;
                firstInstrument = checkPairs.FirstInstrument;
                checkPairs.SecondInstrument = secondInstrument;
                secondInstrument = checkPairs.SecondInstrument;
                checkPairs.SMAPeriod = sMAPeriod;
                sMAPeriod = checkPairs.SMAPeriod;

                if (cachePairs != null)
                    for (int idx = 0; idx < cachePairs.Length; idx++)
                        if (cachePairs[idx].FirstInstrument == firstInstrument && cachePairs[idx].SecondInstrument == secondInstrument && cachePairs[idx].SMAPeriod == sMAPeriod && cachePairs[idx].EqualsInput(input))
                            return cachePairs[idx];

                Pairs indicator = new Pairs();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FirstInstrument = firstInstrument;
                indicator.SecondInstrument = secondInstrument;
                indicator.SMAPeriod = sMAPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                Pairs[] tmp = new Pairs[cachePairs == null ? 1 : cachePairs.Length + 1];
                if (cachePairs != null)
                    cachePairs.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePairs = tmp;
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
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Pairs Pairs(string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            return _indicator.Pairs(Input, firstInstrument, secondInstrument, sMAPeriod);
        }

        /// <summary>
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        public Indicator.Pairs Pairs(Data.IDataSeries input, string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            return _indicator.Pairs(input, firstInstrument, secondInstrument, sMAPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Pairs Pairs(string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            return _indicator.Pairs(Input, firstInstrument, secondInstrument, sMAPeriod);
        }

        /// <summary>
        /// Plots difference between two user defined instruments.
        /// </summary>
        /// <returns></returns>
        public Indicator.Pairs Pairs(Data.IDataSeries input, string firstInstrument, string secondInstrument, int sMAPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Pairs(input, firstInstrument, secondInstrument, sMAPeriod);
        }
    }
}
#endregion
