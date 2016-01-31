using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
//using NinjaTrader.Indicator;

namespace Gom
{	
	partial interface IDataManager : IDisposable
	{
        //Properties that will be used by the recorder
        string Name { get; }
        bool IsWritable { get; }
        bool IsMillisecCompliant { get; }

		// Init Data Manager
		// instr : name of instrument
		// writeData : open provider in write mode
		// GDMParams : params of the data manager.
//		void Initialize(string instrName, bool writeData, DataManagerParams GDMparams);

		//init cursor in the data provider using a timestamp and return first tick.
		// if EOF set gomdata.time to Gom.Utils.nullDT
		void SetCursorTime(DateTime time, ref MarketDataType gomdata);

		//get next tick in the provider. 
		// if EOF set gomdata.time to Gom.Utils.nullDT
		void GetNextTick(ref MarketDataType gomdata);

		//record incoming tick
		// return bool : Record went OK or not ?
		// as soon as RecordTick returns false, RecordTick will stop being called on incoming ticks.
		bool RecordTick(DateTime date, double bid, double ask, double price, int volume);

	}	

	public struct MarketDataType
	{
		public DateTime Time;
		public TickTypeEnum TickType;
		public double Price;
		public int Volume;
		public enum TimeStampStatus{Same,Different,Unknown}
		public TimeStampStatus IsNewTimeStamp;

		public MarketDataType(DateTime t, TickTypeEnum tt, double p, int v)
		{
			Time = t;
			TickType = tt;
			Price = p;
			Volume = v;
			IsNewTimeStamp=TimeStampStatus.Unknown;
		}
		
		public MarketDataType(DateTime t, TickTypeEnum tt, double p, int v,TimeStampStatus b)
		{
			Time = t;
			TickType = tt;
			Price = p;
			Volume = v;
			IsNewTimeStamp=b;
		}
		
	}

	public static partial class Utils
	{
		public static DateTime nullDT = new DateTime(0L);

		public static TickTypeEnum GetIntTickType(double bid, double ask, double price)
		{
			TickTypeEnum tickType;

			if (ask < bid) // should not happen but does
			{
				if (price < ask) tickType = TickTypeEnum.BelowBid;
				else if (price == ask) tickType = TickTypeEnum.AtAsk;
				else if (price < bid) tickType = TickTypeEnum.BetweenBidAsk;
				else if (price == bid) tickType = TickTypeEnum.AtBid;
				else tickType = TickTypeEnum.AboveAsk;
			}
			else if (bid < ask) //normal case
			{
				if (price < bid) tickType = TickTypeEnum.BelowBid;
				else if (price == bid) tickType = TickTypeEnum.AtBid;
				else if (price < ask) tickType = TickTypeEnum.BetweenBidAsk;
				else if (price == ask) tickType = TickTypeEnum.AtAsk;
				else tickType = TickTypeEnum.AboveAsk;
			}
			else //bid==ask, should not happen
			{
				if (price < bid) tickType = TickTypeEnum.BelowBid;
				else if (price > ask) tickType = TickTypeEnum.AboveAsk;
				else tickType = tickType = TickTypeEnum.BetweenBidAsk;
			}

			return tickType;
		}
	}
}
	
public enum TickTypeEnum
{ BelowBid, AtBid, BetweenBidAsk, AtAsk, AboveAsk, Unknown }






