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
    /// Using NT to manage IB positions
    /// </summary>
    [Description("Using NT to manage IB positions")]
    public class ZZManageAccountPositions : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double accStopLoss = 25000; // Default setting for AccStopLoss
            private double accStopLossPct = 10; // Default setting for AccStopLossPct
            private string ignoreSymbol = @""; // Default setting for IgnoreSymbol
            private string ignoreSymbol2 = @""; // Default setting for IgnoreSymbol2
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
			double total_position_usd = 0.0;
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Close[0]);
			for (int i=0;i<=NinjaTrader.Cbi.Globals.Accounts.Count-1;i++) {
				//Print("Account: " + NinjaTrader.Cbi.Globals.Accounts[i].Name);
			
				//Print("Cashvalue: " + NinjaTrader.Cbi.Globals.Accounts[i].GetAccountValue(AccountItem.CashValue, Currency.UsDollar));
			
				if ((NinjaTrader.Cbi.Globals.Accounts[i].Name.CompareTo("U1465027")) == 0 ){
					
					for (int j=0; j <= NinjaTrader.Cbi.Globals.Accounts[i].Positions.Count-1; j++){
						Print("Position: " + NinjaTrader.Cbi.Globals.Accounts[i].Positions.Count);
						Print("Position: " + NinjaTrader.Cbi.Globals.Accounts[i].Positions[j].Instrument);
						//total_position_usd += NinjaTrader.Cbi.Globals.Accounts[i].Positions[j]  //need position value, but none available!!!!
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

        [Description("")]
        [GridCategory("Parameters")]
        public string IgnoreSymbol2
        {
            get { return ignoreSymbol2; }
            set { ignoreSymbol2 = value; }
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
        private ZZManageAccountPositions[] cacheZZManageAccountPositions = null;

        private static ZZManageAccountPositions checkZZManageAccountPositions = new ZZManageAccountPositions();

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public ZZManageAccountPositions ZZManageAccountPositions(double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            return ZZManageAccountPositions(Input, accStopLoss, accStopLossPct, ignoreSymbol, ignoreSymbol2);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public ZZManageAccountPositions ZZManageAccountPositions(Data.IDataSeries input, double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            if (cacheZZManageAccountPositions != null)
                for (int idx = 0; idx < cacheZZManageAccountPositions.Length; idx++)
                    if (Math.Abs(cacheZZManageAccountPositions[idx].AccStopLoss - accStopLoss) <= double.Epsilon && Math.Abs(cacheZZManageAccountPositions[idx].AccStopLossPct - accStopLossPct) <= double.Epsilon && cacheZZManageAccountPositions[idx].IgnoreSymbol == ignoreSymbol && cacheZZManageAccountPositions[idx].IgnoreSymbol2 == ignoreSymbol2 && cacheZZManageAccountPositions[idx].EqualsInput(input))
                        return cacheZZManageAccountPositions[idx];

            lock (checkZZManageAccountPositions)
            {
                checkZZManageAccountPositions.AccStopLoss = accStopLoss;
                accStopLoss = checkZZManageAccountPositions.AccStopLoss;
                checkZZManageAccountPositions.AccStopLossPct = accStopLossPct;
                accStopLossPct = checkZZManageAccountPositions.AccStopLossPct;
                checkZZManageAccountPositions.IgnoreSymbol = ignoreSymbol;
                ignoreSymbol = checkZZManageAccountPositions.IgnoreSymbol;
                checkZZManageAccountPositions.IgnoreSymbol2 = ignoreSymbol2;
                ignoreSymbol2 = checkZZManageAccountPositions.IgnoreSymbol2;

                if (cacheZZManageAccountPositions != null)
                    for (int idx = 0; idx < cacheZZManageAccountPositions.Length; idx++)
                        if (Math.Abs(cacheZZManageAccountPositions[idx].AccStopLoss - accStopLoss) <= double.Epsilon && Math.Abs(cacheZZManageAccountPositions[idx].AccStopLossPct - accStopLossPct) <= double.Epsilon && cacheZZManageAccountPositions[idx].IgnoreSymbol == ignoreSymbol && cacheZZManageAccountPositions[idx].IgnoreSymbol2 == ignoreSymbol2 && cacheZZManageAccountPositions[idx].EqualsInput(input))
                            return cacheZZManageAccountPositions[idx];

                ZZManageAccountPositions indicator = new ZZManageAccountPositions();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AccStopLoss = accStopLoss;
                indicator.AccStopLossPct = accStopLossPct;
                indicator.IgnoreSymbol = ignoreSymbol;
                indicator.IgnoreSymbol2 = ignoreSymbol2;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZManageAccountPositions[] tmp = new ZZManageAccountPositions[cacheZZManageAccountPositions == null ? 1 : cacheZZManageAccountPositions.Length + 1];
                if (cacheZZManageAccountPositions != null)
                    cacheZZManageAccountPositions.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZManageAccountPositions = tmp;
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
        public Indicator.ZZManageAccountPositions ZZManageAccountPositions(double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            return _indicator.ZZManageAccountPositions(Input, accStopLoss, accStopLossPct, ignoreSymbol, ignoreSymbol2);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZManageAccountPositions ZZManageAccountPositions(Data.IDataSeries input, double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            return _indicator.ZZManageAccountPositions(input, accStopLoss, accStopLossPct, ignoreSymbol, ignoreSymbol2);
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
        public Indicator.ZZManageAccountPositions ZZManageAccountPositions(double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            return _indicator.ZZManageAccountPositions(Input, accStopLoss, accStopLossPct, ignoreSymbol, ignoreSymbol2);
        }

        /// <summary>
        /// Using NT to manage IB positions
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZManageAccountPositions ZZManageAccountPositions(Data.IDataSeries input, double accStopLoss, double accStopLossPct, string ignoreSymbol, string ignoreSymbol2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZManageAccountPositions(input, accStopLoss, accStopLossPct, ignoreSymbol, ignoreSymbol2);
        }
    }
}
#endregion
