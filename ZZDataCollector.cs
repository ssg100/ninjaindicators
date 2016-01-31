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
using System.IO;
#endregion

// This namespace holds all indicators and is required. Do not change it.
//
// This indicator is to collect data on ninja chart.   it will be saved to c:\datacollector.csv
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class ZZDataCollector : Indicator
    {
        #region Variables
        // Wizard generated variables
            //private int myInput0 = 1; // Default setting for MyInput0
			private string fileName = "c:\\datacollector.csv";
			private string data;
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
            //Plot0.Set(Close[0]);
			//Print(Time[0].ToShortDateString() + "," + Open[0] + "," + High[0] + "," + Low[0] + "," + Close[0]);
			data = Time[0].ToShortDateString() + "," + Open[0] + "," + High[0] + "," + Low[0] + "," + Close[0];
			
			using (FileStream fs = new FileStream(fileName,FileMode.Append, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs)) 
				{
    				sw.WriteLine(data);
 				}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Filename")]
        [GridCategory("Parameters")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
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
        private ZZDataCollector[] cacheZZDataCollector = null;

        private static ZZDataCollector checkZZDataCollector = new ZZDataCollector();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ZZDataCollector ZZDataCollector(string fileName)
        {
            return ZZDataCollector(Input, fileName);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public ZZDataCollector ZZDataCollector(Data.IDataSeries input, string fileName)
        {
            if (cacheZZDataCollector != null)
                for (int idx = 0; idx < cacheZZDataCollector.Length; idx++)
                    if (cacheZZDataCollector[idx].FileName == fileName && cacheZZDataCollector[idx].EqualsInput(input))
                        return cacheZZDataCollector[idx];

            lock (checkZZDataCollector)
            {
                checkZZDataCollector.FileName = fileName;
                fileName = checkZZDataCollector.FileName;

                if (cacheZZDataCollector != null)
                    for (int idx = 0; idx < cacheZZDataCollector.Length; idx++)
                        if (cacheZZDataCollector[idx].FileName == fileName && cacheZZDataCollector[idx].EqualsInput(input))
                            return cacheZZDataCollector[idx];

                ZZDataCollector indicator = new ZZDataCollector();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FileName = fileName;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZDataCollector[] tmp = new ZZDataCollector[cacheZZDataCollector == null ? 1 : cacheZZDataCollector.Length + 1];
                if (cacheZZDataCollector != null)
                    cacheZZDataCollector.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZDataCollector = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZDataCollector ZZDataCollector(string fileName)
        {
            return _indicator.ZZDataCollector(Input, fileName);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZDataCollector ZZDataCollector(Data.IDataSeries input, string fileName)
        {
            return _indicator.ZZDataCollector(input, fileName);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZDataCollector ZZDataCollector(string fileName)
        {
            return _indicator.ZZDataCollector(Input, fileName);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZDataCollector ZZDataCollector(Data.IDataSeries input, string fileName)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZDataCollector(input, fileName);
        }
    }
}
#endregion
