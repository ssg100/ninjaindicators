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
		[Description("customizable RSX")]
		public class Jurik_RSX_custom : Indicator
		#endregion
    {
        #region Variables	// default values
            private double r_len = 14;   
            private double botLine = 30; 
            private double topLine = 70;  
			// ----------------------------
			private double RSXvalue = 0;
            private DataSeries PriceSeries;
            #endregion

        #region Input Parameters
            [Description("RSX length, any value >= 2")]
            [GridCategory("Parameters")]
            public double _RSX_len
            {
                get { return r_len; }
                set { r_len = Math.Max(2, value); }
            }
		
			[Description("bot flat line, any value between 0 and 100")]
			[GridCategory("Parameters")]
			public double BotLine
			{
				get { return botLine; }
				set { botLine = Math.Min(Math.Max(0, value), 100); }
			}
			
			[Description("top flat line, any value between 0 and 100")]
			[GridCategory("Parameters")]
			public double TopLine
			{
				get { return topLine; }
				set { topLine = Math.Min(Math.Max(0, value), 100); }
			}
			#endregion
		
        protected override void Initialize()
        {
			#region Chart Features
				Add(new Plot(Color.DodgerBlue, PlotStyle.Line, "RSX"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Top Line"));
				Add(new Plot(Color.Gray, PlotStyle.Hash, "Bot Line"));
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range min"));  // invisible            
				Add(new Plot(Color.White, PlotStyle.Line, ".panel range max"));  // invisible
			
				Plots[0].Pen.Width = 2;
				Plots[1].Pen.DashStyle = DashStyle.Dash;
				Plots[2].Pen.DashStyle = DashStyle.Dash;
			
				Add(new Line(Color.Gray, 50, "Mid Line"));
				Lines[0].Pen.Width = 2;
			
				CalculateOnBarClose	= false;
				Overlay				= false;
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
				TopLineSeries.Set( TopLine );
				BotLineSeries.Set( BotLine );
				
				RSXvalue =  JurikRSX( PriceSeries, _RSX_len)[0];
				RSX.Set( CurrentBar >= 31 ? RSXvalue : 50 ) ;
				#endregion
			
			#region Panel Stabilizer
				Panel_range_min.Set(-1);
				Panel_range_max.Set(101);
				#endregion
        }

        #region Output Values
		
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries RSX
			{
				get { return Values[0]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries TopLineSeries
			{
				get { return Values[1]; }
			}
			
			[Browsable(false)]		// do not remove
			[XmlIgnore()]			// do not remove
			public DataSeries BotLineSeries
			{
				get { return Values[2]; }
			}
			
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Panel_range_min
			{
				get { return Values[3]; }
			}
	
			[Browsable(false)]	//  do not remove
			[XmlIgnore()]		//  do not remove
			public DataSeries Panel_range_max
			{
				get { return Values[4]; }
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
        private Jurik_RSX_custom[] cacheJurik_RSX_custom = null;

        private static Jurik_RSX_custom checkJurik_RSX_custom = new Jurik_RSX_custom();

        /// <summary>
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_custom Jurik_RSX_custom(double _RSX_len, double botLine, double topLine)
        {
            return Jurik_RSX_custom(Input, _RSX_len, botLine, topLine);
        }

        /// <summary>
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        public Jurik_RSX_custom Jurik_RSX_custom(Data.IDataSeries input, double _RSX_len, double botLine, double topLine)
        {
            if (cacheJurik_RSX_custom != null)
                for (int idx = 0; idx < cacheJurik_RSX_custom.Length; idx++)
                    if (Math.Abs(cacheJurik_RSX_custom[idx]._RSX_len - _RSX_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_custom[idx].BotLine - botLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_custom[idx].TopLine - topLine) <= double.Epsilon && cacheJurik_RSX_custom[idx].EqualsInput(input))
                        return cacheJurik_RSX_custom[idx];

            lock (checkJurik_RSX_custom)
            {
                checkJurik_RSX_custom._RSX_len = _RSX_len;
                _RSX_len = checkJurik_RSX_custom._RSX_len;
                checkJurik_RSX_custom.BotLine = botLine;
                botLine = checkJurik_RSX_custom.BotLine;
                checkJurik_RSX_custom.TopLine = topLine;
                topLine = checkJurik_RSX_custom.TopLine;

                if (cacheJurik_RSX_custom != null)
                    for (int idx = 0; idx < cacheJurik_RSX_custom.Length; idx++)
                        if (Math.Abs(cacheJurik_RSX_custom[idx]._RSX_len - _RSX_len) <= double.Epsilon && Math.Abs(cacheJurik_RSX_custom[idx].BotLine - botLine) <= double.Epsilon && Math.Abs(cacheJurik_RSX_custom[idx].TopLine - topLine) <= double.Epsilon && cacheJurik_RSX_custom[idx].EqualsInput(input))
                            return cacheJurik_RSX_custom[idx];

                Jurik_RSX_custom indicator = new Jurik_RSX_custom();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator._RSX_len = _RSX_len;
                indicator.BotLine = botLine;
                indicator.TopLine = topLine;
                Indicators.Add(indicator);
                indicator.SetUp();

                Jurik_RSX_custom[] tmp = new Jurik_RSX_custom[cacheJurik_RSX_custom == null ? 1 : cacheJurik_RSX_custom.Length + 1];
                if (cacheJurik_RSX_custom != null)
                    cacheJurik_RSX_custom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJurik_RSX_custom = tmp;
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
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_custom Jurik_RSX_custom(double _RSX_len, double botLine, double topLine)
        {
            return _indicator.Jurik_RSX_custom(Input, _RSX_len, botLine, topLine);
        }

        /// <summary>
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_custom Jurik_RSX_custom(Data.IDataSeries input, double _RSX_len, double botLine, double topLine)
        {
            return _indicator.Jurik_RSX_custom(input, _RSX_len, botLine, topLine);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Jurik_RSX_custom Jurik_RSX_custom(double _RSX_len, double botLine, double topLine)
        {
            return _indicator.Jurik_RSX_custom(Input, _RSX_len, botLine, topLine);
        }

        /// <summary>
        /// customizable RSX
        /// </summary>
        /// <returns></returns>
        public Indicator.Jurik_RSX_custom Jurik_RSX_custom(Data.IDataSeries input, double _RSX_len, double botLine, double topLine)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Jurik_RSX_custom(input, _RSX_len, botLine, topLine);
        }
    }
}
#endregion
