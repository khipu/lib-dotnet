using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Khipu.Api
{
	public class KhipuResponse
	{
		Dictionary<string,object> _wrapper;
		public KhipuResponse (Dictionary<string,object> source)
		{
			this._wrapper= (from x in source select x).ToDictionary (x => x.Key,
				x => KhipuResponse.isList(x.Value)?KhipuResponse.WrappedList(x.Value):x.Value);
		}
		private static bool isList(object obj){
			return (obj.GetType()==typeof(RestSharp.JsonArray));
		}
		private static List<KhipuResponse> WrappedList(object value){
			List<object> ObjList = (List<object>)value;
			List<KhipuResponse> wrapperList = new List<KhipuResponse> ();
			foreach (Dictionary<string, object> ListItem in ObjList) {
				wrapperList.Add(new KhipuResponse(ListItem));
			};
			return wrapperList;
		}
		public bool isList(string name){
			return this[name].GetType()==typeof(List<KhipuResponse>);
		}
		public List<KhipuResponse> list(string name){
			return (List<KhipuResponse>)this[name];
		}
		public object this[string idx]{
			get{
				return _wrapper [idx];
			}
			set{
				_wrapper [idx] = value;
			}
		}
	}
}

