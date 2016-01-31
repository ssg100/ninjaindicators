// ==============================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
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
		[Description("Jurik DMX and T3")]
		public class Jurik_DMX_and_T3 : Indicator
		#endregion
    {
        #region Variables	// default values
            private int dmx_len = 50 ; 
            private double t3boost = 0.3; 
            private double t3len = 8; 
			// ---------------------------
			private double DMX_value = 0;
			private double T3_value = 0;
        	#endregion
		
		#region Input Parameters
			[Description("DMX length, any integer > 2")]
			[GridCategory("Parameters")]
			public int DMX_len
			{
				get { return dmx_len; }
				set { dmx_len = Math.Max(3, value); }
			}
			
			[Description("T3 boost, any decimal between 0 and 1")]
			[GridCategory("Parameters")]
			public double T3_boost
			{
				get { return t3boost; }
				set { t3boost = Math.Min(Math.Max(-0, value),1); }
			}
			
			[Description("T3 length, any decimal >= 1")]
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
				Add(new Plot(Color.Green, PlotStyle.Line, "T3"));
				Add(new Plot(Color.Orange, PlotStyle.Line, "DMX"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));
			
				Plots[0].Pen.Width = 2;
				
				Add(new Line(Color.Gray, 0, "Zero Line"));
				Lines[0].Pen.Width = 1;
	
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
		}

        protected override void OnBarUpdate()
        {
			#region Indicator formula
		
				DMX_value = JurikDMX(DMX_len).DMXValue[0];
				DMX_series.Set( CurrentBar >= 50 ? DMX_value : 0);
			
				T3_value = JurikFunction_Tillson_T3( DMX_series, T3_boost, T3_len)[0];
				T3_series.Set(T3_value);
				#endregion
			
			#region Panel Stabilizer
				panel_range_min.Set(-101);
				panel_range_max.Set(101);
				#endregion
        }

        #region Output Values

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries T3_series
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries DMX_series
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[2]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[3]; }
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
        private Jurik_DMX_and_T3[] cacheJurik_DMX_and_T3 = null;

        private static Jurik_DMX_and_T3 checkJurik_DMX_and_T3 = new Jurik_DMX_and_T3();

        /// <summary>
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_and_T3 Jurik_DMX_and_T3(int dMX_len, double t3_boost, double t3_len)
        {
            return Jurik_DMX_and_T3(Input, dMX_len, t3_boost, t3_len);
        }

        /// <summary>
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        public Jurik_DMX_and_T3 Jurik_DMX_and_T3(Data.IDataSeries input, int dMX_len, double t3_boost, double t3_len)
        {
            if (cacheJurik_DMX_and_T3 != null)
                for (int idx = 0; idx < cacheJurik_DMX_and_T3.Length; idx++)
                    if (cacheJurik_DMX_and_T3[idx].DMX_len == dMX_len && Math.Abs(cacheJurik_DMX_and_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurik_DMX_and_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurik_DMX_and_T3[idx].EqualsInput(input))
                        return cacheJurik_DMX_and_T3[idx];

            lock (checkJurik_DMX_and_T3)
            {
                checkJurik_DMX_and_T3.DMX_len = dMX_len;
                dMX_len = checkJurik_DMX_and_T3.DMX_len;
                checkJurik_DMX_and_T3.T3_boost = t3_boost;
                t3_boost = checkJurik_DMX_and_T3.T3_boost;
                checkJurik_DMX_and_T3.T3_len = t3_len;
                t3_len = checkJurik_DMX_and_T3.T3_len;

                if (cacheJurik_DMX_and_T3 != null)
                    for (int idx = 0; idx < cacheJurik_DMX_and_T3.Length; idx++)
                        if (cacheJurik_DMX_and_T3[idx].DMX_len == dMX_len && Math.Abs(cacheJurik_DMX_and_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurik_DMX_and_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurik_DMX_and_T3[idx].EqualsInput(input))
                            return cacheJurik_DMX_and_T3[idx];

                Jurik_DMX_and_T3 indicator = new Jurik_DMX_and_T3();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DMX_len = dMX_len;
                indicator.T3_boost = t3_boost;
                indicator.T3_len = t3_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_DMX_and_T3[] tmp = new Jurik_DMX_and_T3[cacheJurik_DMX_and_T3 == null ? 1 : cacheJurik_DMX_and_T3.Length + 1];
                if (cacheJurik_DMX_and_T3 != null)
                    cacheJurik_DMX_and_T3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_DMX_and_T3 = tmp;
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
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_and_T3 Jurik_DMX_and_T3(int dMX_len, double t3_boost, double t3_len)
        {
            return _indicator.Jurik_DMX_and_T3(Input, dMX_len, t3_boost, t3_len);
        }

        /// <summary>
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_and_T3 Jurik_DMX_and_T3(Data.IDataSeries input, int dMX_len, double t3_boost, double t3_len)
        {
            return _indicator.Jurik_DMX_and_T3(input, dMX_len, t3_boost, t3_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_DMX_and_T3 Jurik_DMX_and_T3(int dMX_len, double t3_boost, double t3_len)
        {
            return _indicator.Jurik_DMX_and_T3(Input, dMX_len, t3_boost, t3_len);
        }

        /// <summary>
        /// Jurik DMX and T3
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_DMX_and_T3 Jurik_DMX_and_T3(Data.IDataSeries input, int dMX_len, double t3_boost, double t3_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_DMX_and_T3(input, dMX_len, t3_boost, t3_len);
        }
    }
}
#endregion
