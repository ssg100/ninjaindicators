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
    /// Using ROC of Kevin_in GA method
    /// </summary>
    [Description("Using ROC of Kevin_in GA method")]
    public class ZZBuyTheDipROC : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int rOCslow = 80; // Default setting for ROCslow
            private int rOCfast = 7; // Default setting for ROCfast
            private double rOCslowThres = 0.5; // Default setting for ROCslowThres
            private double rOCfastThres = -0.5; // Default setting for ROCfastThres
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.TriangleUp, "Buy"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Crimson), PlotStyle.TriangleDown, "Sell"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            bool condition1;
			bool condition2;
			bool condition3;
			
			condition1 = Close[0] > SMA(200)[0];
			
			condition2 =  CrossBelow(ROC(rOCfast),rOCfastThres,1);
			condition3 = ROC(rOCslow)[0] > rOCslowThres;
			
			if(condition1 && condition2 && condition3)
				Buy.Set(10);
            
			//Sell.Set(High[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Buy
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Sell
        {
            get { return Values[1]; }
        }

        [Description("roc length")]
        [GridCategory("Parameters")]
        public int ROCslow
        {
            get { return rOCslow; }
            set { rOCslow = Math.Max(1, value); }
        }

        [Description("roc lenth fast")]
        [GridCategory("Parameters")]
        public int ROCfast
        {
            get { return rOCfast; }
            set { rOCfast = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ROCslowThres
        {
            get { return rOCslowThres; }
            set { rOCslowThres = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ROCfastThres
        {
            get { return rOCfastThres; }
            set { rOCfastThres =  value; }
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
        private ZZBuyTheDipROC[] cacheZZBuyTheDipROC = null;

        private static ZZBuyTheDipROC checkZZBuyTheDipROC = new ZZBuyTheDipROC();

        /// <summary>
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        public ZZBuyTheDipROC ZZBuyTheDipROC(int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            return ZZBuyTheDipROC(Input, rOCfast, rOCfastThres, rOCslow, rOCslowThres);
        }

        /// <summary>
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        public ZZBuyTheDipROC ZZBuyTheDipROC(Data.IDataSeries input, int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            if (cacheZZBuyTheDipROC != null)
                for (int idx = 0; idx < cacheZZBuyTheDipROC.Length; idx++)
                    if (cacheZZBuyTheDipROC[idx].ROCfast == rOCfast && Math.Abs(cacheZZBuyTheDipROC[idx].ROCfastThres - rOCfastThres) <= double.Epsilon && cacheZZBuyTheDipROC[idx].ROCslow == rOCslow && Math.Abs(cacheZZBuyTheDipROC[idx].ROCslowThres - rOCslowThres) <= double.Epsilon && cacheZZBuyTheDipROC[idx].EqualsInput(input))
                        return cacheZZBuyTheDipROC[idx];

            lock (checkZZBuyTheDipROC)
            {
                checkZZBuyTheDipROC.ROCfast = rOCfast;
                rOCfast = checkZZBuyTheDipROC.ROCfast;
                checkZZBuyTheDipROC.ROCfastThres = rOCfastThres;
                rOCfastThres = checkZZBuyTheDipROC.ROCfastThres;
                checkZZBuyTheDipROC.ROCslow = rOCslow;
                rOCslow = checkZZBuyTheDipROC.ROCslow;
                checkZZBuyTheDipROC.ROCslowThres = rOCslowThres;
                rOCslowThres = checkZZBuyTheDipROC.ROCslowThres;

                if (cacheZZBuyTheDipROC != null)
                    for (int idx = 0; idx < cacheZZBuyTheDipROC.Length; idx++)
                        if (cacheZZBuyTheDipROC[idx].ROCfast == rOCfast && Math.Abs(cacheZZBuyTheDipROC[idx].ROCfastThres - rOCfastThres) <= double.Epsilon && cacheZZBuyTheDipROC[idx].ROCslow == rOCslow && Math.Abs(cacheZZBuyTheDipROC[idx].ROCslowThres - rOCslowThres) <= double.Epsilon && cacheZZBuyTheDipROC[idx].EqualsInput(input))
                            return cacheZZBuyTheDipROC[idx];

                ZZBuyTheDipROC indicator = new ZZBuyTheDipROC();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ROCfast = rOCfast;
                indicator.ROCfastThres = rOCfastThres;
                indicator.ROCslow = rOCslow;
                indicator.ROCslowThres = rOCslowThres;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZBuyTheDipROC[] tmp = new ZZBuyTheDipROC[cacheZZBuyTheDipROC == null ? 1 : cacheZZBuyTheDipROC.Length + 1];
                if (cacheZZBuyTheDipROC != null)
                    cacheZZBuyTheDipROC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZBuyTheDipROC = tmp;
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
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBuyTheDipROC ZZBuyTheDipROC(int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            return _indicator.ZZBuyTheDipROC(Input, rOCfast, rOCfastThres, rOCslow, rOCslowThres);
        }

        /// <summary>
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBuyTheDipROC ZZBuyTheDipROC(Data.IDataSeries input, int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            return _indicator.ZZBuyTheDipROC(input, rOCfast, rOCfastThres, rOCslow, rOCslowThres);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZBuyTheDipROC ZZBuyTheDipROC(int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            return _indicator.ZZBuyTheDipROC(Input, rOCfast, rOCfastThres, rOCslow, rOCslowThres);
        }

        /// <summary>
        /// Using ROC of Kevin_in GA method
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZBuyTheDipROC ZZBuyTheDipROC(Data.IDataSeries input, int rOCfast, double rOCfastThres, int rOCslow, double rOCslowThres)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZBuyTheDipROC(input, rOCfast, rOCfastThres, rOCslow, rOCslowThres);
        }
    }
}
#endregion
