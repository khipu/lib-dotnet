using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp.Serializers;
using RestSharp.Deserializers;

namespace Khipu.Api
{
	public class KhipuApiEndPoint : KhipuService
	{

		public KhipuApiEndPoint (string receiver_id, string secret) : base(receiver_id, secret){
		}
		public Dictionary<string, object> Execute(string endpoint, Dictionary<string, object> kparams, bool add_hash=true, bool json_response=true, string base_url=KhipuApi.API_URL) {
			Dictionary<string, object> postParams = new Dictionary<string, object> (kparams);
			if (add_hash) {
				postParams.Add("hash", HmacSha256(Secret,base.Concatenated(kparams)));
			}
			return Post (endpoint, postParams, json_response, base_url);
		} 

		public KhipuResponse PaymentStatus( Dictionary<string, object> args){
			const string endpoint = "paymentStatus";
			CheckArguments(args, new string[]{"payment_id"});
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id",Receiver_Id },
				{ "payment_id", args ["payment_id"] }
			};
			return new KhipuResponse(Execute(endpoint, kparams));
		}

		public KhipuResponse CreatePaymentUrl( Dictionary<string, object> args){
			const string endpoint = "createPaymentURL";
			CheckArguments (args, new string[] { "subject", "amount" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id},
				{ "subject", args["subject"]},
				{ "body", DValue(args,"body","") },
				{ "amount", args["amount"]},
				{ "payer_email", DValue(args, "payer_email", "") },
				{ "bank_id", DValue(args, "bank_id","") },
				{ "expires_date", DValue(args, "expires_date", "") },
				{ "transaction_id", DValue(args, "transaction_id","") },
				{ "custom", DValue(args,"custom", "") },
				{ "notify_url", DValue(args, "notify_url", "") },
				{ "return_url", DValue(args, "return_url", "") },
				{ "cancel_url", DValue(args, "cancel_url", "") },
				{ "picture_url", DValue(args, "picture_url", "") }
			};
			return new KhipuResponse(Execute(endpoint, kparams));
		}
		public KhipuResponse CreateAuthenticatedPaymentUrl( Dictionary<string, object> args){
			const string endpoint = "createAuthenticatedPaymentURL";
			CheckArguments (args, new string[] { "subject", "amount", "payer_username" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "subject", args ["subject"] },
				{ "body", DValue(args, "body","") },
				{ "amount", args ["amount"] },
				{ "payer_username", args ["payer_username"] },
				{ "payer_email", DValue(args, "payer_email", "") },
				{ "bank_id", DValue(args, "bank_id", "") },
				{ "expires_date", DValue(args, "expires_date", "") },
				{ "transaction_id", DValue(args, "transaction_id", "") },
				{ "custom", DValue(args, "custom", "") },
				{ "notify_url", DValue(args, "notify_url","") },
				{ "return_url", DValue(args, "return_url", "") },
				{ "cancel_url", DValue(args, "cancel_url","") },
				{ "picture_url", DValue(args, "picture_url", "") }
			};
			return new KhipuResponse(Execute(endpoint, kparams));
		}

		public KhipuResponse CreateEmail( Dictionary<string, object> args){
			const string endpoint = "createEmail";
			CheckArguments(args, new string[] { "subject", "destinataries", "pay_directly", "send_emails"});
			string destinataries = null;
			if (args ["destinataries"].GetType () == typeof(string)) {
				try {
					RestSharp.SimpleJson.DeserializeObject (args ["destinataries"].ToString ());
					destinataries = args ["destinataries"].ToString();
				} catch (System.Runtime.Serialization.SerializationException jsonExp) {
					throw new ApplicationException ("invalid detinataries json data", jsonExp);
				}
			} else if (args ["destinataries"].GetType () == typeof(List<Dictionary<string,string>>)) {
				destinataries = RestSharp.SimpleJson.SerializeObject (args ["destinataries"]);
			} else {
				throw new ArgumentException (args ["destinataries"].GetType ()+" is not a destinataries valid type");
			}
				
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "subject", args ["subject"] },
				{ "body", DValue(args, "body", "") },
				{ "destinataries", destinataries }, 
				{ "pay_directly", args ["pay_directly"] },
				{ "send_emails", args ["send_emails"] ?? "" },
				{ "expires_date", DValue(args, "expires_date","") },
				{ "transaction_id", DValue(args, "transaction_id", "") },
				{ "custom", DValue(args, "custom", "") },
				{ "notify_url", DValue(args, "notify_url", "") },
				{ "return_url", DValue(args, "return_url","") },
				{ "cancel_url", DValue(args, "cancel_url", "") },
				{ "picture_url", DValue(args, "picture_url","") }
			};
			return new KhipuResponse(Execute (endpoint, kparams));
		}

		public List<Dictionary<string, object>> ReceiverBanks(){
			const string endpoint = "receiverBanks";
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id }
			};
			Dictionary<string, object> resp = Execute(endpoint, kparams);
			List<object> banks = (List<object>)resp ["banks"];
			List<String> items = new List<string> () { "id", "name", "message", "min-amount" };
			List<Dictionary<string, object>> recbanks = new List<Dictionary<string, object>> ();
			foreach (Dictionary<string, object> bankitem in banks) {
				recbanks.Add(items.Where (bankitem.ContainsKey).ToDictionary (k => k, k => bankitem [k]));
			};
			return recbanks;
		}
		public KhipuResponse ReceiverStatus(){
			const string endpoint = "receiverStatus";
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id }
			};
			return new KhipuResponse(Execute (endpoint, kparams));
		}

		public KhipuResponse SetBillExpired( Dictionary<string, object> args){
			const string endpoint = "setBillExpired";
			CheckArguments (args, new string[]{ "bill_id" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "bill_id", args["bill_id"] },
				{ "text", DValue(args, "text", "") }
			};
			return new KhipuResponse(Execute(endpoint, kparams));
		}
		public KhipuResponse SetPaidByReceiver( Dictionary<string, object> args){
			const string endpoint = "setPaidByReceiver";
			CheckArguments (args, new string[]{ "payment_id" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "payment_id", args["payment_id"] }
			};
			return new KhipuResponse(Execute (endpoint, kparams));
		}
		public KhipuResponse SetRejectedByPayer( Dictionary<string, object> args){
			const string endpoint = "setRejectedByPayer";
			CheckArguments (args, new string[]{ "payment_id" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "payment_id", args["payment_id"] },
				{ "text", DValue(args,"text","") }
			};
			return new KhipuResponse(Execute (endpoint, kparams));
		}
		public KhipuResponse GetPaymentNotification( Dictionary<string, object> args){
			const string endpoint = "GetPaymentNotification";
			CheckArguments (args, new string[]{ "notification_token" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "notification_token", args["notification_token"] }
			};
			return new KhipuResponse(Execute (endpoint, kparams, true, true));
		}
		public bool VerifyPaymentNotification( Dictionary<string, object> args){
			const string endpoint = "verifyPaymentNotification";
			CheckArguments (args, new string[]{ "api_version", "notification_id", "subject", "amount", "payer_email", "notification_signature" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "api_version", args["api_version"] },
				{ "receiver_id", Receiver_Id },
				{ "notification_id", args["notification_id"] },
				{ "subject", args["subject"] },
				{ "amount", args["amount"] },
				{ "currency", DValue(args,"currency","CLP") },
				{ "transaction_id", DValue(args,"transaction_id", "") },
				{ "payer_email", args["payer_email"] ?? "" },
				{ "custom", DValue(args,"custom") ?? "" },
				{ "notification_signature", args["notification_signature"] }
			};
			return Execute (endpoint, kparams, false, false)["raw_response"].ToString().Equals("VERIFIED");
		}

		public bool VerifyPaymentNotificationLocal( Dictionary<string, object> args){
			CheckArguments (args, new string[]{ "api_version", "notification_id", "subject", "amount", "payer_email", "notification_signature" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "api_version", args["api_version"] },
				{ "receiver_id", Receiver_Id },
				{ "notification_id", args["notification_id"] },
				{ "subject", args["subject"] },
				{ "amount", args["amount"] },
				{ "currency", DValue(args, "currency","CLP") },
				{ "transaction_id", DValue(args, "transaction_id", "") },
				{ "payer_email", args["payer_email"] ?? "" },
				{ "custom", DValue(args,"custom","") }
			};
			return VerifySignature (kparams, args["notification_signature"].ToString());
		}

		public KhipuResponse CreateReceiver( Dictionary<string, object> args){
			const string endpoint = "createReceiver";
			CheckArguments (args, new string[]{ "email", "first_name", "last_name", "notify_url", "identifier", "bussiness_category", "bussiness_name",
					"phone", "address_line_1", "address_line_2", "address_line_3", "country_code" });
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "email", args["email"] },
				{ "first_name", args["first_name"] },
				{ "last_name", args["last_name"]},
				{ "notify_url", args["notify_url"]},
				{ "identifier", args["identifier"]},
				{ "bussines_category", args["bussines_category"]},
				{ "bussines_name", args["bussines_name"]},
				{ "phone", args["phone"]},
				{ "address_line1", args["address_line1"]},
				{ "address_line2", args["address_line2"]},
				{ "address_line3", args["address_line3"]},
				{ "country_code", args["country_code"]},
				{ "text", DValue(args, "text","") }
			};
			return new KhipuResponse(Execute (endpoint, kparams, true, true, KhipuApi.INTEGRATOR_API_URL));
		}


	}
}

