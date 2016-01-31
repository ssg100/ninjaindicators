// ==============================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
//
// Plots on price Tim Tillson's T3 moving average
// ==============================================

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
	#region Header
		[Description("Tillsons T3 moving average")]
		public class Jurik_Tillson_T3 : Indicator
		#endregion
    {
        #region Variables	// default values
            private double t3len = 10; 
            private double t3boost = 0.5; 
			// --------------------------------
			private double T3value = 0;
			private DataSeries PriceSeries;
        	#endregion
		
        #region Input Parameters
			[Description("T3 boost, any value between 0 and 1")]
			[GridCategory("Parameters")]
			public double T3_boost
			{
				get { return t3boost; }
				set { t3boost = Math.Min(Math.Max(0, value),1); }
			}
			
			[Description("T3 length, any value >= 1")]
			[GridCategory("Parameters")]
			public double T3_len
			{
				get { return t3len; }
				set { t3len = Math.Max(1, value); }
			}
        	#endregion		

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.Crimson, PlotStyle.Line, "JMA"));
				Plots[0].Pen.Width = 2;
			
	        	CalculateOnBarClose	= false;
	         	Overlay = true;
				PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {		
			#region Indicator Formula
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3  );
				T3value = JurikFunction_Tillson_T3( PriceSeries, T3_boost, T3_len)[0];
				T3_Series.Set( T3value );
				#endregion
        }

        #region Output Values 
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries T3_Series
			{
				get { return Values[0]; }
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
        private Jurik_Tillson_T3[] cacheJurik_Tillson_T3 = null;

        private static Jurik_Tillson_T3 checkJurik_Tillson_T3 = new Jurik_Tillson_T3();

        /// <summary>
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        public Jurik_Tillson_T3 Jurik_Tillson_T3(double t3_boost, double t3_len)
        {
            return Jurik_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        public Jurik_Tillson_T3 Jurik_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            if (cacheJurik_Tillson_T3 != null)
                for (int idx = 0; idx < cacheJurik_Tillson_T3.Length; idx++)
                    if (Math.Abs(cacheJurik_Tillson_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurik_Tillson_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurik_Tillson_T3[idx].EqualsInput(input))
                        return cacheJurik_Tillson_T3[idx];

            lock (checkJurik_Tillson_T3)
            {
                checkJurik_Tillson_T3.T3_boost = t3_boost;
                t3_boost = checkJurik_Tillson_T3.T3_boost;
                checkJurik_Tillson_T3.T3_len = t3_len;
                t3_len = checkJurik_Tillson_T3.T3_len;

                if (cacheJurik_Tillson_T3 != null)
                    for (int idx = 0; idx < cacheJurik_Tillson_T3.Length; idx++)
                        if (Math.Abs(cacheJurik_Tillson_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurik_Tillson_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurik_Tillson_T3[idx].EqualsInput(input))
                            return cacheJurik_Tillson_T3[idx];

                Jurik_Tillson_T3 indicator = new Jurik_Tillson_T3();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.T3_boost = t3_boost;
                indicator.T3_len = t3_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_Tillson_T3[] tmp = new Jurik_Tillson_T3[cacheJurik_Tillson_T3 == null ? 1 : cacheJurik_Tillson_T3.Length + 1];
                if (cacheJurik_Tillson_T3 != null)
                    cacheJurik_Tillson_T3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_Tillson_T3 = tmp;
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
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_Tillson_T3 Jurik_Tillson_T3(double t3_boost, double t3_len)
        {
            return _indicator.Jurik_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_Tillson_T3 Jurik_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            return _indicator.Jurik_Tillson_T3(input, t3_boost, t3_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_Tillson_T3 Jurik_Tillson_T3(double t3_boost, double t3_len)
        {
            return _indicator.Jurik_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// Tillsons T3 moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_Tillson_T3 Jurik_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_Tillson_T3(input, t3_boost, t3_len);
        }
    }
}
#endregion
