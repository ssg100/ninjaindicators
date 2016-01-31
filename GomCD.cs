#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
#endregion

namespace NinjaTrader.Indicator
{
	/// <summary>
    /// GomCD
    /// </summary>
    [Description("Gom Cumulative Delta")]
	public class GomCD : GomDeltaIndicator
	{
		#region Variables
		// Wizard generated variables
		// User defined variables (add any user defined variables below)

		private GomPaintType PtType = GomPaintType.UpDown;
		private GomCDChartType chart = GomCDChartType.CumulativeChart;
		private bool ShowOutline = false;
		private bool EnhanceHL = true;


		private bool ReinitSession = false;
		private bool ForceHiLo = true;

		private int ForcedHiLoBS = 2;

		private int totalvolume = 0, hi, lo;

		private DataSeries dsOpen, dsHigh, dsLow, dsClose;

		private int startbar = -1;
		private int lastcalcbar = -1;

		private bool useplot = true;
		Pen drawPen = new Pen(Color.Transparent);
		SolidBrush drawBrush = new SolidBrush(Color.Transparent);

		#endregion


		protected override void GomInitialize()
		{
			Overlay = false;
			PriceTypeSupported = false;

			Add(new Plot(Color.Transparent, "DeltaValue"));
		}

		protected override void GomOnStartUp()
		{
			dsOpen = new DataSeries(this, MaximumBarsLookBack.Infinite);
			dsHigh = new DataSeries(this, MaximumBarsLookBack.Infinite);
			dsLow = new DataSeries(this, MaximumBarsLookBack.Infinite);
			dsClose = new DataSeries(this, MaximumBarsLookBack.Infinite);
		}


		protected override void GomOnBarUpdate()
		{

			if (FirstTickOfBar)
			{
				if ((chart == GomCDChartType.NonCumulativeChart) || (ReinitSession && Bars.FirstBarOfSession))
					totalvolume = 0;

				dsOpen.Set(totalvolume);
				hi = totalvolume;
				lo = totalvolume;
				lastcalcbar = CurrentBar;
			}

			if (startbar == -1)
				startbar = CurrentBar;

			lastcalcbar = CurrentBar;

		}

		//void ProcessDelta(int delta, bool firstTick)
		protected override void GomOnMarketData(Gom.MarketDataType e)
		{
			int delta = CalcDelta(e);

			totalvolume += delta;

			hi = Math.Max(hi, totalvolume);
			lo = Math.Min(lo, totalvolume);

		}

		protected override void GomOnBarUpdateDone()
		{
			dsClose.Set(totalvolume);
			dsHigh.Set(hi);
			dsLow.Set(lo);

			DeltaValue.Set(totalvolume);
		}

		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{
			if (Bars == null) return;

			if (useplot)
			{

				int lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
				int firstBar = this.FirstBarIndexPainted;

				min = Double.MaxValue;
				max = Double.MinValue;

				for (int index = firstBar; index <= lastBar; index++)
				{
					if ((index <= lastcalcbar) && (index >= Math.Max(1, startbar)))
					{
						min = Math.Min(min, dsLow.Get(index));
						max = Math.Max(max, dsHigh.Get(index));
					}
				}

				if ((max - min) < 1)
				{
					min -= 1;
					max += 1;
				}
			}

			else
				base.GetMinMaxValues(chartControl, ref  min, ref  max);

		}


		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null) return;

			if (useplot)
			{
				base.Plot(graphics, bounds, min, max);


				int lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
				int firstBar = this.FirstBarIndexPainted;

				Pen outlinePen = ChartControl.ChartStyle.Pen;
				
				int barPaintWidth =  (int) (1 + 2 *(ChartControl.ChartStyle.BarWidthUI - 1) + 2 * outlinePen.Width); //ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
				int barwidth = ChartControl.BarWidth;



				Color ColorNeutral = outlinePen.Color;

				Pen HiLoPen = (Pen)outlinePen.Clone();

				if (EnhanceHL)
					HiLoPen.Width *= 2;



				int penSize;

				if (ChartControl.ChartStyle.ChartStyleType == ChartStyleType.OHLC)
					penSize = Math.Max(0, barwidth - 2);
				else if (ChartControl.ChartStyle.ChartStyleType == ChartStyleType.HiLoBars)
					penSize = barwidth;
				else if (chart == GomCDChartType.NonCumulativeChart && ForceHiLo)
					penSize = ForcedHiLoBS;
				else
					penSize = 1;

				drawPen.Width = penSize;

				int x, yHigh, yClose, yOpen, yLow;
				int direction;

				//zero line
				int y0 = ChartControl.GetYByValue(this, 0.0);



				if ((chart == GomCDChartType.NonCumulativeChart) || ReinitSession)
					graphics.DrawLine(new Pen(Color.Blue), bounds.X, y0, bounds.X + bounds.Width, y0);

				for (int index = firstBar; index <= lastBar; index++)
				{

					direction = 0;

					if ((index <= lastcalcbar) && (index >= Math.Max(1, startbar)))
					{
						x = ChartControl.GetXByBarIdx(BarsArray[0], index);
						yHigh = ChartControl.GetYByValue(this, dsHigh.Get(index));
						yClose = ChartControl.GetYByValue(this, dsClose.Get(index));
						yOpen = ChartControl.GetYByValue(this, dsOpen.Get(index));
						yLow = ChartControl.GetYByValue(this, dsLow.Get(index));


						if (PtType == GomPaintType.StrongUpDown)
						{
							if (dsClose.Get(index) < dsLow.Get(index - 1))
								direction = -1;
							else if (dsClose.Get(index) > dsHigh.Get(index - 1))
								direction = 1;
						}
						else if (PtType == GomPaintType.UpDown)
						{
							if (dsClose.Get(index) < dsOpen.Get(index))
								direction = -1;
							else if (dsClose.Get(index) > dsOpen.Get(index))
								direction = 1;
						}


						if (direction == 1)
						{
							drawPen.Color = ChartControl.UpColor;
						}
						else if (direction == -1)
						{
							drawPen.Color = ChartControl.DownColor;
						}
						else
						{
							drawPen.Color = ColorNeutral;
						}

						drawBrush.Color = drawPen.Color;


						if ((ChartControl.ChartStyle.ChartStyleType == ChartStyleType.HiLoBars) || (chart == GomCDChartType.NonCumulativeChart && ForceHiLo))
						{
							graphics.DrawLine(drawPen, x, yHigh, x, yLow);
						}

						else if (ChartControl.ChartStyle.ChartStyleType == ChartStyleType.CandleStick)
						{
							graphics.DrawLine(HiLoPen, x, yLow, x, yHigh);

							if (yClose == yOpen)
								graphics.DrawLine(outlinePen, x - barwidth - outlinePen.Width, yClose, x + barwidth + outlinePen.Width, yClose);

							else
							{
								graphics.FillRectangle(drawBrush, x - barPaintWidth / 2 , Math.Min(yClose, yOpen) + 1, barPaintWidth , Math.Abs(yClose - yOpen));
								
								if (ShowOutline)
								{
									//	graphics.FillRectangle(neutralBrush,x-barwidth-outlinepenwidth,Math.Min(yClose,yOpen)+1,2*(barwidth+outlinepenwidth)+1,Math.Abs(yClose-yOpen)-1);
									//	graphics.FillRectangle(drawBrush,x-barwidth,Math.Min(yClose,yOpen)+1,2*barwidth+1,Math.Abs(yClose-yOpen)-1);
									//graphics.DrawRectangle(outlinePen, x - barwidth - outlinePen.Width / 2, Math.Min(yClose, yOpen), 2 * (barwidth) + outlinePen.Width + 1, Math.Abs(yClose - yOpen));

									graphics.DrawRectangle(outlinePen, x - (barPaintWidth / 2) + (outlinePen.Width / 2), Math.Min(yClose, yOpen), barPaintWidth - outlinePen.Width, Math.Abs(yClose - yOpen));

								}

							}
						}

						else
						{
							graphics.DrawLine(drawPen, x, yLow + penSize / 2, x, yHigh - penSize / 2);
							graphics.DrawLine(drawPen, x, yClose, x + barwidth, yClose);
							graphics.DrawLine(drawPen, x - barwidth, yOpen, x, yOpen);
						}
					}
				}
			}
			else
				base.Plot(graphics, bounds, min, max);

		}


		protected override void GomOnTermination()
		{
			if (dsClose != null)
				dsClose.Dispose();

			if (dsOpen != null)
				dsOpen.Dispose();

			if (dsHigh != null)
				dsHigh.Dispose();

			if (dsLow != null)
				dsLow.Dispose();
		}

		#region Properties

		[Description("UpDownTick : volume is up if price>lastprice, down if price<lastprice.\nUpDownTickWithContinuation : volume is up if price>lastprice or\nprice=lastprice and last direction was up, same for downside")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Delta:Calculation Mode")]
		public GomCDCalculationModeType CalcMode
		{
			get { return base.CalcMode; }
			set { base.CalcMode = value; }
		}
		
		
		[Description("Type of delta calculation - Cumulative or Non")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("GomCD:Delta Calculation")]
		public GomCDChartType Chart
		{
			get { return chart; }
			set { chart = value; }
		}
		[Description("Paint Type")]
		[Category("Settings:GomCD")]
		[Gui.Design.DisplayNameAttribute("Paint Type")]
		public GomPaintType ptType
		{
			get { return PtType; }
			set { PtType = value; }
		}

		[Description("CandleSticks : Show Outline")]
		[Category("Settings:GomCD")]
		[Gui.Design.DisplayNameAttribute("CandleSticks : Show Outline")]
		public bool showOutline
		{
			get { return ShowOutline; }
			set { ShowOutline = value; }
		}

		[Description("CandleSticks : Enhance HiLo Bar")]
		[Category("Settings:GomCD")]
		[Gui.Design.DisplayNameAttribute("CandleSticks : Enhance HiLo Bar")]
		public bool enhanceHL
		{
			get { return EnhanceHL; }
			set { EnhanceHL = value; }
		}


		[Description("Force HiLo on noncumulative chart")]
		[Category("Settings:GomCD")]
		[Gui.Design.DisplayNameAttribute("Force HiLo for Non cumulative")]
		public bool forceHiLo
		{
			get { return ForceHiLo; }
			set { ForceHiLo = value; }
		}

		[Description("Size of HiLo bars when force hilo mode is used")]
		[Category("Settings:GomCD")]
		[Gui.Design.DisplayNameAttribute("Forced HiLo Bar Size")]
		public int forcedHiLoBS
		{
			get { return ForcedHiLoBS; }
			set { ForcedHiLoBS = value; }
		}

		[Description("Reinit on session break")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("GomCD:Reinit on session break")]
		public bool reinitSession
		{
			get { return ReinitSession; }
			set { ReinitSession = value; }
		}

		
		

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries DeltaValue
		{
			get { return Values[0]; }
		}


		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries DeltaClose
		{
			get
			{
				return dsClose;
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries DeltaOpen
		{
			get
			{
				return dsOpen;
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries DeltaHigh
		{
			get
			{
				return dsHigh;
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries DeltaLow
		{
			get
			{
				return dsLow;
			}
		}


		#endregion
			
	}
	
	//legacy constructor
	public partial class Indicator:IndicatorBase
	{
		public GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, string fileFormat, GomFilterModeType filterMode, int filterSize, bool reinitSession)
        {
            GomCD indy = GomCD(Input,calcMode,chart,reinitSession);
			indy.FileFormat=fileFormat;
            indy.FilterMode = filterMode;
            indy.FilterSize = filterSize;


            return indy;
        }
		
		public GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, string fileFormat, Gom.FileModeType fileModeType,GomFilterModeType filterMode, int filterSize, bool reinitSession)
        {
            GomCD indy = GomCD(calcMode,chart,fileFormat,filterMode,filterSize,reinitSession);
			indy.FileMode=fileModeType;

            return indy;
        }
		
		public  GomCD GomCD(GomCDChartType chart, bool reinitSession)
        {
            GomCD indy = GomCD(GomCDCalculationModeType.BidAsk,chart,reinitSession);
			
            return indy;
        }
	}



}

namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		
		public Indicator.GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, string fileFormat, GomFilterModeType filterMode, int filterSize, bool reinitSession)
        {
            Indicator.GomCD indy = _indicator.GomCD(Input,calcMode,chart,reinitSession);
			indy.FileFormat=fileFormat;
            indy.FilterMode = filterMode;
            indy.FilterSize = filterSize;

            return indy;
        }

		public Indicator.GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, string fileFormat, Gom.FileModeType fileModeType,GomFilterModeType filterMode, int filterSize, bool reinitSession)
        {
            Indicator.GomCD indy = _indicator.GomCD(calcMode,chart,fileFormat,filterMode,filterSize,reinitSession);
			indy.FileMode=fileModeType;

            return indy;
        }
		
		
		public Indicator.GomCD GomCD(GomCDChartType chart, bool reinitSession)
        {
            return _indicator.GomCD(Input, GomCDCalculationModeType.BidAsk, chart, reinitSession);
        }	
		
		
	}
}
	

public enum GomPaintType
{
	None,
	UpDown,
	StrongUpDown
}

//Horrible hack that allows GomFileFormat.Binary to still work with new model
static public class GomFileFormat
{
	static public string Binary = "Binary";
}


#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private GomCD[] cacheGomCD = null;

        private static GomCD checkGomCD = new GomCD();

        /// <summary>
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        public GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            return GomCD(Input, calcMode, chart, reinitSession);
        }

        /// <summary>
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        public GomCD GomCD(Data.IDataSeries input, GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            if (cacheGomCD != null)
                for (int idx = 0; idx < cacheGomCD.Length; idx++)
                    if (cacheGomCD[idx].CalcMode == calcMode && cacheGomCD[idx].Chart == chart && cacheGomCD[idx].reinitSession == reinitSession && cacheGomCD[idx].EqualsInput(input))
                        return cacheGomCD[idx];

            lock (checkGomCD)
            {
                checkGomCD.CalcMode = calcMode;
                calcMode = checkGomCD.CalcMode;
                checkGomCD.Chart = chart;
                chart = checkGomCD.Chart;
                checkGomCD.reinitSession = reinitSession;
                reinitSession = checkGomCD.reinitSession;

                if (cacheGomCD != null)
                    for (int idx = 0; idx < cacheGomCD.Length; idx++)
                        if (cacheGomCD[idx].CalcMode == calcMode && cacheGomCD[idx].Chart == chart && cacheGomCD[idx].reinitSession == reinitSession && cacheGomCD[idx].EqualsInput(input))
                            return cacheGomCD[idx];

                GomCD indicator = new GomCD();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CalcMode = calcMode;
                indicator.Chart = chart;
                indicator.reinitSession = reinitSession;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomCD[] tmp = new GomCD[cacheGomCD == null ? 1 : cacheGomCD.Length + 1];
                if (cacheGomCD != null)
                    cacheGomCD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomCD = tmp;
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
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            return _indicator.GomCD(Input, calcMode, chart, reinitSession);
        }

        /// <summary>
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        public Indicator.GomCD GomCD(Data.IDataSeries input, GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            return _indicator.GomCD(input, calcMode, chart, reinitSession);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomCD GomCD(GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            return _indicator.GomCD(Input, calcMode, chart, reinitSession);
        }

        /// <summary>
        /// Gom Cumulative Delta
        /// </summary>
        /// <returns></returns>
        public Indicator.GomCD GomCD(Data.IDataSeries input, GomCDCalculationModeType calcMode, GomCDChartType chart, bool reinitSession)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomCD(input, calcMode, chart, reinitSession);
        }
    }
}
#endregion
