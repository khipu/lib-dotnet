using System;
using System.Collections.Generic;

namespace Khipu.Api
{
	public class HtmlHelper : KhipuService
	{
		public HtmlHelper(string receiver_id, string secret) : base(receiver_id, secret){
		}
		public String CreatePaymentForm(Dictionary<string, object> args, string button = null){
			const string endPointUrl = KhipuApi.API_URL + "createPaymentPage";
			if (button == null) {
				button = KhipuApi.FORM_BUTTONS ["50x25"];
			}
			CheckArguments(args, new string[] {"subject", "amount"});
			Dictionary<string, object> kparams = new Dictionary<string, object> () {
				{ "receiver_id", Receiver_Id },
				{ "subject", args ["subject"] },
				{ "body", DValue(args, "body", "") },
				{ "amount", args ["amount"] },
				{ "payer_email", DValue(args, "payer_email", "") },
				{ "bank_id", DValue(args, "bank_id", "") },
				{ "expires_date", DValue(args, "expires_date", "") },
				{ "transaction_id", DValue(args, "transaction_id", "") },
				{ "custom", DValue(args, "custom", "") },
				{ "notify_url", DValue(args, "notify_url", "") },
				{ "return_url", DValue(args, "return_url", "") },
				{ "cancel_url", DValue(args,"cancel_url", "")},
				{ "picture_url", DValue(args,"picture_url", "") }
			};
			string payerUsername = "";
			if (args.ContainsKey("payer_username")){
				payerUsername = "\n<input type=\"hidden\" name=\"payer_username\" value=\""+args["payer_username"]+"\"/>";
				kparams["payer_username"] = args["payer_username"];
			}
			kparams.Add("hash", HmacSha256(this.Secret, Concatenated(kparams)));
			return "<form action=\""+endPointUrl+"\" method=\"post\">\n<input type=\"hidden\" name=\"receiver_id\" value=\""+kparams["receiver_id"]
				+"\">\n<input type=\"hidden\" name=\"subject\" value=\""+kparams["subject"]+
				"\"/>\n<input type=\"hidden\" name=\"body\" value=\""+kparams["body"]+
				"\">\n<input type=\"hidden\" name=\"amount\" value=\""+kparams["amount"]+
				"\">"+payerUsername+"\n<input type=\"hidden\" name=\"notify_url\" value=\""+kparams["notify_url"]+
				"\"/>\n<input type=\"hidden\" name=\"return_url\" value=\""+kparams["return_url"]+
				"\"/>\n<input type=\"hidden\" name=\"cancel_url\" value=\""+kparams["cancel_url"]+
				"\"/>\n<input type=\"hidden\" name=\"custom\" value=\""+kparams["custom"]+
				"\">\n<input type=\"hidden\" name=\"transaction_id\" value=\""+kparams["transaction_id"]+
				"\">\n<input type=\"hidden\" name=\"payer_email\" value=\""+kparams["payer_email"]+
				"\">\n<input type=\"hidden\" name=\"expires_date\" value=\""+kparams["expires_date"]+
				"\">\n<input type=\"hidden\" name=\"bank_id\" value=\""+kparams["bank_id"]+
				"\">\n<input type=\"hidden\" name=\"picture_url\" value=\""+kparams["picture_url"]+
				"\">\n<input type=\"hidden\" name=\"hash\" value=\""+kparams["hash"]+
				"\">\n<input type=\"image\" name=\"submit\" src=\""+button+"\">\n</form>";
		}
	}
}

