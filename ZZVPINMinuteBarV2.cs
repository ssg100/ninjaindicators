#region Using declarations
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System.Runtime.InteropServices; // DLL support
using System.Collections.Generic;
#endregion

// !!!!!!!!!!!!!!!!!!!!!!!!!!!
// Version 2. Enhanced based on http://www.quantresearch.info/From%20PIN%20to%20VPIN.pdf
// !!!!!!!!!!!!!!!!!!!!!!!!!!!

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{

    /// <summary>
    /// VPIN indicator
    /// </summary>
    [Description("VPIN indicator")]
    public class ZZVPINMinuteBarV2 : Indicator
    {
		//public static extern void Analyze();
        #region Variables
        // Wizard generated variables
            private int volBucketSize = 31999; // Default setting for VolBucketSize (V variable)
            private int sampleSize = 30; // Default setting for SampleSize
			private int nWindowsize = 50;
			private int cdfsamplesize = 50;
			private DataSeries	deltaPrice;
			private DataSeries	vpinDataseries;
			private DataSeries	deltaPdivSigmaP;
			private double stddevVpin=0;
			private double avgVpin = 0;
			//private DataSeries  stdDev;
			private bool useRealtimeVol = false;
			private long lastTradeVol = 0;
			private double accumVol = 0;
			private long n = 0;   // n = number of samples
			private int currentBarSaved = 0;
			private int timeBarCounter = 5;  //counter for num of time bars inside prev vol bucket
			private int numTimeBarLastBucket = 6;	// use this to 
			/// </summary>
			private bool bucketFull = false;	// flag if bucket is full
			private double VolCounter = 0;
			private double VolBuy = 0;
			private double VolSell = 0;	//Vs
			private double cummDeltaVsVB = 0;  // |Vs - VB|
			private double VPINvalue = 0;			// VPIN final value
			private Queue qAccOI;
			private Queue queueAvgVpin;
			private double vpintemp=0;
			//private Array deltaVolArray;
			private bool initialBucketFull = false; // one time flag indicating first bucket full for the Queue
			private int basketCount = 0;
			private double accumulatedBuyVol;
			private double accumulatedSellVol;
			
			private double accOrderImbalance=0.0;
			private double prevBasketOI;	// marking first basket order imbalance
			private	double orderImbalance;
		
			private int basketTempCount=0; // count for accorderimbalance
		
        // User defined variables (add any user defined variables below)
        #endregion
		//public static extern void Mean();
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			accOrderImbalance=0.0;
			qAccOI = new Queue();
			queueAvgVpin = new Queue();
			deltaPrice	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			deltaPdivSigmaP = new DataSeries(this, MaximumBarsLookBack.Infinite); 
			vpinDataseries	= new DataSeries(this, MaximumBarsLookBack.Infinite);
			
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "VPIN"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "CDFofVPIN"));
            Overlay				= true;
			if(useRealtimeVol)
				CalculateOnBarClose = false;
			else
				CalculateOnBarClose = true;
			//stats = new Simulator(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double squared = 0;
			double mean = 0;
			double sumDeltaVol = 0;
			double stddevtemp=0;
			double normdisttemp=0;
			double sumOfOI = 0;  
			
			double sumOfDerivationAverage=0;
			double remainingVolume=0;
			double remainingAggBuyVol=0;
			double remainingAggSellVol=0;
			double sumOfDerivation = 0.0;
			int counter=0;
			
			double cdfBuyDeltaPrice, cdfSellDeltaPrice, volBuyDeltaPrice,volSellDeltaPrice;

			
			//Print("Currentbar   = " + CurrentBar);
			
			if(useRealtimeVol)
			{
				if(Historical)
					return;
			}
			
			// CALCULATE EACH BAR 
			// then, if over V > 30000 , then basket full, CALCULATE VPIN
			if (CurrentBar < 5) 
			{
				return;
			}
			deltaPrice[0] = Close[0] - Close[1];
			
			if (CurrentBar < 510) 
			{
				return;
			}
			
			stddevtemp = StdDev(deltaPrice, 500)[0];
			deltaPdivSigmaP[0] = deltaPrice[0]/stddevtemp;
				
			//normdisttemp = NORMDIST( deltaPdivSigmaP[0], SMA(deltaPdivSigmaP, 500)[0], StdDev(deltaPdivSigmaP, 500)[0], true);
			normdisttemp = NORMDIST( deltaPdivSigmaP[0], 0, StdDev(deltaPdivSigmaP, 500)[0], true);
			

			//VolBuy = VolBuy + (Volume[0] * normdisttemp  );
			//VolSell = VolSell + ( Volume[0] * (1 - normdisttemp ) );
						
			// keep track of the volume bucket
			// if over V, then calculate vpin
				
			VolCounter = VolCounter + Volume[0];
			
			// LOGS ----------------------------------
			cdfSellDeltaPrice = 1 - normdisttemp;
			volBuyDeltaPrice = (Volume[0] * normdisttemp);
			volSellDeltaPrice = Volume[0] * (1 - normdisttemp );
			//Print(" " + Time[0] + "\t" + deltaPrice[0].ToString("0.00") + "\t" + Volume[0] + "\t" + VolCounter + "\t" + basketCount + "\t" +
			// 	normdisttemp.ToString("0.00") + "\t" + cdfSellDeltaPrice.ToString("0.00") + "\t" + 
			//	volBuyDeltaPrice.ToString("0") + "\t" + ( volSellDeltaPrice.ToString("0")));
			// ------------------------------------------
			
			accumulatedBuyVol = accumulatedBuyVol + volBuyDeltaPrice;
			accumulatedSellVol = accumulatedSellVol + volSellDeltaPrice;

			// V bucket is accomplished, VPIN then calculated (every completed bucket)
			if( VolCounter > volBucketSize ) {
				basketTempCount=basketTempCount+1;
				
				remainingVolume = VolCounter - volBucketSize;
			//	Print("remainingVolume " +remainingVolume);
				remainingAggBuyVol = remainingVolume * normdisttemp;
				remainingAggSellVol = remainingVolume * (1- normdisttemp);
				
				//save previous orderImbalance value first
				prevBasketOI = orderImbalance;
				//Print("prevBasketOI " +prevBasketOI);
				// VPIN = sum(order imbalance of 50 buckets) / 50 * VolBucketSize
				orderImbalance = Math.Abs(accumulatedBuyVol - accumulatedSellVol);
				//orderImbalance = accumulatedBuyVol - accumulatedSellVol;
				
				qAccOI.Enqueue(orderImbalance);
				if( qAccOI.Count >= 50 )
				{
					foreach (double value in qAccOI)
					{
						sumOfOI += value; 		
					}
					qAccOI.Dequeue();
				}
				
				//Print("orderImbalance = "+orderImbalance.ToString("0")
				//+ " sumOfOI = " + sumOfOI.ToString("0")
				//+" accumulatedBuyVol "+accumulatedBuyVol.ToString("0")
				//+ " accumulatedSellVol " +accumulatedSellVol.ToString("0") );
				
				basketCount = basketCount +1;
				
				// Calculate VPIN
				VPINvalue = sumOfOI / (nWindowsize * volBucketSize);
				
				
				// Calculate VPIN CDF
				queueAvgVpin.Enqueue( (double) VPINvalue );	
				if( queueAvgVpin.Count >= 50 ){
					foreach (double value in queueAvgVpin)
					{
						//Print("queue["+counter+"] = " + value);
						counter += 1;
						avgVpin = avgVpin + value;
						sumOfDerivation += (value) * (value); 
							
					}
					sumOfDerivationAverage = sumOfDerivation / queueAvgVpin.Count;
						
					avgVpin = avgVpin/queueAvgVpin.Count;
					stddevVpin = Math.Sqrt(sumOfDerivationAverage - (avgVpin*avgVpin));
			//		Print("avgVpin = " + avgVpin);	
			//		Print("stddevVpin = " + stddevVpin);
					queueAvgVpin.Dequeue();
				}
				//Print(" " + Time[0] + "\t" + Close[0]  +"\t" + VPINvalue.ToString("0.0000"));
			//	Print( "" + VPINvalue.ToString("0.0000") + "\t" + sumOfOI.ToString("0") + "\t" + (basketCount-1) + "\t" + "\t"
			//		+ accumulatedBuyVol.ToString("0")+ "\t"+ accumulatedSellVol.ToString("0") + "\t" + orderImbalance.ToString("0") );
				
				if( basketCount > (nWindowsize-1) ) {
		
					basketCount = 0;
				}
						
				accumulatedBuyVol = remainingAggBuyVol;
				accumulatedSellVol = remainingAggSellVol;
				VolCounter = remainingVolume;
				
				vpintemp=NORMDIST( VPINvalue, avgVpin, stddevVpin, true);
				
			} // if( VolCounter > volBucketSize )
			
			vpinDataseries[0] = VPINvalue;
			
				
			avgVpin=0;
				
			//Print("vpinDataseries[0] = " + vpinDataseries[0] + " stddevVpin = "+stddevVpin+" vpintemp = " +vpintemp );
			VPIN.Set(vpinDataseries[0]);
           	CDFofVPIN.Set(vpintemp)  ;
				
			
        }
		


        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VPIN
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CDFofVPIN
        {
            get { return Values[1]; }
        }

        [Description("V")]
        [GridCategory("Parameters")]
        public int VolBucketSize
        {
            get { return volBucketSize; }
            set { volBucketSize = Math.Max(100, value); }
        }

        [Description("n")]
        [GridCategory("Parameters")]
        public int SampleSize
        {
            get { return sampleSize; }
            set { sampleSize = Math.Max(1, value); }
        }
		
        [Description("cdfsamplesize")]
        [GridCategory("Parameters")]
        public int Cdfsamplesize
        {
            get { return cdfsamplesize; }
            set { cdfsamplesize = Math.Max(1, value); }
        }
		
        #endregion
    
	
 	#region Miscellaneous
/// <summary>The normdist.</summary>
        /// <param name="x">The x.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="std">The std. </param>
        /// <param name="cumulative">The cumulative.</param>
        /// <returns>The normdist.</returns>
        private static double NORMDIST(double x, double mean, double std, bool cumulative)
        {
            if (cumulative)
            {
                return Phi(x, mean, std);
            }

            var tmp = 1/(Math.Sqrt(2*Math.PI)*std);
            return tmp*Math.Exp(-.5*Math.Pow((x - mean)/std, 2));
        }

        // from http://www.cs.princeton.edu/introcs/...Math.java.html
        // fractional error less than 1.2 * 10 ^ -7.

        // cumulative normal distribution
        /// <summary>The phi.</summary>
        /// <param name="z">The z.</param>
        /// <returns>The phi.</returns>
        private static double Phi(double z)
        {
            return 0.5*(1.0 + erf(z/Math.Sqrt(2.0)));
        }

        // cumulative normal distribution with mean mu and std deviation sigma
        /// <summary>The phi.</summary>
        /// <param name="z">The z.</param>
        /// <param name="mu">The mu.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns>The phi.</returns>
        private static double Phi(double z, double mu, double sigma)
        {
            return Phi((z - mu)/sigma);
        }

        /// <summary>The erf.</summary>
        /// <param name="z">The z.</param>
        /// <returns>The erf.</returns>
        private static double erf(double z)
        {
            var t = 1.0/(1.0 + 0.5*Math.Abs(z));

            // use Horner's method
            var ans = 1 -
                      t*
                      Math.Exp(
                          -z*z - 1.26551223 +
                          t*
                          (1.00002368 +
                           t*
                           (0.37409196 +
                            t*
                            (0.09678418 +
                             t*
                             (-0.18628806 +
                              t*
                              (0.27886807 +
                               t*(-1.13520398 + t*(1.48851587 + t*(-0.82215223 + t*0.17087277)))))))));
            if (z >= 0)
            {
                return ans;
            }

            return -ans;
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
        private ZZVPINMinuteBarV2[] cacheZZVPINMinuteBarV2 = null;

        private static ZZVPINMinuteBarV2 checkZZVPINMinuteBarV2 = new ZZVPINMinuteBarV2();

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return ZZVPINMinuteBarV2(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            if (cacheZZVPINMinuteBarV2 != null)
                for (int idx = 0; idx < cacheZZVPINMinuteBarV2.Length; idx++)
                    if (cacheZZVPINMinuteBarV2[idx].Cdfsamplesize == cdfsamplesize && cacheZZVPINMinuteBarV2[idx].SampleSize == sampleSize && cacheZZVPINMinuteBarV2[idx].VolBucketSize == volBucketSize && cacheZZVPINMinuteBarV2[idx].EqualsInput(input))
                        return cacheZZVPINMinuteBarV2[idx];

            lock (checkZZVPINMinuteBarV2)
            {
                checkZZVPINMinuteBarV2.Cdfsamplesize = cdfsamplesize;
                cdfsamplesize = checkZZVPINMinuteBarV2.Cdfsamplesize;
                checkZZVPINMinuteBarV2.SampleSize = sampleSize;
                sampleSize = checkZZVPINMinuteBarV2.SampleSize;
                checkZZVPINMinuteBarV2.VolBucketSize = volBucketSize;
                volBucketSize = checkZZVPINMinuteBarV2.VolBucketSize;

                if (cacheZZVPINMinuteBarV2 != null)
                    for (int idx = 0; idx < cacheZZVPINMinuteBarV2.Length; idx++)
                        if (cacheZZVPINMinuteBarV2[idx].Cdfsamplesize == cdfsamplesize && cacheZZVPINMinuteBarV2[idx].SampleSize == sampleSize && cacheZZVPINMinuteBarV2[idx].VolBucketSize == volBucketSize && cacheZZVPINMinuteBarV2[idx].EqualsInput(input))
                            return cacheZZVPINMinuteBarV2[idx];

                ZZVPINMinuteBarV2 indicator = new ZZVPINMinuteBarV2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Cdfsamplesize = cdfsamplesize;
                indicator.SampleSize = sampleSize;
                indicator.VolBucketSize = volBucketSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZZVPINMinuteBarV2[] tmp = new ZZVPINMinuteBarV2[cacheZZVPINMinuteBarV2 == null ? 1 : cacheZZVPINMinuteBarV2.Length + 1];
                if (cacheZZVPINMinuteBarV2 != null)
                    cacheZZVPINMinuteBarV2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZZVPINMinuteBarV2 = tmp;
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
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINMinuteBarV2(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINMinuteBarV2(input, cdfsamplesize, sampleSize, volBucketSize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            return _indicator.ZZVPINMinuteBarV2(Input, cdfsamplesize, sampleSize, volBucketSize);
        }

        /// <summary>
        /// VPIN indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZZVPINMinuteBarV2 ZZVPINMinuteBarV2(Data.IDataSeries input, int cdfsamplesize, int sampleSize, int volBucketSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZZVPINMinuteBarV2(input, cdfsamplesize, sampleSize, volBucketSize);
        }
    }
}
#endregion
