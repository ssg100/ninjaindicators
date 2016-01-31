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
    /// Collect data
    /// </summary>
    [Description("Collect data")]
    public class ZZCollectData : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
           // Plot0.Set(Close[0]);
			//string datetemp; 
			//datetemp = Time[0].ToShortDateString;
			
			if(Time[0].DayOfWeek == DayOfWeek.Monday) {
				Print(Time[0].ToString("MM/dd/yyy") + " " +"Mon" + " " + High[0] + " " + Low[0] + " " 	+ (High[0]-Low[0]));
        	
			}
			else
			{
				Print(Time[0].ToString("MM/dd/yyy") + " " + "nm" + " " + High[0] + " " + Low[0] + " " 	+ (High[0]-Low[0]));
			
			}
			
		}
		protected override void OnMarketData(MarketDataEventArgs e)
		{
    		// Print some data to the Output window
    		//if (e.MarketDataType == MarketDataType.Last) 
          	//	Print("Last=" + e.Price + " " + e.Volume);
 
    		//else if (e.MarketDataType == MarketDataType.Ask)
         	//	Print("Ask=" + e.Price + " " + e.Volume);
    		//else if (e.MarketDataType == MarketDataType.Bid)
         	//	Print("Bid=" + e.Price + " " + e.Volume);
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
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
        private ZZCollectData[] cacheZZCollectData = null;

        private static ZZCollectData checkZZCollectData = new ZZCollectData();

        /// <summary>
        /// Collect data
        /// </summary>
        /// <returns></returns>
        public ZZCollectData ZZCollectData(int myInput0)
        {
            return ZZCollectData(Input, myInput0);
        }

        /// <summary>
        /// Collect data
        /// </summary>
        /// <returns></returns>
        public ZZCollectData ZZCollectData(Data.IDataSeries input, int myInput0)
        {
            if (cacheZZCollectData != null)
                for (int idx = 0; idx < cacheZZCollectData.Length; idx++)
                    if (cacheZZCollectData[idx].MyInput0 == myInput0 && cacheZZCollectData[idx].EqualsInput(input))
                        return cacheZZCollectData[idx];

            lock (checkZZCollectData)
            {
                checkZZCollectData.MyInput0 = myInput0;
                myInput0 = checkZZCollectData.MyInput0;

                if (cacheZZCollectData != null)
                    for (int idx = 0; idx < cacheZZCollectData.Length; idx++)
                        if (cacheZZCollectData[idx].MyInput0 == myInput0 && cacheZZCollectData[idx].EqualsInput(input))
                            return cacheZZCollectData[idx];

                ZZCollectData indicator = new ZZCollectData();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MyInput0 = myInput0;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZCollectData[] tmp = new ZZCollectData[cacheZZCollectData == null ? 1 : cacheZZCollectData.Length + 1];
                if (cacheZZCollectData != null)
                    cacheZZCollectData.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZCollectData = tmp;
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
        /// Collect data
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZCollectData ZZCollectData(int myInput0)
        {
            return _indicator.ZZCollectData(Input, myInput0);
        }

        /// <summary>
        /// Collect data
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZCollectData ZZCollectData(Data.IDataSeries input, int myInput0)
        {
            return _indicator.ZZCollectData(input, myInput0);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Collect data
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZCollectData ZZCollectData(int myInput0)
        {
            return _indicator.ZZCollectData(Input, myInput0);
        }

        /// <summary>
        /// Collect data
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZCollectData ZZCollectData(Data.IDataSeries input, int myInput0)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZCollectData(input, myInput0);
        }
    }
}
#endregion
