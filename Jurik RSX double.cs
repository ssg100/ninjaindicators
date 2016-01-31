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
		[Description("RSX double")]
		public class Jurik_RSX_double : Indicator
		#endregion
    {
        #region Variables    // default settinfgs
            private double rsx_fast_len = 20;  
            private double rsx_slow_len = 30;  
		    private double blend = 2.0;  
		    private double upperband = 50;  
		    private double lowerband = -50;  
		    private double phaseshift = 0.9; 
			// ----------------------------------
			private double phasedValue = 0;
			private double RSX_fast = 0;
			private double RSX_slow = 0;
			private double mixture = 0;
			private DataSeries PriceSeries;
			#endregion
		
		#region Input Parameters
			[Description("lower band, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double __LowerBand
			{
				get { return lowerband; }
				set { lowerband = Math.Min(100, Math.Max(-100, value)); }
			}
			
			[Description("upper band, any value between -100 and +100")]
			[GridCategory("Parameters")]
			public double __UpperBand
			{
				get { return upperband; }
				set { upperband = Math.Min(100, Math.Max(-100, value)); }
			}
						
			[Description("fast RSX length, any value >= 2")]
			[GridCategory("Parameters")]
			public double _RSX_fast_len
			{
				get { return rsx_fast_len; }
				set { rsx_fast_len = Math.Max(2, value); }
			}
			
			[Description("slow RSX length, any value >= 2")]
			[GridCategory("Parameters")]
			public double _RSX_slow_len
			{
				get { return rsx_slow_len; }
				set { rsx_slow_len = Math.Max(2, value); }
			}
			
			[Description("RSX blend factor, any value between -3 and +3")]
			[GridCategory("Parameters")]
			public double Blend
			{
				get { return blend; }
				set { blend = Math.Min(3, Math.Max(-3,value)); }
			}
						
			[Description("color phase shift, any value between -2 and +2")]
			[GridCategory("Parameters")]
			public double PhaseShift
			{
				get { return phaseshift; }
				set { phaseshift =  Math.Min(2, Math.Max(-2, value)); }
			}
			#endregion

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.Black, PlotStyle.Line, "RSX blend"));
				Add(new Plot(Color.Blue, PlotStyle.Hash, "upper"));
				Add(new Plot(Color.Blue, PlotStyle.Hash, "lower"));
				Add(new Plot(Color.MediumTurquoise, PlotStyle.Bar, "RSX+"));
				Add(new Plot(Color.MediumVioletRed, PlotStyle.Bar, "RSX-"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));  // invisible            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));  // invisible
				Add(new Plot(Color.White, PlotStyle.Line, "phased blend"));  	// invisible  

				Plots[0].Pen.Width = 2 ;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;
				Plots[3].Pen.Width = 2 ;
				Plots[4].Pen.Width = 2 ;		
				
				CalculateOnBarClose	= false ;
				Overlay				= false ;
				PriceTypeSupported	= false ;
				#endregion			
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator Formula
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3 );
				UpperSeries.Set( __UpperBand );
				LowerSeries.Set( __LowerBand );
			
				RSX_fast = JurikRSX(PriceSeries, _RSX_fast_len)[0] ;
				RSX_slow = JurikRSX(PriceSeries, _RSX_slow_len)[0] ;
				mixture = (1-Blend) * RSX_slow + Blend * RSX_fast ;
				RSX_blend.Set( CurrentBar < 31 ? 0 : 2*mixture-100 ) ;	
						
				if (CurrentBar > 0)
					phasedValue = RSX_blend[0] - PhaseShift * RSX_blend[1] ;
		
				RSXRSXpos.Set( phasedValue > 0 ? RSX_blend[0] : 0 ); 
				RSXRSXneg.Set( phasedValue < 0 ? RSX_blend[0] : 0 ); 
				Phased_blend.Set( phasedValue );
				#endregion

			#region Panel Stabilizer
				panel_range_min.Set(-101);
				panel_range_max.Set(101);
				#endregion
		}

        #region Output Values		

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSX_blend
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries UpperSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries LowerSeries
			{
				get { return Values[2]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSXRSXpos
			{
				get { return Values[3]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries RSXRSXneg
			{
				get { return Values[4]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[5]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
			{
				get { return Values[6]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Phased_blend
			{
				get { return Values[7]; }
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
        private Jurik_RSX_double[] cacheJurik_RSX_double = null;

        private static Jurik_RSX_double checkJurik_RSX_double = new Jurik_RSX_double();

        /// <summary>
        /// RSX double
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_double Jurik_RSX_double(double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            return Jurik_RSX_double(Input, __LowerBand, __UpperBand, _RSX_fast_len, _RSX_slow_len, blend, phaseShift);
        }

        /// <summary>
        /// RSX double
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_double Jurik_RSX_double(Data.IDataSeries input, double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            if (cacheJurik_RSX_double != null)
                for (int idx = 0; idx < cacheJurik_RSX_double.Length; idx++)
                    if (Math.Abs(cacheJurik_RSX_double[idx].__LowerBand - __LowerBand) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].__UpperBand - __UpperBand) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx]._RSX_fast_len - _RSX_fast_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx]._RSX_slow_len - _RSX_slow_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].Blend - blend) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_RSX_double[idx].EqualsInput(input))
                        return cacheJurik_RSX_double[idx];

            lock (checkJurik_RSX_double)
            {
                checkJurik_RSX_double.__LowerBand = __LowerBand;
                __LowerBand = checkJurik_RSX_double.__LowerBand;
                checkJurik_RSX_double.__UpperBand = __UpperBand;
                __UpperBand = checkJurik_RSX_double.__UpperBand;
                checkJurik_RSX_double._RSX_fast_len = _RSX_fast_len;
                _RSX_fast_len = checkJurik_RSX_double._RSX_fast_len;
                checkJurik_RSX_double._RSX_slow_len = _RSX_slow_len;
                _RSX_slow_len = checkJurik_RSX_double._RSX_slow_len;
                checkJurik_RSX_double.Blend = blend;
                blend = checkJurik_RSX_double.Blend;
                checkJurik_RSX_double.PhaseShift = phaseShift;
                phaseShift = checkJurik_RSX_double.PhaseShift;

                if (cacheJurik_RSX_double != null)
                    for (int idx = 0; idx < cacheJurik_RSX_double.Length; idx++)
                        if (Math.Abs(cacheJurik_RSX_double[idx].__LowerBand - __LowerBand) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].__UpperBand - __UpperBand) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx]._RSX_fast_len - _RSX_fast_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx]._RSX_slow_len - _RSX_slow_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].Blend - blend) <= double.Epsilon && Math.Abs(cacheJurik_RSX_double[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_RSX_double[idx].EqualsInput(input))
                            return cacheJurik_RSX_double[idx];

                Jurik_RSX_double indicator = new Jurik_RSX_double();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.__LowerBand = __LowerBand;
                indicator.__UpperBand = __UpperBand;
                indicator._RSX_fast_len = _RSX_fast_len;
                indicator._RSX_slow_len = _RSX_slow_len;
                indicator.Blend = blend;
                indicator.PhaseShift = phaseShift;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_RSX_double[] tmp = new Jurik_RSX_double[cacheJurik_RSX_double == null ? 1 : cacheJurik_RSX_double.Length + 1];
                if (cacheJurik_RSX_double != null)
                    cacheJurik_RSX_double.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_RSX_double = tmp;
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
        /// RSX double
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_double Jurik_RSX_double(double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_RSX_double(Input, __LowerBand, __UpperBand, _RSX_fast_len, _RSX_slow_len, blend, phaseShift);
        }

        /// <summary>
        /// RSX double
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_double Jurik_RSX_double(Data.IDataSeries input, double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_RSX_double(input, __LowerBand, __UpperBand, _RSX_fast_len, _RSX_slow_len, blend, phaseShift);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// RSX double
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_double Jurik_RSX_double(double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_RSX_double(Input, __LowerBand, __UpperBand, _RSX_fast_len, _RSX_slow_len, blend, phaseShift);
        }

        /// <summary>
        /// RSX double
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_double Jurik_RSX_double(Data.IDataSeries input, double __LowerBand, double __UpperBand, double _RSX_fast_len, double _RSX_slow_len, double blend, double phaseShift)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_RSX_double(input, __LowerBand, __UpperBand, _RSX_fast_len, _RSX_slow_len, blend, phaseShift);
        }
    }
}
#endregion
