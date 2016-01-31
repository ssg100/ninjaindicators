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
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Using NT to manage IB positions
    /// </summary>
    [Description("Using NT to manage IB positions")]
    public class ZZEquityCurveIB : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double accStopLoss = 25000; // Default setting for AccStopLoss
            private double accStopLossPct = 10; // Default setting for AccStopLossPct
            private string ignoreSymbol = @""; // Default setting for IgnoreSymbol
            private string ignoreSymbol2 = @""; // Default setting for IgnoreSymbol2
			private string accountName = @"U1465027";
			private string fileName = "c:\\IBequitycurve.csv";
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Black, 3), PlotStyle.Line, "Plot0"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double data;
			string datastr;
			
			if (Historical) 
				return;
			
			for (int i=0; i<=NinjaTrader.Cbi.Globals.Accounts.Count-1; i++) 
			{
				Print("Account: " + NinjaTrader.Cbi.Globals.Accounts[i].Name);
				Print("Netliq: " + NinjaTrader.Cbi.Globals.Accounts[i].GetAccountValue(AccountItem.NetLiquidation, Currency.UsDollar));
				if ((NinjaTrader.Cbi.Globals.Accounts[i].Name.CompareTo(accountName)) == 0 )
				{
					//data = Convert.ToDouble(NinjaTrader.Cbi.Globals.Accounts[i].GetAccountValue(AccountItem.NetLiquidation, Currency.UsDollar));
					Plot0.Set(NinjaTrader.Cbi.Globals.Accounts[i].GetAccountValue(AccountItem.NetLiquidation, Currency.UsDollar).Value);
					
					datastr = Convert.ToString(Time[0]) + "," + Convert.ToString((NinjaTrader.Cbi.Globals.Accounts[i].GetAccountValue(AccountItem.NetLiquidation, Currency.UsDollar).Value));
					
					Print(datastr);
					
					// Save it to file
					using (FileStream fs = new FileStream(fileName,FileMode.Append, FileAccess.Write))
						using (StreamWriter sw = new StreamWriter(fs)) 
						{
    						sw.WriteLine(datastr);
 						}
				}
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Portfolio Stop Loss in Dollar")]
        [GridCategory("Parameters")]
        public double AccStopLoss
        {
            get { return accStopLoss; }
            set { accStopLoss = Math.Max(1, value); }
        }

        [Description("in Percent")]
        [GridCategory("Parameters")]
        public double AccStopLossPct
        {
            get { return accStopLossPct; }
            set { accStopLossPct = Math.Max(1, value); }
        }

        [Description("Symbol to ignore from auto closing")]
        [GridCategory("Parameters")]
        public string IgnoreSymbol
        {
            get { return ignoreSymbol; }
            set { ignoreSymbol = value; }
        }

		[Description("Account Name or number")]
        [GridCategory("Parameters")]
        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }

        [Description("")]
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
        private ZZEquityCurveIB[] cacheZZEquityCurveIB = null;

        private static ZZEquityCurveIB checkZZEquityCurveIB = new ZZEquityCurveIB();

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public ZZEquityCurveIB ZZEquityCurveIB(string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            return ZZEquityCurveIB(Input, accountName, accStopLoss, accStopLossPct, fileName, ignoreSymbol);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public ZZEquityCurveIB ZZEquityCurveIB(Data.IDataSeries input, string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            if (cacheZZEquityCurveIB != null)
                for (int idx = 0; idx < cacheZZEquityCurveIB.Length; idx++)
                    if (cacheZZEquityCurveIB[idx].AccountName == accountName && Math.Abs(cacheZZEquityCurveIB[idx].AccStopLoss - accStopLoss) <= double.Epsilon && Math.Abs(cacheZZEquityCurveIB[idx].AccStopLossPct - accStopLossPct) <= double.Epsilon && cacheZZEquityCurveIB[idx].FileName == fileName && cacheZZEquityCurveIB[idx].IgnoreSymbol == ignoreSymbol && cacheZZEquityCurveIB[idx].EqualsInput(input))
                        return cacheZZEquityCurveIB[idx];

            lock (checkZZEquityCurveIB)
            {
                checkZZEquityCurveIB.AccountName = accountName;
                accountName = checkZZEquityCurveIB.AccountName;
                checkZZEquityCurveIB.AccStopLoss = accStopLoss;
                accStopLoss = checkZZEquityCurveIB.AccStopLoss;
                checkZZEquityCurveIB.AccStopLossPct = accStopLossPct;
                accStopLossPct = checkZZEquityCurveIB.AccStopLossPct;
                checkZZEquityCurveIB.FileName = fileName;
                fileName = checkZZEquityCurveIB.FileName;
                checkZZEquityCurveIB.IgnoreSymbol = ignoreSymbol;
                ignoreSymbol = checkZZEquityCurveIB.IgnoreSymbol;

                if (cacheZZEquityCurveIB != null)
                    for (int idx = 0; idx < cacheZZEquityCurveIB.Length; idx++)
                        if (cacheZZEquityCurveIB[idx].AccountName == accountName && Math.Abs(cacheZZEquityCurveIB[idx].AccStopLoss - accStopLoss) <= double.Epsilon && Math.Abs(cacheZZEquityCurveIB[idx].AccStopLossPct - accStopLossPct) <= double.Epsilon && cacheZZEquityCurveIB[idx].FileName == fileName && cacheZZEquityCurveIB[idx].IgnoreSymbol == ignoreSymbol && cacheZZEquityCurveIB[idx].EqualsInput(input))
                            return cacheZZEquityCurveIB[idx];

                ZZEquityCurveIB indicator = new ZZEquityCurveIB();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AccountName = accountName;
                indicator.AccStopLoss = accStopLoss;
                indicator.AccStopLossPct = accStopLossPct;
                indicator.FileName = fileName;
                indicator.IgnoreSymbol = ignoreSymbol;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZEquityCurveIB[] tmp = new ZZEquityCurveIB[cacheZZEquityCurveIB == null ? 1 : cacheZZEquityCurveIB.Length + 1];
                if (cacheZZEquityCurveIB != null)
                    cacheZZEquityCurveIB.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZEquityCurveIB = tmp;
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
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZEquityCurveIB ZZEquityCurveIB(string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            return _indicator.ZZEquityCurveIB(Input, accountName, accStopLoss, accStopLossPct, fileName, ignoreSymbol);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZEquityCurveIB ZZEquityCurveIB(Data.IDataSeries input, string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            return _indicator.ZZEquityCurveIB(input, accountName, accStopLoss, accStopLossPct, fileName, ignoreSymbol);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZEquityCurveIB ZZEquityCurveIB(string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            return _indicator.ZZEquityCurveIB(Input, accountName, accStopLoss, accStopLossPct, fileName, ignoreSymbol);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZEquityCurveIB ZZEquityCurveIB(Data.IDataSeries input, string accountName, double accStopLoss, double accStopLossPct, string fileName, string ignoreSymbol)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZEquityCurveIB(input, accountName, accStopLoss, accStopLossPct, fileName, ignoreSymbol);
        }
    }
}
#endregion
