#region Using declarations
using System;
#endregion

namespace JHL.Utility {
		
	using NinjaTrader.Data;
	using System.Collections.Generic;
	
	class constants {
		public static readonly double golden = (Math.Sqrt(5.0) + 1.0) / 2.0;
		public static readonly double phi = golden;
		public static readonly double Phi = 2.0 / (Math.Sqrt(5.0) + 1.0);
		public static readonly double log_2 = Math.Log(2.0);
		public const double pi = Math.PI;
	}
	
	// Calculates linearly weighted sum
	// Used by WMA and CoVarianceX
	public class WSum {
		// Because this is a supporting class, allow users to access (but not set) internals
		public int period { get; private set; }
		public int lastBar { get; private set; }
		public int bars { get; private set; }
		public double sum { get; private set; }
		public double[] value { get; private set; }
		
		public delegate double set_type(double curVal, int barNum);
		public set_type set { get; private set; }
		
		public WSum(int period)
		{
			lastBar = -1;
			bars = 0;
			sum = 0;
			value = new double[this.period = Math.Max(period, 1)];
			set = set_startup;
		}
		
		private double set_running(double curVal, int barNum)
		{
			if ( barNum == lastBar )
				sum -= value[barNum % period] * period;
			else {
				lastBar = barNum;
				for ( int i = 0; i < period; ++i, --barNum )
					sum -= value[barNum % period];
			}
			return sum += (value[lastBar % period] = curVal) * period;
		}
		
		private double set_startup(double curVal, int barNum)
		{
			if ( barNum == lastBar )
				sum -= value[barNum % period] * bars;
			else {
				lastBar = barNum;
				if ( ++bars == period )
					set = set_running;
			}
			return sum += (value[barNum % period] = curVal) * bars;
		}
	}
	
	public class SortList {
		private struct key {
			private IDataSeries input;
			private int period;
			
			public key(IDataSeries input, int period)
			{
				this.input = input;
				this.period = period;
			}
			
			public bool Equals(ref key k)
			{
				return period == k.period
					&& input == k.input;
			}			
		}
		private static Dictionary<key, SortList> dict = new Dictionary<key, SortList>();
		
		private int period;
		private double[] values;
		private double[] sorted;
		private int idxLastInsert = 0;
		private int lastBar = -1;
		private int ticks = 0;
		private int bars = 0;
		
		public static SortList instance(IDataSeries input, int period)
		{
			if ( input == null )
				return new SortList(input, period);
			lock(dict) {
				key k = new key(input, period);
				if ( dict.ContainsKey(k) )
					return dict[k];
				else
					return dict[k] = new SortList(input, period);
			}
		}		
		
		public SortList(IDataSeries input, int period)
		{
			this.period = Math.Max(period, 1);
			values = new double[this.period];
			sorted = new double[this.period];
			for ( int i = 0; i < this.period; ++i ) {
				values[i] = double.MaxValue;
				sorted[i] = double.MaxValue;
			}
		}
		
		public SortList(int period) : this(null, period)
		{
		}
		
		public void set(double curVal, int barNum)
		{
			set(curVal, barNum, 0);
		}
		
		public void set(double curVal, int barNum, int tickCount)
		{
			if ( ticks == tickCount && barNum == lastBar && tickCount != 0 )
				return;
			
			// Find location of value to be removed
			int idxRemove;
			if ( barNum != lastBar ) {
				// Adding a new bar, the bar to be removed will be at the beginning
				// of the series, so we assume it may not be proximate in value and
				// we use a binary search.
				double n = values[barNum % period];
				idxRemove = Array.BinarySearch(sorted, n);
				lastBar = barNum;
				if ( bars < period )
					++bars;
			}
			else 
				idxRemove = idxLastInsert;
			
			// Find location which will contain new value.  Because prices are
			// relatively continuous, we use a linear search from prior insertion point.
			int idxInsert;
			if ( curVal == sorted[idxLastInsert] )
				idxInsert = idxLastInsert;
			else
			if ( curVal > sorted[idxLastInsert] ) {
				// search up
				for ( idxInsert = idxLastInsert + 1; idxInsert < period && curVal > sorted[idxInsert]; ++idxInsert )
					;
			}
			else
				// search down
				for ( idxInsert = idxLastInsert; idxInsert > 0 && curVal < sorted[idxInsert - 1]; --idxInsert )
					;
						
			if ( idxRemove > idxInsert ) {
				for ( int i = idxRemove - 1; i >= idxInsert; --i )
					sorted[i + 1] = sorted[i];
			}
			else if ( idxRemove < idxInsert ) {
				--idxInsert;
				for ( int i = idxRemove + 1; i <= idxInsert; ++i )
					sorted[i - 1] = sorted[i];
			}
			
			sorted[idxLastInsert = idxInsert] = values[barNum % period] = curVal;
		}
		
		public double first()
		{
			return sorted[0];
		}
		
		public double last()
		{
			return sorted[bars - 1];
		}		
	}
}
