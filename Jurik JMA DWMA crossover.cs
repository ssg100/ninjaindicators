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
		[Description("JMA and DWMA plots")]
		public class Jurik_JMA_DWMA_crossover : Indicator
		#endregion
    {
        #region Variables	// default values
            private int dwma_len = 10; 		 
            private double jma_len = 7; 	 
            private double jma_phase = 0; 		 
			// ---------------------------
			private double JMAValue = 0;
			private double DWMAvalue = 0;
			private DataSeries PriceSeries;
        	#endregion
		
		#region Input Parameters
			[Description("DWMA length, any integer > 2")]
			[GridCategory("Parameters")]
			public int Dwma_len
			{
				get { return dwma_len; }
				set { dwma_len = Math.Max(3, value); }
			}
	
			[Description("JMA length, any value >= 1")]
			[GridCategory("Parameters")]
			public double Jma_len
			{
				get { return jma_len; }
				set { jma_len = Math.Max(1, value); }
			}
	
			[Description("JMA phase, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double Jma_phase
			{
				get { return jma_phase; }
				set { jma_phase = Math.Max(-100, Math.Min(100,value)); }
			}
        	#endregion
		
        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.LimeGreen, PlotStyle.Line, "JMA"));
				Add(new Plot(Color.Violet, PlotStyle.Line, "DWMA"));
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash ;
			
				CalculateOnBarClose	= false;
				Overlay				= true;
				PriceTypeSupported	= false;
				#endregion
					
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator Formula
				PriceSeries.Set( ( High[0] + Low[0] + 2*Close[0] )/4 );
				JMAValue = JurikJMA(PriceSeries, Jma_phase, Jma_len)[0];
				DWMAvalue = WMA(WMA(PriceSeries, Dwma_len), Dwma_len)[0];
				JMA_Series.Set(CurrentBar >= 50 ? JMAValue : PriceSeries[0]);
				DWMA_Series.Set(CurrentBar >= 50 ? DWMAvalue : PriceSeries[0]);
				#endregion
        }

        #region Output Values
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries JMA_Series
			{
				get { return Values[0]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries DWMA_Series
			{
				get { return Values[1]; }
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
        private Jurik_JMA_DWMA_crossover[] cacheJurik_JMA_DWMA_crossover = null;

        private static Jurik_JMA_DWMA_crossover checkJurik_JMA_DWMA_crossover = new Jurik_JMA_DWMA_crossover();

        /// <summary>
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(int dwma_len, double jma_len, double jma_phase)
        {
            return Jurik_JMA_DWMA_crossover(Input, dwma_len, jma_len, jma_phase);
        }

        /// <summary>
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase)
        {
            if (cacheJurik_JMA_DWMA_crossover != null)
                for (int idx = 0; idx < cacheJurik_JMA_DWMA_crossover.Length; idx++)
                    if (cacheJurik_JMA_DWMA_crossover[idx].Dwma_len == dwma_len && Math.Abs(cacheJurik_JMA_DWMA_crossover[idx].Jma_len - jma_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_DWMA_crossover[idx].Jma_phase - jma_phase) <= double.Epsilon && cacheJurik_JMA_DWMA_crossover[idx].EqualsInput(input))
                        return cacheJurik_JMA_DWMA_crossover[idx];

            lock (checkJurik_JMA_DWMA_crossover)
            {
                checkJurik_JMA_DWMA_crossover.Dwma_len = dwma_len;
                dwma_len = checkJurik_JMA_DWMA_crossover.Dwma_len;
                checkJurik_JMA_DWMA_crossover.Jma_len = jma_len;
                jma_len = checkJurik_JMA_DWMA_crossover.Jma_len;
                checkJurik_JMA_DWMA_crossover.Jma_phase = jma_phase;
                jma_phase = checkJurik_JMA_DWMA_crossover.Jma_phase;

                if (cacheJurik_JMA_DWMA_crossover != null)
                    for (int idx = 0; idx < cacheJurik_JMA_DWMA_crossover.Length; idx++)
                        if (cacheJurik_JMA_DWMA_crossover[idx].Dwma_len == dwma_len && Math.Abs(cacheJurik_JMA_DWMA_crossover[idx].Jma_len - jma_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_DWMA_crossover[idx].Jma_phase - jma_phase) <= double.Epsilon && cacheJurik_JMA_DWMA_crossover[idx].EqualsInput(input))
                            return cacheJurik_JMA_DWMA_crossover[idx];

                Jurik_JMA_DWMA_crossover indicator = new Jurik_JMA_DWMA_crossover();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Dwma_len = dwma_len;
                indicator.Jma_len = jma_len;
                indicator.Jma_phase = jma_phase;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_JMA_DWMA_crossover[] tmp = new Jurik_JMA_DWMA_crossover[cacheJurik_JMA_DWMA_crossover == null ? 1 : cacheJurik_JMA_DWMA_crossover.Length + 1];
                if (cacheJurik_JMA_DWMA_crossover != null)
                    cacheJurik_JMA_DWMA_crossover.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_JMA_DWMA_crossover = tmp;
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
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(int dwma_len, double jma_len, double jma_phase)
        {
            return _indicator.Jurik_JMA_DWMA_crossover(Input, dwma_len, jma_len, jma_phase);
        }

        /// <summary>
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase)
        {
            return _indicator.Jurik_JMA_DWMA_crossover(input, dwma_len, jma_len, jma_phase);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(int dwma_len, double jma_len, double jma_phase)
        {
            return _indicator.Jurik_JMA_DWMA_crossover(Input, dwma_len, jma_len, jma_phase);
        }

        /// <summary>
        /// JMA and DWMA plots
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DWMA_crossover Jurik_JMA_DWMA_crossover(Data.IDataSeries input, int dwma_len, double jma_len, double jma_phase)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_JMA_DWMA_crossover(input, dwma_len, jma_len, jma_phase);
        }
    }
}
#endregion
