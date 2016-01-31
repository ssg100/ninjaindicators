using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NinjaTrader.Indicator;

namespace Gom
{	
	
	partial interface IDataManager : IDisposable
	{
		void Initialize(string instrName, bool writeData, GomRecorderIndicator indy);
	}
		

	[System.AttributeUsage(System.AttributeTargets.Property)]
	public class SpecificTo : System.Attribute
	{
		public string[] Name;

		public SpecificTo(params string[] param)
		{
			Name = param;
		}

        public SpecificTo(string param)
        {
            Name = new string[]{param};
        }
	}


	static class DataManagerList
	{
		public static List<string> Name = new List<string>();
        public static List<bool> Writable = new List<bool>();
        public static List<bool> MillisecCompliant = new List<bool>();
		public static List<Type> Type = new List<Type>();
 
		static DataManagerList()
		{
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsClass && !t.IsAbstract && (t.GetInterface(typeof(Gom.IDataManager).Name) != null)
						select t;

			foreach (var type in types)
			{
                IDataManager instance = (IDataManager)Activator.CreateInstance(type);

                Name.Add((string)(type.GetProperty("Name").GetValue(instance, null)));
                Writable.Add((bool)(type.GetProperty("IsWritable").GetValue(instance, null)));
                MillisecCompliant.Add((bool)(type.GetProperty("IsMillisecCompliant").GetValue(instance, null)));
 				Type.Add(type);

                instance.Dispose();
			}
		}
	}

	public class GomDataManagerConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StandardValuesCollection cols = new StandardValuesCollection(Gom.DataManagerList.Name);
			return cols;
		}
	}

}




