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
		[Description("customizable CCX")]
		public class Jurik_CCX_custom : Indicator
		#endregion
    {
		#region Variables	// default values
			private int cCX_len = 30; 	
			private double botLine = -100; 
			private double topLine = 100;
			// ----------------------------------
			private double DWMAvalue = 0;
			private double JMAValue = 0;
			private double numer = 0;
			private double denom = 0;
			private double ccx_value = 0;
			private DataSeries PriceSeries;
			private DataSeries AbsNumerSeries;		
			#endregion
		
		#region Input Parameters   
			[Description("bot flat line, any value between -150 and +150")]
			[GridCategory("Parameters")]
			public double _BotLine
			{
				get { return botLine; }
				set { botLine = Math.Min(Math.Max(-150, value), 150); }
			}
				
			[Description("top flat line, any value between -150 and +150")]
			[GridCategory("Parameters")]
			public double _TopLine
			{
				get { return topLine; }
				set { topLine = Math.Min(Math.Max(-150, value), 150); }
			}
			
			[Description("CCX length, any integer > 2")]
			[GridCategory("Parameters")]
			public int CCX_len
			{
				get { return cCX_len; }
				set { cCX_len = Math.Max(3, value); }
			}
			#endregion

        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.MediumTurquoise, PlotStyle.Line, "CCX"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Top Line"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Bot Line"));
				Add(new Plot(Color.Black, PlotStyle.Line, "Zero Line"));			
				Add(new Plot(Color.White, PlotStyle.Bar, "White Bar"));
				Add(new Plot(Color.Black, PlotStyle.Bar, "Black Bar"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;
				Plots[4].Pen.Width = 2;
				Plots[5].Pen.Width = 2;
					
	        	CalculateOnBarClose	= false;
            	Overlay				= false;
            	PriceTypeSupported	= false;
				#endregion
					
			#region Series Initialization
				PriceSeries = new DataSeries(this);		// sync dataseries to historical data bars
				AbsNumerSeries = new DataSeries(this);	// sync dataseries to historical data bars
				#endregion
        }

        protected override void OnBarUpdate()
        {
			#region Indicator formula
				PriceSeries.Set( ( High[0] + Low[0] + Close[0] )/3 );
				JMAValue 	= JurikJMA(PriceSeries, 50, 6)[0];
				DWMAvalue 	= WMA(WMA(PriceSeries, CCX_len), CCX_len)[0];
				numer 		= JMAValue - DWMAvalue;
				AbsNumerSeries.Set(Math.Abs(numer));
				denom 		= 0.015 * SMA(AbsNumerSeries,255)[0];
				ccx_value = (CurrentBar >= 2*CCX_len && denom > 0.0000001) ? numer / denom : 0 ;
				CCX.Set(ccx_value);
			
				_CCXbarBlack.Set(ccx_value);
				_CCXbarWhite.Set(Math.Min(_TopLine, Math.Max(_BotLine, ccx_value)));
				_TopLineSeries.Set( _TopLine );
				_BotLineSeries.Set( _BotLine );	
				_ZeroLineSeries.Set(0);
				#endregion
			
			#region Panel Stabilizer
				panel_range_min.Set(-200);
				panel_range_max.Set(200);
				#endregion
        }

        #region Output Values
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries CCX
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _TopLineSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _BotLineSeries
			{
				get { return Values[2]; }
			}
						
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _ZeroLineSeries
			{
				get { return Values[3]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _CCXbarWhite
			{
				get { return Values[4]; }
			}	
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries _CCXbarBlack
			{
				get { return Values[5]; }
			}			

			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_min
			{
				get { return Values[6]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries panel_range_max
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
        private Jurik_CCX_custom[] cacheJurik_CCX_custom = null;

        private static Jurik_CCX_custom checkJurik_CCX_custom = new Jurik_CCX_custom();

        /// <summary>
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        public Jurik_CCX_custom Jurik_CCX_custom(double _BotLine, double _TopLine, int cCX_len)
        {
            return Jurik_CCX_custom(Input, _BotLine, _TopLine, cCX_len);
        }

        /// <summary>
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        public Jurik_CCX_custom Jurik_CCX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int cCX_len)
        {
            if (cacheJurik_CCX_custom != null)
                for (int idx = 0; idx < cacheJurik_CCX_custom.Length; idx++)
                    if (Math.Abs(cacheJurik_CCX_custom[idx]._BotLine - _BotLine) <= double.Epsilon && Math.Abs(cacheJurik_CCX_custom[idx]._TopLine - _TopLine) <= double.Epsilon && cacheJurik_CCX_custom[idx].CCX_len == cCX_len && cacheJurik_CCX_custom[idx].EqualsInput(input))
                        return cacheJurik_CCX_custom[idx];

            lock (checkJurik_CCX_custom)
            {
                checkJurik_CCX_custom._BotLine = _BotLine;
                _BotLine = checkJurik_CCX_custom._BotLine;
                checkJurik_CCX_custom._TopLine = _TopLine;
                _TopLine = checkJurik_CCX_custom._TopLine;
                checkJurik_CCX_custom.CCX_len = cCX_len;
                cCX_len = checkJurik_CCX_custom.CCX_len;

                if (cacheJurik_CCX_custom != null)
                    for (int idx = 0; idx < cacheJurik_CCX_custom.Length; idx++)
                        if (Math.Abs(cacheJurik_CCX_custom[idx]._BotLine - _BotLine) <= double.Epsilon && Math.Abs(cacheJurik_CCX_custom[idx]._TopLine - _TopLine) <= double.Epsilon && cacheJurik_CCX_custom[idx].CCX_len == cCX_len && cacheJurik_CCX_custom[idx].EqualsInput(input))
                            return cacheJurik_CCX_custom[idx];

                Jurik_CCX_custom indicator = new Jurik_CCX_custom();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator._BotLine = _BotLine;
                indicator._TopLine = _TopLine;
                indicator.CCX_len = cCX_len;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_CCX_custom[] tmp = new Jurik_CCX_custom[cacheJurik_CCX_custom == null ? 1 : cacheJurik_CCX_custom.Length + 1];
                if (cacheJurik_CCX_custom != null)
                    cacheJurik_CCX_custom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_CCX_custom = tmp;
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
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_CCX_custom Jurik_CCX_custom(double _BotLine, double _TopLine, int cCX_len)
        {
            return _indicator.Jurik_CCX_custom(Input, _BotLine, _TopLine, cCX_len);
        }

        /// <summary>
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_CCX_custom Jurik_CCX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int cCX_len)
        {
            return _indicator.Jurik_CCX_custom(input, _BotLine, _TopLine, cCX_len);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_CCX_custom Jurik_CCX_custom(double _BotLine, double _TopLine, int cCX_len)
        {
            return _indicator.Jurik_CCX_custom(Input, _BotLine, _TopLine, cCX_len);
        }

        /// <summary>
        /// customizable CCX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_CCX_custom Jurik_CCX_custom(Data.IDataSeries input, double _BotLine, double _TopLine, int cCX_len)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_CCX_custom(input, _BotLine, _TopLine, cCX_len);
        }
    }
}
#endregion
