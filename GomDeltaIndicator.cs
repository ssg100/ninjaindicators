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

	/// <summary>
    /// Gom Delta Indicator
    /// </summary>
    [Description("Base Class. Do not instantiate")]
	public class GomDeltaIndicator : GomRecorderIndicator
	{
		#region Variables
		private GomCDCalculationModeType calcMode = GomCDCalculationModeType.BidAsk;
		private bool backupMode = true;
		private GomFilterModeType filterMode = GomFilterModeType.None;
		private int filterSize = 1;

		private double lastprice = 0;
		private int lastdirection = 0;
		private bool startlookingforreversal = false;
		#endregion

		protected int CalcDelta(Gom.MarketDataType e)
		{
			return CalcDelta(e.TickType,e.Price,e.Volume,calcMode,backupMode,filterSize,filterMode);
		}
		
		protected int CalcDelta(TickTypeEnum tickType, double price, int volume)
		{
			return CalcDelta(tickType, price, volume, calcMode, backupMode, filterSize, filterMode);
		}

		private int CalcDelta(TickTypeEnum tickType, double price, int volume, GomCDCalculationModeType calcmode, bool backupmode, int filtersize, GomFilterModeType filtermode)
		{
			int delta = 0;
			int direction = lastdirection;


			if ((calcmode == GomCDCalculationModeType.BidAsk) && (tickType != TickTypeEnum.Unknown) && (tickType != TickTypeEnum.BetweenBidAsk))
			{
				if ((tickType == TickTypeEnum.BelowBid) || (tickType == TickTypeEnum.AtBid))
					delta = -volume;
				else if ((tickType == TickTypeEnum.AboveAsk) || (tickType == TickTypeEnum.AtAsk))
					delta = volume;
			}
			else if (calcmode == GomCDCalculationModeType.UpDownTick)
			{
				if (lastprice != 0)
				{
					if (price > lastprice) delta = volume;
					if (price < lastprice) delta = -volume;
				}
			}
			else if ((calcmode == GomCDCalculationModeType.UpDownTickWithContinuation) ||(calcmode == GomCDCalculationModeType.UpDownOneTickWithContinuation)|| ((calcmode == GomCDCalculationModeType.BidAsk) && (backupmode == true)))
			{
				if (price > lastprice)  //normal uptick/dn tick
					direction = 1;
				else if (price < lastprice)
					direction = -1;

				if (calcmode == GomCDCalculationModeType.UpDownOneTickWithContinuation)
					delta=direction;
				else
					delta = direction * volume;
			}

			// added	

			else if ((calcmode == GomCDCalculationModeType.Hybrid))
			{

				if (price > lastprice)  //normal uptick/dn tick
				{
					direction = 1;
					//price changed, we reinit the startlookingforreversal bool.
					startlookingforreversal = false;
				}
				else if (price < lastprice)
				{
					direction = -1;
					startlookingforreversal = false;
				}


				if (!startlookingforreversal)
					if (direction == 1)
						//if going up, we want to be hitting bid to be able to start to spot reversals (hitting the ask)
						startlookingforreversal = (tickType == TickTypeEnum.AtBid) || (tickType == TickTypeEnum.BelowBid);
					else
						startlookingforreversal = (tickType == TickTypeEnum.AtAsk) || (tickType == TickTypeEnum.AboveAsk);

				//what happens when price is same
				if (price == lastprice)
				{
					//if going up, and we have already hit the bid (startlookingforreversal is true) at a price level, 
					// and start hitting the ask, let's reverse

					if ((direction == 1) && startlookingforreversal && ((tickType == TickTypeEnum.AtAsk) || (tickType == TickTypeEnum.BetweenBidAsk)))
						direction = -1;

					else if ((direction == -1) && startlookingforreversal && ((tickType == TickTypeEnum.AtBid) || (tickType == TickTypeEnum.BetweenBidAsk)))
						direction = 1;	//buyers take control of ask
				}


				delta = direction * volume;

			}

			lastprice = price;
			lastdirection = direction;

			if ((filtermode == GomFilterModeType.OnlyLargerThan) && (volume <= filtersize))
				delta = 0;

			if ((filtermode == GomFilterModeType.OnlySmallerThan) && (volume >= filtersize))
				delta = 0;

			return delta;

		}


		#region Properties

		[Description("UpDownTick : volume is up if price>lastprice, down if price<lastprice.\nUpDownTickWithContinuation : volume is up if price>lastprice or\nprice=lastprice and last direction was up, same for downside")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Delta:Calculation Mode")]
		public GomCDCalculationModeType CalcMode
		{
			get { return calcMode; }
			set { calcMode = value; }
		}

		[Description("Volume Filter")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Delta:Volume Filter Size")]
		public int FilterSize
		{
			get { return filterSize; }
			set { filterSize = value; }
		}

		[Description("If Bid/ask data invalid, do we use updownwithcontinuation ?")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Delta:Volume UpDownTick Completion")]
		public bool BackupMode
		{
			get { return backupMode; }
			set { backupMode = value; }
		}

		[Description("Filter Mode")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Delta:Size Filter Mode")]
		public GomFilterModeType FilterMode
		{
			get { return filterMode; }
			set { filterMode = value; }
		}
		#endregion
	}
}

public enum GomCDCalculationModeType
{
	BidAsk,
	UpDownTick,
	UpDownTickWithContinuation,
	Hybrid,
	UpDownOneTickWithContinuation
}


public enum GomCDChartType
{
	CumulativeChart,
	NonCumulativeChart,
}


public enum GomFilterModeType
{
	OnlyLargerThan,
	OnlySmallerThan,
	None
}



#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private GomDeltaIndicator[] cacheGomDeltaIndicator = null;

        private static GomDeltaIndicator checkGomDeltaIndicator = new GomDeltaIndicator();

        /// <summary>
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        public GomDeltaIndicator GomDeltaIndicator(bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            return GomDeltaIndicator(Input, backupMode, calcMode, filterMode, filterSize);
        }

        /// <summary>
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        public GomDeltaIndicator GomDeltaIndicator(Data.IDataSeries input, bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            if (cacheGomDeltaIndicator != null)
                for (int idx = 0; idx < cacheGomDeltaIndicator.Length; idx++)
                    if (cacheGomDeltaIndicator[idx].BackupMode == backupMode && cacheGomDeltaIndicator[idx].CalcMode == calcMode && cacheGomDeltaIndicator[idx].FilterMode == filterMode && cacheGomDeltaIndicator[idx].FilterSize == filterSize && cacheGomDeltaIndicator[idx].EqualsInput(input))
                        return cacheGomDeltaIndicator[idx];

            lock (checkGomDeltaIndicator)
            {
                checkGomDeltaIndicator.BackupMode = backupMode;
                backupMode = checkGomDeltaIndicator.BackupMode;
                checkGomDeltaIndicator.CalcMode = calcMode;
                calcMode = checkGomDeltaIndicator.CalcMode;
                checkGomDeltaIndicator.FilterMode = filterMode;
                filterMode = checkGomDeltaIndicator.FilterMode;
                checkGomDeltaIndicator.FilterSize = filterSize;
                filterSize = checkGomDeltaIndicator.FilterSize;

                if (cacheGomDeltaIndicator != null)
                    for (int idx = 0; idx < cacheGomDeltaIndicator.Length; idx++)
                        if (cacheGomDeltaIndicator[idx].BackupMode == backupMode && cacheGomDeltaIndicator[idx].CalcMode == calcMode && cacheGomDeltaIndicator[idx].FilterMode == filterMode && cacheGomDeltaIndicator[idx].FilterSize == filterSize && cacheGomDeltaIndicator[idx].EqualsInput(input))
                            return cacheGomDeltaIndicator[idx];

                GomDeltaIndicator indicator = new GomDeltaIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BackupMode = backupMode;
                indicator.CalcMode = calcMode;
                indicator.FilterMode = filterMode;
                indicator.FilterSize = filterSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomDeltaIndicator[] tmp = new GomDeltaIndicator[cacheGomDeltaIndicator == null ? 1 : cacheGomDeltaIndicator.Length + 1];
                if (cacheGomDeltaIndicator != null)
                    cacheGomDeltaIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomDeltaIndicator = tmp;
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
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaIndicator GomDeltaIndicator(bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            return _indicator.GomDeltaIndicator(Input, backupMode, calcMode, filterMode, filterSize);
        }

        /// <summary>
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaIndicator GomDeltaIndicator(Data.IDataSeries input, bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            return _indicator.GomDeltaIndicator(input, backupMode, calcMode, filterMode, filterSize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomDeltaIndicator GomDeltaIndicator(bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            return _indicator.GomDeltaIndicator(Input, backupMode, calcMode, filterMode, filterSize);
        }

        /// <summary>
        /// Base Class. Do not instantiate
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaIndicator GomDeltaIndicator(Data.IDataSeries input, bool backupMode, GomCDCalculationModeType calcMode, GomFilterModeType filterMode, int filterSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomDeltaIndicator(input, backupMode, calcMode, filterMode, filterSize);
        }
    }
}
#endregion
