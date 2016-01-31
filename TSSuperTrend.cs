/// <summary>
/// TSSuperTrend Indicator
/// Version 2.2
/// Visual options for bar colors added by Elliott Wave 12/08.
/// Version 2.3 
/// Fixed VWMA issue
/// Added Sound Alerts
/// </summary>
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using TSSuperTrend.Utility;

namespace NinjaTrader.Indicator
{
    [Description("TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)")]
    public class TSSuperTrend : Indicator
    {
        private int _length = 14;
        private double _multiplier = 2.618;
        private bool _showArrows;
        private bool _colorBars;
        private BoolSeries _trend;
        private IDataSeries _avg;
        private double _offset;
        private MovingAverageType _maType = MovingAverageType.HMA;
        private SuperTrendMode _smode = SuperTrendMode.ATR;
        private int _smooth = 14;
        private Color _barColorUp = Color.Blue;
        private Color _barColorDown = Color.Red;
        private Color _tempColor;
        private Color _prevColor;
        private double _th;
        private double _tl = double.MaxValue;
        private bool _playAlert;
        private string _longAlert = "Alert4.wav";
        private string _shortAlert = "Alert4.wav";
        private int _thisbar = -1;

        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Hash, "UpTrend"));
            Add(new Plot(Color.Red, PlotStyle.Hash, "DownTrend"));
            Overlay = true;
            _trend = new BoolSeries(this);
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1)
            {
                if (_smooth > 1 && _avg == null)
                    switch (_maType)
                    {
                        case MovingAverageType.SMA:
                            _avg = SMA(Input, _smooth);
                            break;
                        case MovingAverageType.SMMA:
                            _avg = SMMA(Input, _smooth);
                            break;
                        case MovingAverageType.TMA:
                            _avg = TMA(Input, _smooth);
                            break;
                        case MovingAverageType.WMA:
                            _avg = WMA(Input, _smooth);
                            break;
                        case MovingAverageType.VWMA:
                            _avg = VWMA(Input, _smooth);
                            break;
                        case MovingAverageType.TEMA:
                            _avg = TEMA(Input, _smooth);
                            break;
                        case MovingAverageType.HMA:
                            _avg = HMA(Input, _smooth);
                            break;
                        case MovingAverageType.VMA:
                            _avg = VMA(Input, _smooth, _smooth);
                            break;
						case MovingAverageType.JMA:
							_avg = JurikJMA(Input, 0, _smooth);
							break;
                        default:
                            _avg = EMA(Input, _smooth);
                            break;
                    }
                else
                    _avg = Input;

                _trend.Set(true);
                UpTrend.Set(Input[0]);
                DownTrend.Set(Input[0]);
                return;
            }

            switch (_smode)
            {
                case SuperTrendMode.ATR:
                    _offset = ATR(_length)[0] * Multiplier;
                    break;
                case SuperTrendMode.Adaptive:
                    _offset = ATR(_length)[0] * HomodyneDiscriminator(Input)[0] / 10;
                    break;
                default:
                    _offset = Dtt(_length, Multiplier);
                    break;
            }

            if (FirstTickOfBar)
                _prevColor = _tempColor;

            _trend.Set(Close[0] > DownTrend[1] ? true : Close[0] < UpTrend[1] ? false : _trend[1]);

            if (_trend[0] && !_trend[1])
            {
                _th = High[0];
                UpTrend.Set(Math.Max(_avg[0] - _offset, _tl));
                if (Plots[0].PlotStyle == PlotStyle.Line) UpTrend.Set(1, DownTrend[1]);
                _tempColor = _barColorUp;
                if (ShowArrows)
                    DrawArrowUp(CurrentBar.ToString(), true, 0, UpTrend[0] - TickSize, _barColorUp);
                if(PlayAlert && _thisbar != CurrentBar)
                {
                    _thisbar = CurrentBar;
                    PlaySound(LongAlert);
                }
            }
            else
                if (!_trend[0] && _trend[1])
                {
                    _tl = Low[0];
                    DownTrend.Set(Math.Min(_avg[0] + _offset, _th));
                    if (Plots[1].PlotStyle == PlotStyle.Line) DownTrend.Set(1, UpTrend[1]);
                    _tempColor = _barColorDown;
                    if (ShowArrows)
                        DrawArrowDown(CurrentBar.ToString(), true, 0, DownTrend[0] + TickSize, _barColorDown);
                    if (PlayAlert && _thisbar != CurrentBar)
                    {
                        _thisbar = CurrentBar;
                        PlaySound(ShortAlert);
                    }
                }
                else
                {
                    if (_trend[0])
                    {
                        UpTrend.Set((_avg[0] - _offset) > UpTrend[1] ? (_avg[0] - _offset) : UpTrend[1]);
                        _th = Math.Max(_th, High[0]);
                    }
                    else
                    {
                        DownTrend.Set((_avg[0] + _offset) < DownTrend[1] ? (_avg[0] + _offset) : DownTrend[1]);
                        _tl = Math.Min(_tl, Low[0]);
                    }
                    RemoveDrawObject(CurrentBar.ToString());
                    _tempColor = _prevColor;
                }

            if (!_colorBars) 
                return;

            CandleOutlineColor = _tempColor;

            BarColor = Open[0] < Close[0] && ChartControl.ChartStyleType == ChartStyleType.CandleStick
                           ? Color.Transparent
                           : _tempColor;
        }

        private double Dtt(int nDay, double mult)
        {
            double hh = MAX(High, nDay)[0];
            double hc = MAX(Close, nDay)[0];
            double ll = MIN(Low, nDay)[0];
            double lc = MIN(Close, nDay)[0];
            return mult * Math.Max((hh - lc), (hc - ll));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpTrend
        {
            get
            {
                Update();
                return Values[0];
            }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DownTrend
        {
            get
            {
                Update();
                return Values[1];
            }
        }

        [Description("SuperTrendMode")]
        [Category("Parameters")]
        [Gui.Design.DisplayName("01. SuperTrend Mode")]
        public SuperTrendMode StMode
        {
            get { return _smode; }
            set { _smode = value; }
        }

        [Description("ATR/DT Period")]
        [Category("Parameters")]
        [Gui.Design.DisplayName("02. Period")]
        public int Length
        {
            get { return _length; }
            set { _length = Math.Max(1, value); }
        }

        [Description("ATR Multiplier")]
        [Category("Parameters")]
        [Gui.Design.DisplayName("03. Multiplier")]
        public double Multiplier
        {
            get { return _multiplier; }
            set { _multiplier = Math.Max(0.0001, value); }
        }

        [Description("Moving Average Type for smoothing")]
        [Category("Parameters")]
        [Gui.Design.DisplayName("04. Moving Average Type")]
        public MovingAverageType MaType
        {
            get { return _maType; }
            set { _maType = value; }
        }

        [Description("Smoothing Period")]
        [Category("Parameters")]
        [Gui.Design.DisplayName("05. SmoothingPeriod (MA)")]
        public int Smooth
        {
            get { return _smooth; }
            set { _smooth = Math.Max(1, value); }
        }

        [Description("Show Arrows when Trendline is violated?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("01. Show Arrows?")]
        public bool ShowArrows
        {
            get { return _showArrows; }
            set { _showArrows = value; }
        }

        [Description("Color the bars in the direction of the trend?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("02. Color Bars?")]
        public bool ColorBars
        {
            get { return _colorBars; }
            set { _colorBars = value; }
        }

        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("03. Up color")]
        public Color BarColorUp
        {
            get { return _barColorUp; }
            set { _barColorUp = value; }
        }

        [Browsable(false)]
        public string BarColorUpSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(_barColorUp); }
            set { _barColorUp = Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("04. Down color")]
        public Color BarColorDown
        {
            get { return _barColorDown; }
            set { _barColorDown = value; }
        }

        [Browsable(false)]
        public string BarColorDownSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(_barColorDown); }
            set { _barColorDown = Gui.Design.SerializableColor.FromString(value); }
        }
        [Browsable(false)]
        public BoolSeries Trend
        {
            get
            {
                Update();
                return _trend;
            }
        }

        [Description("Play Alert")]
        [Category("Sounds")]
        [Gui.Design.DisplayName("01. Play Alert?")]
        public bool PlayAlert
        {
            get { return _playAlert; }
            set { _playAlert = value; }
        }

        [Description("File Name for long alert")]
        [Category("Sounds")]
        [Gui.Design.DisplayName("02. Long Alert")]
        public string LongAlert
        {
            get { return _longAlert; }
            set { _longAlert = value; }
        }

        [Description("File Name for short alert")]
        [Category("Sounds")]
        [Gui.Design.DisplayName("03. Short Alert")]
        public string ShortAlert
        {
            get { return _shortAlert; }
            set { _shortAlert = value; }
        }
        #endregion
    }
}

namespace TSSuperTrend.Utility
{
    public enum SuperTrendMode
    {
        ATR,
        DualThrust,
        Adaptive
    }

    public enum MovingAverageType
    {
        SMA,
        SMMA,
        TMA,
        WMA,
        VWMA,
        TEMA,
        HMA,
        EMA,
        VMA,
		JMA
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private TSSuperTrend[] cacheTSSuperTrend = null;

        private static TSSuperTrend checkTSSuperTrend = new TSSuperTrend();

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        public TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        public TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            if (cacheTSSuperTrend != null)
                for (int idx = 0; idx < cacheTSSuperTrend.Length; idx++)
                    if (cacheTSSuperTrend[idx].Length == length && cacheTSSuperTrend[idx].MaType == maType && Math.Abs(cacheTSSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheTSSuperTrend[idx].Smooth == smooth && cacheTSSuperTrend[idx].StMode == stMode && cacheTSSuperTrend[idx].EqualsInput(input))
                        return cacheTSSuperTrend[idx];

            lock (checkTSSuperTrend)
            {
                checkTSSuperTrend.Length = length;
                length = checkTSSuperTrend.Length;
                checkTSSuperTrend.MaType = maType;
                maType = checkTSSuperTrend.MaType;
                checkTSSuperTrend.Multiplier = multiplier;
                multiplier = checkTSSuperTrend.Multiplier;
                checkTSSuperTrend.Smooth = smooth;
                smooth = checkTSSuperTrend.Smooth;
                checkTSSuperTrend.StMode = stMode;
                stMode = checkTSSuperTrend.StMode;

                if (cacheTSSuperTrend != null)
                    for (int idx = 0; idx < cacheTSSuperTrend.Length; idx++)
                        if (cacheTSSuperTrend[idx].Length == length && cacheTSSuperTrend[idx].MaType == maType && Math.Abs(cacheTSSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheTSSuperTrend[idx].Smooth == smooth && cacheTSSuperTrend[idx].StMode == stMode && cacheTSSuperTrend[idx].EqualsInput(input))
                            return cacheTSSuperTrend[idx];

                TSSuperTrend indicator = new TSSuperTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.MaType = maType;
                indicator.Multiplier = multiplier;
                indicator.Smooth = smooth;
                indicator.StMode = stMode;
                Indicators.Add(indicator);
                indicator.SetUp();

                TSSuperTrend[] tmp = new TSSuperTrend[cacheTSSuperTrend == null ? 1 : cacheTSSuperTrend.Length + 1];
                if (cacheTSSuperTrend != null)
                    cacheTSSuperTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTSSuperTrend = tmp;
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
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        public Indicator.TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(input, length, maType, multiplier, smooth, stMode);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSSuperTrend TSSuperTrend(int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            return _indicator.TSSuperTrend(Input, length, maType, multiplier, smooth, stMode);
        }

        /// <summary>
        /// TSSuperTrend Indicator developed by TradingStudies.com (Vertsion 2.3)
        /// </summary>
        /// <returns></returns>
        public Indicator.TSSuperTrend TSSuperTrend(Data.IDataSeries input, int length, MovingAverageType maType, double multiplier, int smooth, SuperTrendMode stMode)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSSuperTrend(input, length, maType, multiplier, smooth, stMode);
        }
    }
}
#endregion
