using System;
using System.IO;
using System.Collections.Generic;
using Khipu.Api;
using System.Diagnostics;

	public static class KhipuApi
	{
		public const string version = "1.3.3";
		public const string API_URL="https://khipu.com/api/1.3/";
		public const string INTEGRATOR_API_URL = "https://khipu.com/integratorApi/1.3/";
		private const string BUTTONS_URL_BASE = "https://s3.amazonaws.com/static.khipu.com/buttons/";
		public static Dictionary<string, string> FORM_BUTTONS = new Dictionary<string, string>(){
			{"50x25"       , BUTTONS_URL_BASE+"50x25.png"},
			{"100x25"      , BUTTONS_URL_BASE+"100x25.png"},
			{"100x25white" , BUTTONS_URL_BASE+"100x25-w.png"},
			{"100x50"      , BUTTONS_URL_BASE+"100x50.png"},
			{"100x50white" , BUTTONS_URL_BASE+"100x50-w.png"},
			{"150x25"      , BUTTONS_URL_BASE+"150x25.png"},
			{"150x25white" , BUTTONS_URL_BASE+"150x25-w.png"},
			{"150x50"      , BUTTONS_URL_BASE+"150x50.png"},
			{"150x50white" , BUTTONS_URL_BASE+"150x50-w.png"},
			{"150x75"      , BUTTONS_URL_BASE+"150x75.png"},
			{"150x75white" , BUTTONS_URL_BASE+"150x75-w.png"},
			{"200x50"      , BUTTONS_URL_BASE+"200x50.png"},
			{"200x50white" , BUTTONS_URL_BASE+"200x50-w.png"},
			{"200x75"      , BUTTONS_URL_BASE+"200x75.png"},
			{"200x75white" , BUTTONS_URL_BASE+"200x75-w.png"}
		};

		public static KhipuApiEndPoint CreateKhipuApi(string receiver_id, string secret=""){
			return new KhipuApiEndPoint(receiver_id, secret);
		}

		public static HtmlHelper CreateHtmlHelper(string receiver_id, string secret=""){
			return new HtmlHelper (receiver_id, secret);
		}

		public static TextWriter DebugStream{
			get {
			return Console.Out;
			}
		}
		public static bool debug{
			get{
				return Environment.GetEnvironmentVariable("KHIPU_DEBUG")=="true";
			}
		}
		public static bool dev_env{
			get {
				return Environment.GetEnvironmentVariable("KHIPU_ENV")=="DEV";
			}
		}

	}


