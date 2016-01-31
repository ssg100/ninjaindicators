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
		[Description("VEL on VEL")]
		public class Jurik_VEL_on_VEL : Indicator
		#endregion
    {
		#region Input Parameters
			[Description("lower band, any value between -2 and +2")]
			[GridCategory("Parameters")]
			public double __LowerBand
			{
				get { return lowerband; }
				set { lowerband = Math.Min(2, Math.Max(-2, value)); }
			}
			
			[Description("upper band, any value between -2 and +2")]
			[GridCategory("Parameters")]
			public double __UpperBand
			{
				get { return upperband; }
				set { upperband = Math.Min(2, Math.Max(-2, value)); }
			}
			
			[Description("VEL length, any integer > 1")]
			[GridCategory("Parameters")]
			public int _VEL_len
			{
				get { return vellen; }
				set { vellen = Math.Max(2, value); }
			}
			
			[Description("VEL blend factor, any value between -2 and +2")]
			[GridCategory("Parameters")]
			public double Blend
			{
				get { return blend; }
				set { blend = Math.Min(2, Math.Max(-2, value)); }
			}
			
			[Description("color phase shift, any value between -2 and =2")]
			[GridCategory("Parameters")]
			public double PhaseShift
			{
				get { return phaseshift; }
				set { phaseshift =  Math.Min(2, Math.Max(-2, value)); }
			}
			#endregion
			
        #region Variables    // default settings
			private double lowerband = -0.3;  
			private double upperband = 0.3; 
			private int vellen = 80;  
			private double blend = 0.8;  
			private double phaseshift = 0.8; 
 			// ----------------------------------
			private double phasedValue = 0;
			private double VEL1 = 0;
			private double VEL2 = 0;
			private double sqrtLen = 0;
			// ----------------------------------			
			private DataSeries PriceSeries;
			private DataSeries VEL1_Series;
			private DataSeries NormPriceSeries;
			#endregion
			
        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.Black, PlotStyle.Line, "VEL blend"));
				Add(new Plot(Color.Blue, PlotStyle.Hash, "upper"));
				Add(new Plot(Color.Blue, PlotStyle.Hash, "lower"));
				Add(new Plot(Color.YellowGreen, PlotStyle.Bar, "VEL+"));
				Add(new Plot(Color.MediumOrchid, PlotStyle.Bar, "VEL-"));
				Add(new Plot(Color.White, PlotStyle.Line, "phased blend"));  // invisible plot
			
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
				PriceSeries 	= new DataSeries(this);
				VEL1_Series 	= new DataSeries(this);
				NormPriceSeries	= new DataSeries(this);
				#endregion
			
			#region Constants
				sqrtLen = Math.Sqrt( _VEL_len ) ;
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator Formula
			
				UpperSeries.Set( __UpperBand );
				LowerSeries.Set( __LowerBand );
				PriceSeries.Set( (High[0] + Low[0] + Close[0]) / 3 );
				NormPriceSeries.Set( 2*JurikFunction_volat_norm(PriceSeries, 50)[0] );

				if (CurrentBar == 50)
					for (int idx = 50-1; idx >=0; idx--)
						NormPriceSeries[idx] = NormPriceSeries[0]  ;

				VEL1 = CurrentBar <= 50 ? 0 : JurikVEL(NormPriceSeries, _VEL_len)[0] ;
				VEL1_Series.Set( VEL1 ) ;
				VEL2 = JurikVEL(VEL1_Series, 4)[0] * sqrtLen ;
				VEL_blend.Set( (1-Blend) * VEL1 + Blend * VEL2 ) ;				
	
				if (CurrentBar > 0)
					phasedValue = VEL_blend[0] - PhaseShift * VEL_blend[1] ;

				VELVELpos.Set( phasedValue > 0 ? VEL_blend[0] : 0 ); 
				VELVELneg.Set( phasedValue < 0 ? VEL_blend[0] : 0 ); 
				Phased_blend.Set( phasedValue );				
				#endregion
        }

        #region Output Values

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries VEL_blend
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
			public DataSeries VELVELpos
			{
				get { return Values[3]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries VELVELneg
			{
				get { return Values[4]; }
			}

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Phased_blend
			{
				get { return Values[5]; }
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
        private Jurik_VEL_on_VEL[] cacheJurik_VEL_on_VEL = null;

        private static Jurik_VEL_on_VEL checkJurik_VEL_on_VEL = new Jurik_VEL_on_VEL();

        /// <summary>
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        public Jurik_VEL_on_VEL Jurik_VEL_on_VEL(double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            return Jurik_VEL_on_VEL(Input, __LowerBand, __UpperBand, _VEL_len, blend, phaseShift);
        }

        /// <summary>
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        public Jurik_VEL_on_VEL Jurik_VEL_on_VEL(Data.IDataSeries input, double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            if (cacheJurik_VEL_on_VEL != null)
                for (int idx = 0; idx < cacheJurik_VEL_on_VEL.Length; idx++)
                    if (Math.Abs(cacheJurik_VEL_on_VEL[idx].__LowerBand - __LowerBand) <= double.Epsilon && Math.Abs(cacheJurik_VEL_on_VEL[idx].__UpperBand - __UpperBand) <= double.Epsilon && cacheJurik_VEL_on_VEL[idx]._VEL_len == _VEL_len && Math.Abs(cacheJurik_VEL_on_VEL[idx].Blend - blend) <= double.Epsilon && Math.Abs(cacheJurik_VEL_on_VEL[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_VEL_on_VEL[idx].EqualsInput(input))
                        return cacheJurik_VEL_on_VEL[idx];

            lock (checkJurik_VEL_on_VEL)
            {
                checkJurik_VEL_on_VEL.__LowerBand = __LowerBand;
                __LowerBand = checkJurik_VEL_on_VEL.__LowerBand;
                checkJurik_VEL_on_VEL.__UpperBand = __UpperBand;
                __UpperBand = checkJurik_VEL_on_VEL.__UpperBand;
                checkJurik_VEL_on_VEL._VEL_len = _VEL_len;
                _VEL_len = checkJurik_VEL_on_VEL._VEL_len;
                checkJurik_VEL_on_VEL.Blend = blend;
                blend = checkJurik_VEL_on_VEL.Blend;
                checkJurik_VEL_on_VEL.PhaseShift = phaseShift;
                phaseShift = checkJurik_VEL_on_VEL.PhaseShift;

                if (cacheJurik_VEL_on_VEL != null)
                    for (int idx = 0; idx < cacheJurik_VEL_on_VEL.Length; idx++)
                        if (Math.Abs(cacheJurik_VEL_on_VEL[idx].__LowerBand - __LowerBand) <= double.Epsilon && Math.Abs(cacheJurik_VEL_on_VEL[idx].__UpperBand - __UpperBand) <= double.Epsilon && cacheJurik_VEL_on_VEL[idx]._VEL_len == _VEL_len && Math.Abs(cacheJurik_VEL_on_VEL[idx].Blend - blend) <= double.Epsilon && Math.Abs(cacheJurik_VEL_on_VEL[idx].PhaseShift - phaseShift) <= double.Epsilon && cacheJurik_VEL_on_VEL[idx].EqualsInput(input))
                            return cacheJurik_VEL_on_VEL[idx];

                Jurik_VEL_on_VEL indicator = new Jurik_VEL_on_VEL();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.__LowerBand = __LowerBand;
                indicator.__UpperBand = __UpperBand;
                indicator._VEL_len = _VEL_len;
                indicator.Blend = blend;
                indicator.PhaseShift = phaseShift;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_VEL_on_VEL[] tmp = new Jurik_VEL_on_VEL[cacheJurik_VEL_on_VEL == null ? 1 : cacheJurik_VEL_on_VEL.Length + 1];
                if (cacheJurik_VEL_on_VEL != null)
                    cacheJurik_VEL_on_VEL.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_VEL_on_VEL = tmp;
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
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_VEL_on_VEL Jurik_VEL_on_VEL(double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_VEL_on_VEL(Input, __LowerBand, __UpperBand, _VEL_len, blend, phaseShift);
        }

        /// <summary>
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_VEL_on_VEL Jurik_VEL_on_VEL(Data.IDataSeries input, double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_VEL_on_VEL(input, __LowerBand, __UpperBand, _VEL_len, blend, phaseShift);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_VEL_on_VEL Jurik_VEL_on_VEL(double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            return _indicator.Jurik_VEL_on_VEL(Input, __LowerBand, __UpperBand, _VEL_len, blend, phaseShift);
        }

        /// <summary>
        /// VEL on VEL
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_VEL_on_VEL Jurik_VEL_on_VEL(Data.IDataSeries input, double __LowerBand, double __UpperBand, int _VEL_len, double blend, double phaseShift)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_VEL_on_VEL(input, __LowerBand, __UpperBand, _VEL_len, blend, phaseShift);
        }
    }
}
#endregion
