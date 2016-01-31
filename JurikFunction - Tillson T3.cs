// ======================================================
// NinjaTrader module by Jurik Research Software
// © 2010 Jurik Research   ;   www.jurikres.com
// ======================================================
//
// FUNCTION -- Tim Tillson's T3 moving average
//
// ******* THIS IS A FUNCTION, NOT AN INDICATOR *******
//
// ======================================================

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{
	#region Header
		[Description("NOT AN INDICATOR -- DO NOT USE TO PLOT")]
		public class JurikFunction_Tillson_T3 : Indicator
		#endregion
    {
        #region Variables	// default values
            private double t3len = 10;  
            private double t3boost = 0.5;  		
		    // ----------------------------------
			private double b1 = 0;
			private double b2 = 0;
			private double b3 = 0;
			private double c1 = 0;
			private double c2 = 0;
			private double c3 = 0;
			private double c4 = 0;
			private double f1 = 0;
			private double f2 = 0;
		
			private DataSeries e1 ;
			private DataSeries e2 ;
			private DataSeries e3 ;
			private DataSeries e4 ;
			private DataSeries e5 ;
			private DataSeries e6 ;
			#endregion
		
		#region Input Parameters
			[Description("T3 length, any decimal >= 1")]
			[GridCategory("Parameters")]
			public double T3_len
			{
				get { return t3len; }
				set { t3len = Math.Max(1, value); }
			}
			
			[Description("T3 boost, any decimal between 0 and 1")]
			[GridCategory("Parameters")]
			public double T3_boost
			{
				get { return t3boost; }
				set { t3boost = Math.Max(0, Math.Min(1, value)); }
			}
			#endregion
		
        protected override void Initialize()
        {	
			#region Series Initialization
				e1 	= new DataSeries(this);		
				e2 	= new DataSeries(this);		
				e3 	= new DataSeries(this);		
				e4 	= new DataSeries(this);		
				e5 	= new DataSeries(this);		
				e6 	= new DataSeries(this);		
				#endregion
			
			#region Constants
				b1 = T3_boost ;
				b2 = b1 * b1;
				b3 = b1 * b1 * b1;
				c1 = -b3;
				c2 = 3 * b2 + 3 * b3;
				c3 = -6 * b2 - 3 * b1 - 3 * b3;
				c4 = 1 + 3 * b1 + b3 + 3 * b2;
				f1 = 3 / (3 + T3_len) ;
				f2 = 1 - f1 ;
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Function Formula
				if (CurrentBar == 0)
				{
					e1.Set(Input[0]);
					e2.Set(Input[0]);
					e3.Set(Input[0]);
					e4.Set(Input[0]);
					e5.Set(Input[0]);
					e6.Set(Input[0]);
					
					Value.Set(Input[0]);
				}
				else
				{
					e1.Set( f2 * e1[1] + f1 * Input[0] ) ;
					e2.Set( f2 * e2[1] + f1 * e1[0] ) ;
					e3.Set( f2 * e3[1] + f1 * e2[0] ) ;
					e4.Set( f2 * e4[1] + f1 * e3[0] ) ;
					e5.Set( f2 * e5[1] + f1 * e4[0] ) ;
					e6.Set( f2 * e6[1] + f1 * e5[0] ) ;
					
					Value.Set( c1 * e6[0] + c2 * e5[0] + c3 * e4[0] + c4 * e3[0] ) ;
				}			
				#endregion
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private JurikFunction_Tillson_T3[] cacheJurikFunction_Tillson_T3 = null;

        private static JurikFunction_Tillson_T3 checkJurikFunction_Tillson_T3 = new JurikFunction_Tillson_T3();

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(double t3_boost, double t3_len)
        {
            return JurikFunction_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            if (cacheJurikFunction_Tillson_T3 != null)
                for (int idx = 0; idx < cacheJurikFunction_Tillson_T3.Length; idx++)
                    if (Math.Abs(cacheJurikFunction_Tillson_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurikFunction_Tillson_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurikFunction_Tillson_T3[idx].EqualsInput(input))
                        return cacheJurikFunction_Tillson_T3[idx];

            lock (checkJurikFunction_Tillson_T3)
            {
                checkJurikFunction_Tillson_T3.T3_boost = t3_boost;
                t3_boost = checkJurikFunction_Tillson_T3.T3_boost;
                checkJurikFunction_Tillson_T3.T3_len = t3_len;
                t3_len = checkJurikFunction_Tillson_T3.T3_len;

                if (cacheJurikFunction_Tillson_T3 != null)
                    for (int idx = 0; idx < cacheJurikFunction_Tillson_T3.Length; idx++)
                        if (Math.Abs(cacheJurikFunction_Tillson_T3[idx].T3_boost - t3_boost) <= double.Epsilon && Math.Abs(cacheJurikFunction_Tillson_T3[idx].T3_len - t3_len) <= double.Epsilon && cacheJurikFunction_Tillson_T3[idx].EqualsInput(input))
                            return cacheJurikFunction_Tillson_T3[idx];

                JurikFunction_Tillson_T3 indicator = new JurikFunction_Tillson_T3();
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

                JurikFunction_Tillson_T3[] tmp = new JurikFunction_Tillson_T3[cacheJurikFunction_Tillson_T3 == null ? 1 : cacheJurikFunction_Tillson_T3.Length + 1];
                if (cacheJurikFunction_Tillson_T3 != null)
                    cacheJurikFunction_Tillson_T3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurikFunction_Tillson_T3 = tmp;
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
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(double t3_boost, double t3_len)
        {
            return _indicator.JurikFunction_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            return _indicator.JurikFunction_Tillson_T3(input, t3_boost, t3_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(double t3_boost, double t3_len)
        {
            return _indicator.JurikFunction_Tillson_T3(Input, t3_boost, t3_len);
        }

        /// <summary>
        /// NOT AN INDICATOR -- DO NOT USE TO PLOT
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikFunction_Tillson_T3 JurikFunction_Tillson_T3(Data.IDataSeries input, double t3_boost, double t3_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikFunction_Tillson_T3(input, t3_boost, t3_len);
        }
    }
}
#endregion
