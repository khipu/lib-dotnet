using System;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography.X509Certificates;


namespace Khipu.Api
{

	public class KhipuService
	{

		public KhipuService (string receiver_id, string secret)
		{
			this.Receiver_Id = receiver_id;
			this.Secret = secret;
		}
		public String Receiver_Id {
			get;
			set;
		}
		public String Secret {
			get;
			set;
		}
		public string Agent {
			get ;
			set ;
		}

		public virtual IRestResponse DoRequest(string endpoint, string base_url, Dictionary<string,object> kparams){
			var client = new RestClient(base_url);
			var request = new RestRequest (endpoint, Method.POST);
			client.UserAgent = "khipu-dotNet-"+KhipuApi.version;
			foreach (KeyValuePair<string,object> entry in kparams) {
				request.AddParameter (entry.Key, entry.Value);
			}
			request.RequestFormat = DataFormat.Json;
			return client.Execute (request);
		}
		public Dictionary<string, object> Post(string endpoint, Dictionary<string,object> kparams, Boolean json_response=true, string base_url="" ){
			if (this.Agent!=null)
				kparams.Add("agent","lib-dotNet-"+KhipuApi.version);
			var response = DoRequest (endpoint, base_url, kparams);
			JsonDeserializer deserial= new JsonDeserializer();

			if (response.StatusCode == HttpStatusCode.BadRequest) {
				try {
					var JSONObj = deserial.Deserialize<Dictionary<string, Dictionary<string,string>>> (response); 
					ApiException _exception = new ApiException (JSONObj["error"]["type"], JSONObj["error"]["message"]);
					throw _exception;
				} catch (System.Runtime.Serialization.SerializationException serExp) {
					throw new ApplicationException ("Invalid response from endpoint " + base_url + endpoint, serExp);
				}
			} else if (response.StatusCode != HttpStatusCode.OK) {
				throw new ApplicationException("Invalid response code "+(int)response.StatusCode+" from endpoint " + base_url + endpoint);
			}
			if (KhipuApi.debug) {
				KhipuApi.DebugStream.WriteLine (response.Content);
			}
			if (json_response) {
				try {
					return deserial.Deserialize<Dictionary<string, object>> (response);
				} catch (System.Runtime.Serialization.SerializationException serExp) {
					throw new ApplicationException ("Invalid response from endpoint " + base_url + endpoint, serExp);
				}
			} else {
				Dictionary<string, object> JSONObj = new Dictionary<string, object>();
				JSONObj.Add("raw_response", response.Content);
				return JSONObj;
			}
		}

		public void CheckArguments(Dictionary<string, object> kparams, string[] mandatoryArgs){
			foreach (string arg in mandatoryArgs) {
				if (!kparams.ContainsKey(arg)) {
					throw new ArgumentException (arg+" not present");
				}
			}
		}
		public String HmacSha256(string secret, string data){
			using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret))) {
				byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
				return BitConverter.ToString (hashValue).Replace("-", string.Empty).ToLower();
			}
		}
		public String Concatenated(Dictionary<string, object> kparams){
			int i = 0;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (KeyValuePair<string, object> entry in kparams) {
				if (i >0) {
					sb.Append ("&");
				}
				sb.Append(entry.Key+ "=" + (entry.Value!=null?(entry.Value.GetType()==typeof(bool)?entry.Value.ToString().ToLower(): entry.Value.ToString()):""));
				i++;
			}
			return sb.ToString();
		}
		public bool VerifySignature(Dictionary<string, object> kparams, string signature){
			X509Certificate2 cert = new X509Certificate2(Encoding.UTF8.GetBytes(KhipuApi.dev_env?Const.CERT_DEV:Const.CERT_PROD));
			RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
			SHA1Managed sha1 = new SHA1Managed();
			byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(Concatenated(kparams)));
			return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(signature));
		}

		protected object DValue(Dictionary<string, object> kparams, string key, string defaultvalue=null){
			object value;
			if (!kparams.TryGetValue (key, out value)) {
				value = defaultvalue;
			};
			return value;
		}
		protected string DValue(Dictionary<string, string> kparams, string key, string defaultvalue=null){
			string value;
			if (kparams.TryGetValue (key, out value)) {
				value = defaultvalue;
			};
			return value;
		}


	}
}

