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
		[Description("JMA Double Stochastic")]
		public class Jurik_JMA_DoubleStoch : Indicator
		#endregion
    {
        #region Variables	// default values
            private int stoch_len = 12; 	
            private double jma_len = 5; 	
			// ------------------------------
			private double hh1 = 0 ;
			private double ll1 = 0 ;
			private double hh2 = 0 ;
			private double ll2 = 0 ;
			private DataSeries stoch1;
			private DataSeries stoch2;
			private DataSeries JMAstoch1;
			private DataSeries JMAstoch2;
        	#endregion
		
		#region Input Parameters
			[Description("JMA length, any value >= 1")]
			[GridCategory("Parameters")]
			public double Jma_len
			{
				get { return jma_len; }
				set { jma_len = Math.Max(1, value); }
			}
			
			[Description("Stochastic lookback, any integer > 0")]
			[GridCategory("Parameters")]
			public int Stoch_len
			{
				get { return stoch_len; }
				set { stoch_len = Math.Max(1, value); }
			}
	       	#endregion

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));  // invisible            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));  // invisible
				Add(new Plot(Color.Cyan, PlotStyle.Line, "Stoch"));
			
				Plots[2].Pen.Width = 2;
			
				Add(new Line(Color.Gray, -60, "Lower Line"));
				Add(new Line(Color.Gray, 60, "Upper Line"));
				Add(new Line(Color.Gray, 0, "Zero Line"));
			
				Lines[0].Pen.DashStyle = DashStyle.Dash;
				Lines[1].Pen.DashStyle = DashStyle.Dash;
				Lines[2].Pen.Width = 2;
				
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				stoch1 = new DataSeries(this);
				stoch2 = new DataSeries(this);			
				JMAstoch1 = new DataSeries(this);
				JMAstoch2 = new DataSeries(this);
				#endregion
		}

        protected override void OnBarUpdate()
        {
			#region Indicator formula
				hh1 = MAX(High, Stoch_len)[0];
				ll1 = MIN(Low, Stoch_len)[0];
				stoch1.Set((hh1 - ll1  < 0.0000001 ? 0.5 : (Close[0] - ll1) / (hh1 - ll1)));
				JMAstoch1.Set( JurikJMA(stoch1, -100, Jma_len)[0] );
				
				hh2 = MAX( JMAstoch1, Stoch_len)[0];
				ll2 = MIN( JMAstoch1, Stoch_len)[0];
				stoch2.Set((hh2 - ll2  < 0.0000001 ? 0.5 : ( JMAstoch1[0] - ll2) / (hh2 - ll2)));
				JMAstoch2.Set( JurikJMA(stoch2, -100, Jma_len)[0] );
				
				K.Set( CurrentBar >= 50 ? (JMAstoch2[0] * 200) - 100 : 0 );
				#endregion
			
			#region Panel Stabilizer
				panel_range_min.Set(-101);
				panel_range_max.Set(101);
			#endregion
        }

        #region Output Parameters
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[0]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries K
			{
				get { return Values[2]; }
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
        private Jurik_JMA_DoubleStoch[] cacheJurik_JMA_DoubleStoch = null;

        private static Jurik_JMA_DoubleStoch checkJurik_JMA_DoubleStoch = new Jurik_JMA_DoubleStoch();

        /// <summary>
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(double jma_len, int stoch_len)
        {
            return Jurik_JMA_DoubleStoch(Input, jma_len, stoch_len);
        }

        /// <summary>
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        public Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(Data.IDataSeries input, double jma_len, int stoch_len)
        {
            if (cacheJurik_JMA_DoubleStoch != null)
                for (int idx = 0; idx < cacheJurik_JMA_DoubleStoch.Length; idx++)
                    if (Math.Abs(cacheJurik_JMA_DoubleStoch[idx].Jma_len - jma_len) <= double.Epsilon && cacheJurik_JMA_DoubleStoch[idx].Stoch_len == stoch_len && cacheJurik_JMA_DoubleStoch[idx].EqualsInput(input))
                        return cacheJurik_JMA_DoubleStoch[idx];

            lock (checkJurik_JMA_DoubleStoch)
            {
                checkJurik_JMA_DoubleStoch.Jma_len = jma_len;
                jma_len = checkJurik_JMA_DoubleStoch.Jma_len;
                checkJurik_JMA_DoubleStoch.Stoch_len = stoch_len;
                stoch_len = checkJurik_JMA_DoubleStoch.Stoch_len;

                if (cacheJurik_JMA_DoubleStoch != null)
                    for (int idx = 0; idx < cacheJurik_JMA_DoubleStoch.Length; idx++)
                        if (Math.Abs(cacheJurik_JMA_DoubleStoch[idx].Jma_len - jma_len) <= double.Epsilon && cacheJurik_JMA_DoubleStoch[idx].Stoch_len == stoch_len && cacheJurik_JMA_DoubleStoch[idx].EqualsInput(input))
                            return cacheJurik_JMA_DoubleStoch[idx];

                Jurik_JMA_DoubleStoch indicator = new Jurik_JMA_DoubleStoch();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Jma_len = jma_len;
                indicator.Stoch_len = stoch_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_JMA_DoubleStoch[] tmp = new Jurik_JMA_DoubleStoch[cacheJurik_JMA_DoubleStoch == null ? 1 : cacheJurik_JMA_DoubleStoch.Length + 1];
                if (cacheJurik_JMA_DoubleStoch != null)
                    cacheJurik_JMA_DoubleStoch.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_JMA_DoubleStoch = tmp;
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
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(double jma_len, int stoch_len)
        {
            return _indicator.Jurik_JMA_DoubleStoch(Input, jma_len, stoch_len);
        }

        /// <summary>
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(Data.IDataSeries input, double jma_len, int stoch_len)
        {
            return _indicator.Jurik_JMA_DoubleStoch(input, jma_len, stoch_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(double jma_len, int stoch_len)
        {
            return _indicator.Jurik_JMA_DoubleStoch(Input, jma_len, stoch_len);
        }

        /// <summary>
        /// JMA Double Stochastic
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_JMA_DoubleStoch Jurik_JMA_DoubleStoch(Data.IDataSeries input, double jma_len, int stoch_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_JMA_DoubleStoch(input, jma_len, stoch_len);
        }
    }
}
#endregion
