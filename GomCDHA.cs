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
#endregion

namespace NinjaTrader.Indicator
{

	[Description("GomCD Heikin Ashi")]
	public class GomCDHA : Indicator
	{
		#region Variables
		private Color barColorUp = Color.RoyalBlue;
		private Color barColorDown = Color.Red;
		private Color barColorWarn = Color.Yellow;
		private Color hiloBarColor = Color.Black;
		private int minCompBars = 0;
		private int maxCompBars = 6;
		private int shadowWidth = 3;
		private DataSeries ModVal;
		private IntSeries Direction;

		private GomCD gcd;

		private int startbar = -1;
        private int lastcalcbar = -1;


		#endregion

		protected override void Initialize()
		{
			Add(new Plot(Color.Transparent, PlotStyle.Line, "HAOpen"));
			Add(new Plot(Color.Transparent, PlotStyle.Line, "HAHigh"));
			Add(new Plot(Color.Transparent, PlotStyle.Line, "HALow"));
			Add(new Plot(Color.Transparent, PlotStyle.Line, "HAClose"));


			ModVal = new DataSeries(this, MaximumBarsLookBack.Infinite);
			Direction = new IntSeries(this, MaximumBarsLookBack.Infinite);

			PaintPriceMarkers = false;
			CalculateOnBarClose = false;
			Overlay = false;
			PriceTypeSupported = false;
			PlotsConfigurable = false;

		}


		void PlotChart()
		{
			if (MinCompBars > MaxCompBars)
				return;

			Values[3].Set((gcd.DeltaOpen[0] + gcd.DeltaHigh[0] + gcd.DeltaLow[0] + gcd.DeltaClose[0]) / 4); // Calculate the close
			Values[0].Set((Values[0][1] + Values[3][1]) / 2); // Calculate the open
			Values[1].Set(Math.Max(Math.Max(gcd.DeltaHigh[0], Values[0][0]), Values[3][0])); // Calculate the high
			Values[2].Set(Math.Min(Math.Min(gcd.DeltaLow[0], Values[0][0]), Values[3][0])); // Calculate the low

			double haMinDir = 0;
			double haMaxDir = 0;

			if (CurrentBar > MaxCompBars)
			{
				if (Values[3][0] > Values[0][0])
					ModVal.Set(1);
				else
					ModVal.Set(2);

				for (int i = 0; i < MinCompBars; i++)
				{
					if ((Values[0][0] <= Math.Max(Values[0][i], Values[3][i])) &&
						(Values[0][0] >= Math.Min(Values[0][i], Values[3][i])) &&
						(Values[3][0] <= Math.Max(Values[0][i], Values[3][i])) &&
						(Values[3][0] >= Math.Min(Values[0][i], Values[3][i])))
					{
						ModVal.Set(ModVal[i]);
						//break;
					}
				}
				haMinDir = ModVal[0];

				if (Values[3][0] > Values[0][0])
					ModVal.Set(1);
				else
					ModVal.Set(2);

				for (int i = 0; i < MaxCompBars; i++)
				{
					if ((Values[0][0] <= Math.Max(Values[0][i], Values[3][i])) &&
						(Values[0][0] >= Math.Min(Values[0][i], Values[3][i])) &&
						(Values[3][0] <= Math.Max(Values[0][i], Values[3][i])) &&
						(Values[3][0] >= Math.Min(Values[0][i], Values[3][i])))
					{
						ModVal.Set(ModVal[i]);
						//break;
					}
				}
				haMaxDir = ModVal[0];
			}

			if (haMinDir == 1 && haMaxDir == 1)
				Direction[0] = 1;

			if (haMinDir == 2 && haMaxDir == 2)
				Direction[0] = -1;

			if (haMinDir != haMaxDir)
				Direction[0] = 0;



		}

		protected override void OnStartUp()
		{
			gcd = GomCD(GomCDCalculationModeType.BidAsk, GomCDChartType.CumulativeChart, "Binary",Gom.FileModeType.OnePerDay, GomFilterModeType.None, 1, false);			
		}


		protected override void OnBarUpdate()
		{

			double dummy = gcd.DeltaValue[0];

			if (CurrentBar == 0)
			{
				Values[0].Set(0);
				Values[1].Set(0);
				Values[2].Set(0);
				Values[3].Set(0);
				ModVal.Set(0);
				return;
			}

			PlotChart();

			if (startbar == -1)
				startbar = CurrentBar;

			lastcalcbar = CurrentBar;

		}

		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{
			if (Bars == null) return;

			int lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
			int firstBar = this.FirstBarIndexPainted;

			min = Double.MaxValue;
			max = Double.MinValue;

			for (int index = firstBar; index <= lastBar; index++)
			{
				if ((index <= lastcalcbar) && (index >= Math.Max(1, startbar)))
				{
					min = Math.Min(min, HALow.Get(index));
					max = Math.Max(max, HAHigh.Get(index));
				}
			}

			if ((max - min) < 1)
			{
				min -= 1;
				max += 1;
			}


		}



		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			base.Plot(graphics, bounds, min, max);

			if (Bars == null) return;

			int lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
			int firstBar = this.FirstBarIndexPainted;

			int x, yHigh, yClose, yOpen, yLow;

			using (Pen drawPen = new Pen(Color.Transparent, ShadowWidth), HiLoPen = new Pen(hiloBarColor))
			{
				for (int index = firstBar; index <= lastBar; index++)
				{

					if ((index <= lastcalcbar) && (index >= Math.Max(1, startbar)))
					{
						x = ChartControl.GetXByBarIdx(BarsArray[0], index);
						yHigh = ChartControl.GetYByValue(this, HAHigh.Get(index));
						yClose = ChartControl.GetYByValue(this, HAClose.Get(index));
						yOpen = ChartControl.GetYByValue(this, HAOpen.Get(index));
						yLow = ChartControl.GetYByValue(this, HALow.Get(index));

						if (Direction.Get(index) > 0)
							drawPen.Color = BarColorUp;
						else if (Direction.Get(index) < 0)
							drawPen.Color = BarColorDown;
						else
							drawPen.Color = BarColorWarn;

						graphics.DrawLine(HiLoPen, x, yHigh, x, yLow);
						graphics.DrawLine(drawPen, x, yOpen, x, yClose);

					}
				}
			}
		}


		protected override void OnTermination()
		{
			if (HAClose != null)
				HAClose.Dispose();

			if (HAOpen != null)
				HAOpen.Dispose();

			if (HAHigh != null)
				HAHigh.Dispose();

			if (HALow != null)
				HALow.Dispose();
		}



		#region Properties
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of down bars.")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Down color")]
		public Color BarColorDown
		{
			get { return barColorDown; }
			set { barColorDown = value; }
		}
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of HiLo Bar")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("HiLo color")]
		public Color HiLoColor
		{
			get { return hiloBarColor; }
			set { hiloBarColor = value; }
		}
		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string HiLoColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(hiloBarColor); }
			set { hiloBarColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string BarColorDownSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorDown); }
			set { barColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of up bars.")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Up color")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of warn bars.")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Warn color")]
		public Color BarColorWarn
		{
			get { return barColorWarn; }
			set { barColorWarn = value; }
		}
		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string BarColorWarnSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorWarn); }
			set { barColorWarn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string BarColorUpSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorUp); }
			set { barColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Width of shadow line.")]
		[Category("Settings")]
		[Gui.Design.DisplayNameAttribute("Shadow width")]
		public int ShadowWidth
		{
			get { return shadowWidth; }
			set { shadowWidth = Math.Max(value, 1); }
		}

		/// <summary>
		/// </summary>
		[Description("HA : Min Compare Bars.")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Min Compare Bars")]
		public int MinCompBars
		{
			get { return minCompBars; }
			set { minCompBars = value; }
		}
		/// <summary>
		/// </summary>
		[Description("HA : Max Compare Bars.")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Max Compare Bars")]
		public int MaxCompBars
		{
			get { return maxCompBars; }
			set { maxCompBars = value; }
		}

		/// <summary>
		/// Gets the ModHA2 Open value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries HAOpen
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Gets the ModHA2 High value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries HAHigh
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// Gets the ModHA2 Low value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries HALow
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// Gets the ModHA2 Close value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries HAClose
		{
			get { return Values[3]; }
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
        private GomCDHA[] cacheGomCDHA = null;

        private static GomCDHA checkGomCDHA = new GomCDHA();

        /// <summary>
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        public GomCDHA GomCDHA(int maxCompBars, int minCompBars)
        {
            return GomCDHA(Input, maxCompBars, minCompBars);
        }

        /// <summary>
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        public GomCDHA GomCDHA(Data.IDataSeries input, int maxCompBars, int minCompBars)
        {
            if (cacheGomCDHA != null)
                for (int idx = 0; idx < cacheGomCDHA.Length; idx++)
                    if (cacheGomCDHA[idx].MaxCompBars == maxCompBars && cacheGomCDHA[idx].MinCompBars == minCompBars && cacheGomCDHA[idx].EqualsInput(input))
                        return cacheGomCDHA[idx];

            lock (checkGomCDHA)
            {
                checkGomCDHA.MaxCompBars = maxCompBars;
                maxCompBars = checkGomCDHA.MaxCompBars;
                checkGomCDHA.MinCompBars = minCompBars;
                minCompBars = checkGomCDHA.MinCompBars;

                if (cacheGomCDHA != null)
                    for (int idx = 0; idx < cacheGomCDHA.Length; idx++)
                        if (cacheGomCDHA[idx].MaxCompBars == maxCompBars && cacheGomCDHA[idx].MinCompBars == minCompBars && cacheGomCDHA[idx].EqualsInput(input))
                            return cacheGomCDHA[idx];

                GomCDHA indicator = new GomCDHA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MaxCompBars = maxCompBars;
                indicator.MinCompBars = minCompBars;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomCDHA[] tmp = new GomCDHA[cacheGomCDHA == null ? 1 : cacheGomCDHA.Length + 1];
                if (cacheGomCDHA != null)
                    cacheGomCDHA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomCDHA = tmp;
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
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomCDHA GomCDHA(int maxCompBars, int minCompBars)
        {
            return _indicator.GomCDHA(Input, maxCompBars, minCompBars);
        }

        /// <summary>
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        public Indicator.GomCDHA GomCDHA(Data.IDataSeries input, int maxCompBars, int minCompBars)
        {
            return _indicator.GomCDHA(input, maxCompBars, minCompBars);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomCDHA GomCDHA(int maxCompBars, int minCompBars)
        {
            return _indicator.GomCDHA(Input, maxCompBars, minCompBars);
        }

        /// <summary>
        /// GomCD Heikin Ashi
        /// </summary>
        /// <returns></returns>
        public Indicator.GomCDHA GomCDHA(Data.IDataSeries input, int maxCompBars, int minCompBars)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomCDHA(input, maxCompBars, minCompBars);
        }
    }
}
#endregion
