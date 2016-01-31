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
		[Description("customizable VEL")]
		public class Jurik_VEL_custom : Indicator
		#endregion
    {
		#region Input Parameters
			[Description("VEL length, any integer > 1")]
			[GridCategory("Parameters")]
			public int __VEL_len
			{
				get { return v_len; }
				set { v_len = Math.Max(2, value); }
			}
			
			[Description("Normalize to standard range? 1:yes  0:no")]
			[GridCategory("Parameters")]
			public int _NormSwitch
			{
				get { return normswitch; }
				set { normswitch = Math.Max(0, Math.Min(1,value)); }
			}
						
			[Description("Upper threshold line. Range: -2 to +2.  Use only when NormSwitch = 1.")]
			[GridCategory("Parameters")]
			public double UpperLine
			{
				get { return upperline; }
				set { upperline = Math.Max(-2, Math.Min(2,value)); }
			}		
			
			[Description("Lower threshold line. Range: -2 to +2.  Use only when NormSwitch = 1.")]
			[GridCategory("Parameters")]
			public double LowerLine
			{
				get { return lowerline; }
				set { lowerline = Math.Max(-2, Math.Min(2,value)); }
			}
			#endregion
			
		#region Variables	// default values
			private int v_len = 16; 
			private int normswitch = 1;
			private double lowerline = -1.0;
			private double upperline = 1.0;
			// ---------------------------------
			private double VELvalue = 0;
			private DataSeries PriceSeries;
			private DataSeries NormPriceSeries;
			#endregion
				
        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.Magenta, PlotStyle.Line, "VEL"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "upper"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "lower"));
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;			
			
				Add(new Line(Color.Gray, 0, "Zero Line"));
				Lines[0].Pen.Width = 1;
			
				CalculateOnBarClose	= false;
				Overlay				= false;
				PriceTypeSupported	= false;
				#endregion
						
			#region Series Initialization
				PriceSeries = new DataSeries(this);
				NormPriceSeries	= new DataSeries(this);
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator formula
			
				PriceSeries.Set( (High[0] + Low[0] + Close[0]) / 3 );
			
				if (_NormSwitch == 1)
				{
					NormPriceSeries.Set( 2*JurikFunction_volat_norm(PriceSeries, 50)[0] );
					if (CurrentBar == 50)
						for (int idx = 50-1; idx >=0; idx--)
							NormPriceSeries[idx] = NormPriceSeries[0]  ;
					VELvalue = CurrentBar <= 50 ? 0 : JurikVEL(NormPriceSeries, __VEL_len)[0] ;
				}
				else
				{
					VELvalue = CurrentBar <= 50 ? 0 : JurikVEL(PriceSeries, __VEL_len)[0] ;
					LowerLine = 0;
					UpperLine = 0;
				}
				
				VEL.Set( VELvalue );
				UpperLineSeries.Set( UpperLine );
				LowerLineSeries.Set( LowerLine );
				#endregion
        }

        #region Output Values
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries VEL
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries UpperLineSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries LowerLineSeries
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
        private Jurik_VEL_custom[] cacheJurik_VEL_custom = null;

        private static Jurik_VEL_custom checkJurik_VEL_custom = new Jurik_VEL_custom();

        /// <summary>
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        public Jurik_VEL_custom Jurik_VEL_custom(int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            return Jurik_VEL_custom(Input, __VEL_len, _NormSwitch, lowerLine, upperLine);
        }

        /// <summary>
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        public Jurik_VEL_custom Jurik_VEL_custom(Data.IDataSeries input, int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            if (cacheJurik_VEL_custom != null)
                for (int idx = 0; idx < cacheJurik_VEL_custom.Length; idx++)
                    if (cacheJurik_VEL_custom[idx].__VEL_len == __VEL_len && cacheJurik_VEL_custom[idx]._NormSwitch == _NormSwitch && Math.Abs(cacheJurik_VEL_custom[idx].LowerLine - lowerLine) <= double.Epsilon && Math.Abs(cacheJurik_VEL_custom[idx].UpperLine - upperLine) <= double.Epsilon && cacheJurik_VEL_custom[idx].EqualsInput(input))
                        return cacheJurik_VEL_custom[idx];

            lock (checkJurik_VEL_custom)
            {
                checkJurik_VEL_custom.__VEL_len = __VEL_len;
                __VEL_len = checkJurik_VEL_custom.__VEL_len;
                checkJurik_VEL_custom._NormSwitch = _NormSwitch;
                _NormSwitch = checkJurik_VEL_custom._NormSwitch;
                checkJurik_VEL_custom.LowerLine = lowerLine;
                lowerLine = checkJurik_VEL_custom.LowerLine;
                checkJurik_VEL_custom.UpperLine = upperLine;
                upperLine = checkJurik_VEL_custom.UpperLine;

                if (cacheJurik_VEL_custom != null)
                    for (int idx = 0; idx < cacheJurik_VEL_custom.Length; idx++)
                        if (cacheJurik_VEL_custom[idx].__VEL_len == __VEL_len && cacheJurik_VEL_custom[idx]._NormSwitch == _NormSwitch && Math.Abs(cacheJurik_VEL_custom[idx].LowerLine - lowerLine) <= double.Epsilon && Math.Abs(cacheJurik_VEL_custom[idx].UpperLine - upperLine) <= double.Epsilon && cacheJurik_VEL_custom[idx].EqualsInput(input))
                            return cacheJurik_VEL_custom[idx];

                Jurik_VEL_custom indicator = new Jurik_VEL_custom();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.__VEL_len = __VEL_len;
                indicator._NormSwitch = _NormSwitch;
                indicator.LowerLine = lowerLine;
                indicator.UpperLine = upperLine;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_VEL_custom[] tmp = new Jurik_VEL_custom[cacheJurik_VEL_custom == null ? 1 : cacheJurik_VEL_custom.Length + 1];
                if (cacheJurik_VEL_custom != null)
                    cacheJurik_VEL_custom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_VEL_custom = tmp;
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
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_VEL_custom Jurik_VEL_custom(int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            return _indicator.Jurik_VEL_custom(Input, __VEL_len, _NormSwitch, lowerLine, upperLine);
        }

        /// <summary>
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_VEL_custom Jurik_VEL_custom(Data.IDataSeries input, int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            return _indicator.Jurik_VEL_custom(input, __VEL_len, _NormSwitch, lowerLine, upperLine);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_VEL_custom Jurik_VEL_custom(int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            return _indicator.Jurik_VEL_custom(Input, __VEL_len, _NormSwitch, lowerLine, upperLine);
        }

        /// <summary>
        /// customizable VEL
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_VEL_custom Jurik_VEL_custom(Data.IDataSeries input, int __VEL_len, int _NormSwitch, double lowerLine, double upperLine)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_VEL_custom(input, __VEL_len, _NormSwitch, lowerLine, upperLine);
        }
    }
}
#endregion
