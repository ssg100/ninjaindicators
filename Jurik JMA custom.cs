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
		[Description("customizable JMA")]
		public class Jurik_JMA_custom : Indicator
		#endregion
    {
        #region Variables	// default values
            private int    j_lag = 0;
            private double j_len = 7; 
            private double j_phase = 50; 
			// --------------------------------
			private double JMAvalue = 0;
			private DataSeries PriceSeries;
        	#endregion
		
        #region Input Parameters		
			[Description("JMA lag, any integer >= 0")]
			[GridCategory("Parameters")]
			public int J_lag
			{
				get { return j_lag; }
				set { j_lag = Math.Max(0, value); }
			}
			
			[Description("JMA length, any value >= 1")]
			[GridCategory("Parameters")]
			public double J_len
			{
				get { return j_len; }
				set { j_len = Math.Max(1, value); }
			}
	
			[Description("JMA phase, any integer between -100 and +100")]
			[GridCategory("Parameters")]
			public double J_phase
			{
				get { return j_phase; }
				set { j_phase = Math.Min(Math.Max(-100.000, value),100); }
			}
        	#endregion		

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.MediumTurquoise, PlotStyle.Line, "JMA"));
				Plots[0].Pen.Width = 2;
			
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
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3  );
				JMAvalue = ( CurrentBar >= J_lag ?  JurikJMA( PriceSeries, J_phase, J_len)[J_lag] : PriceSeries[0]) ;
				JMA_Series.Set( JMAvalue );
				#endregion
        }

        #region Output Values 
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries JMA_Series
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
        private Jurik_JMA_custom[] cacheJurik_JMA_custom = null;

        private static Jurik_JMA_custom checkJurik_JMA_custom = new Jurik_JMA_custom();

        /// <summary>
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_custom Jurik_JMA_custom(int j_lag, double j_len, double j_phase)
        {
            return Jurik_JMA_custom(Input, j_lag, j_len, j_phase);
        }

        /// <summary>
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_custom Jurik_JMA_custom(Data.IDataSeries input, int j_lag, double j_len, double j_phase)
        {
            if (cacheJurik_JMA_custom != null)
                for (int idx = 0; idx < cacheJurik_JMA_custom.Length; idx++)
                    if (cacheJurik_JMA_custom[idx].J_lag == j_lag && Math.Abs(cacheJurik_JMA_custom[idx].J_len - j_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_custom[idx].J_phase - j_phase) <= double.Epsilon && cacheJurik_JMA_custom[idx].EqualsInput(input))
                        return cacheJurik_JMA_custom[idx];

            lock (checkJurik_JMA_custom)
            {
                checkJurik_JMA_custom.J_lag = j_lag;
                j_lag = checkJurik_JMA_custom.J_lag;
                checkJurik_JMA_custom.J_len = j_len;
                j_len = checkJurik_JMA_custom.J_len;
                checkJurik_JMA_custom.J_phase = j_phase;
                j_phase = checkJurik_JMA_custom.J_phase;

                if (cacheJurik_JMA_custom != null)
                    for (int idx = 0; idx < cacheJurik_JMA_custom.Length; idx++)
                        if (cacheJurik_JMA_custom[idx].J_lag == j_lag && Math.Abs(cacheJurik_JMA_custom[idx].J_len - j_len) <= double.Epsilon && Math.Abs(cacheJurik_JMA_custom[idx].J_phase - j_phase) <= double.Epsilon && cacheJurik_JMA_custom[idx].EqualsInput(input))
                            return cacheJurik_JMA_custom[idx];

                Jurik_JMA_custom indicator = new Jurik_JMA_custom();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.J_lag = j_lag;
                indicator.J_len = j_len;
                indicator.J_phase = j_phase;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_JMA_custom[] tmp = new Jurik_JMA_custom[cacheJurik_JMA_custom == null ? 1 : cacheJurik_JMA_custom.Length + 1];
                if (cacheJurik_JMA_custom != null)
                    cacheJurik_JMA_custom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_JMA_custom = tmp;
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
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_custom Jurik_JMA_custom(int j_lag, double j_len, double j_phase)
        {
            return _indicator.Jurik_JMA_custom(Input, j_lag, j_len, j_phase);
        }

        /// <summary>
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_custom Jurik_JMA_custom(Data.IDataSeries input, int j_lag, double j_len, double j_phase)
        {
            return _indicator.Jurik_JMA_custom(input, j_lag, j_len, j_phase);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_custom Jurik_JMA_custom(int j_lag, double j_len, double j_phase)
        {
            return _indicator.Jurik_JMA_custom(Input, j_lag, j_len, j_phase);
        }

        /// <summary>
        /// customizable JMA
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_custom Jurik_JMA_custom(Data.IDataSeries input, int j_lag, double j_len, double j_phase)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_JMA_custom(input, j_lag, j_len, j_phase);
        }
    }
}
#endregion
