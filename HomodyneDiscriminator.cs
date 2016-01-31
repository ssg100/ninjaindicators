using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
    [Description("")]
    public class HomodyneDiscriminator : Indicator
    {
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.ForestGreen), PlotStyle.Line, "Homodyne"));
			Add(new Line(Color.FromKnownColor(KnownColor.Red), 16, "Zero"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= true;
        }

        protected override void OnBarUpdate()
        {
            Homodyne.Set(HilbertTransform(Input,0).CycleSmoothPeriod[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Homodyne
        {
            get { return Values[0]; }
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
        private HomodyneDiscriminator[] cacheHomodyneDiscriminator = null;

        private static HomodyneDiscriminator checkHomodyneDiscriminator = new HomodyneDiscriminator();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HomodyneDiscriminator HomodyneDiscriminator()
        {
            return HomodyneDiscriminator(Input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HomodyneDiscriminator HomodyneDiscriminator(Data.IDataSeries input)
        {
            if (cacheHomodyneDiscriminator != null)
                for (int idx = 0; idx < cacheHomodyneDiscriminator.Length; idx++)
                    if (cacheHomodyneDiscriminator[idx].EqualsInput(input))
                        return cacheHomodyneDiscriminator[idx];

            lock (checkHomodyneDiscriminator)
            {
                if (cacheHomodyneDiscriminator != null)
                    for (int idx = 0; idx < cacheHomodyneDiscriminator.Length; idx++)
                        if (cacheHomodyneDiscriminator[idx].EqualsInput(input))
                            return cacheHomodyneDiscriminator[idx];

                HomodyneDiscriminator indicator = new HomodyneDiscriminator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                HomodyneDiscriminator[] tmp = new HomodyneDiscriminator[cacheHomodyneDiscriminator == null ? 1 : cacheHomodyneDiscriminator.Length + 1];
                if (cacheHomodyneDiscriminator != null)
                    cacheHomodyneDiscriminator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHomodyneDiscriminator = tmp;
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
        /// 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HomodyneDiscriminator HomodyneDiscriminator()
        {
            return _indicator.HomodyneDiscriminator(Input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.HomodyneDiscriminator HomodyneDiscriminator(Data.IDataSeries input)
        {
            return _indicator.HomodyneDiscriminator(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HomodyneDiscriminator HomodyneDiscriminator()
        {
            return _indicator.HomodyneDiscriminator(Input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Indicator.HomodyneDiscriminator HomodyneDiscriminator(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HomodyneDiscriminator(input);
        }
    }
}
#endregion
