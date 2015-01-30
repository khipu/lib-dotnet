using System;
using NUnit.Framework;
using Khipu.Api;
using Rhino.Mocks;
using System.Collections.Generic;

namespace unittest
{
	[TestFixture ()]
	public class KhipuApiEndPointTest
	{
		KhipuApiEndPoint endPoint;
		[TestFixtureSetUp]
		public void PreTestInitialize ()
		{
			endPoint = MockRepository.GeneratePartialMock<KhipuApiEndPoint> ("1234", "123456");
		}

		[Test()]
		public void VerifyPaymentNotification(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"VERIFIED"));

			bool verified = endPoint.VerifyPaymentNotification(new Dictionary<string, object> {
				{"api_version", "1.2"},
				{"notification_id", "xxxxxxx"},
				{"subject", "abono a cuenta"},
				{"amount", 2014},
				{"payer_email", "payer@email.com"},
				{"notification_signature", "Cw1pyk0JfvQo9mZO9ob1hjaMEO8hLqP2v/X81TW9jdH4ECuAr2fwGtPASuDR5vKf9I4nIy2ZOKlSYrBbMbVJ2psjaGQq3qwf4PuVz2uvN26WtjmroOYkx60Riq4TlsaE9WbfxtF3fgc5wlvRgeNKJKcWRPzwPRqFB/A2My3JsTo40wJiHho11/GoU1/p7cH5XdMN3oE37VXgtaHrsL8ZGDQ6F+qEZweSiBnvlJ4YoW/XKy6WVfRA+iWholte2VndvD/B450bToZi8ULO/JLLtYeXZoZyNpI/SD3vOE8pp0q5TGds//oQwKVSC09lqGv1ITv3baCpwuysDDvoS/jU4A=="}
			});
			Assert.AreEqual (true, verified);
		}
		[Test()]
		public void PaymentStatus(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"status\":\"done\",\"detail\":\"normal\"}"));
			KhipuResponse resp = endPoint.PaymentStatus(new Dictionary<string, object>{
				{"payment_id","zdcgmvxxxsd"}
			});
			Assert.AreEqual("done", resp["status"]);
			Assert.AreEqual("normal", resp["detail"]);
		}
		[Test()]
		public void ReceiverStatus(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"ready_to_collect\":true,\"type\":\"development\"}"));
			KhipuResponse resp = endPoint.ReceiverStatus();
			Assert.AreEqual(true, resp["ready_to_collect"]);
			Assert.AreEqual("development", resp["type"]);
		}
		[Test()]
		public void ReceiverBanks(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"banks\":[{\"id\":\"Bawdf\",\"name\":\"DemoBank\",\"message\":\"Este es un banco de pruebas. Las transacciones no son reales.\",\"min-amount\":200,\"type\":\"Persona\",\"parent\":\"\"}]}"));
			List<Dictionary<string, object>> recBanks = endPoint.ReceiverBanks ();
			Assert.AreEqual(1, recBanks.Count);
			Assert.AreEqual ("Bawdf", recBanks[0]["id"]);
			Assert.AreEqual ("DemoBank", recBanks[0]["name"]);
			Assert.AreEqual (200, recBanks[0]["min-amount"]);
		}

		[Test()]
		public void GetPaymentNotification(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{ \"notification_token\":\"khipu_notification_token\", \"receicer_id\":\"asdawsdx\", \"subject\":\"pago de articulo\", \"amount\":2015, \"custom\":\"\",\"transaction_id\":\"\", \"payment_id\":\"1234\", \"currency\":\"CLP\", \"payer_email\":\"payer@email.com\"}" ));
			KhipuResponse resp = endPoint.GetPaymentNotification(new Dictionary<string, object>{
				{"notification_token","khipu_notification_token"}
			});
			Assert.AreEqual("khipu_notification_token",resp["notification_token"]);
		}
		[Test()]
		public void CreateEmailWithDestinatariesAsString(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{ \"id\": \"HWkZc\", \"payments\": [ {\"id\": \"cghs6n7mtklx\",\"destinataries\":[ { \"name\": \"Juan Rojo\", \"email\": \"juan.rojo@ejemplo.com\" } ], \"url\": \"https://khipu.com/payment/show/cghs6n7mtklx\" }, {\"id\": \"ueqahp3e4xq3\", \"destinataries\":[{\"name\": \"Pedro Piedra\",\"email\": \"pedro.piedra@ejemplo.com\"}],\"url\": \"https://khipu.com/payment/show/ueqahp3e4xq3\"}, {\"id\": \"9823ojq8ahs\",\"destinataries\":[{\"name\": \"Ana Soto\",\"email\": \"ana.soto@ejemplo.com\"}],\"url\": \"https://khipu.com/payment/show/9823ojq8ahs\"}]}" ));
			KhipuResponse resp = endPoint.CreateEmail(new Dictionary<string, object>{
				{"subject", "Un cobro desde Ruby"},
				{"pay_directly", true},
				{"send_emails", true},
				{"destinataries", "[ {\"name\": \"Juan Rojo\", \"email\": \"juan.rojo@ejemplo.com\", \"amount\": \"1000\"}, {\"name\": \"Pedro Piedra\", \"email\": \"pedro.piedra@ejemplo.com\", \"amount\": \"1000\"}, { \"name\": \"Ana Soto\", \"email\": \"ana.soto@ejemplo.com\", \"amount\": \"1000\"}]"} 
			});
			Assert.IsNotNull (resp ["id"]);
			Assert.IsTrue (resp.isList ("payments"));
			Assert.AreEqual (3, resp.list("payments").Count);
			Assert.AreEqual (1, resp.list ("payments") [0].list ("destinataries").Count);
		}

		[Test()]
		public void CreateEmailWithDestinatariesAsMap(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{ \"id\": \"HWkZc\", \"payments\": [ {\"id\": \"cghs6n7mtklx\",\"destinataries\":[ { \"name\": \"Juan Rojo\", \"email\": \"juan.rojo@ejemplo.com\" } ], \"url\": \"https://khipu.com/payment/show/cghs6n7mtklx\" }, {\"id\": \"ueqahp3e4xq3\", \"destinataries\":[{\"name\": \"Pedro Piedra\",\"email\": \"pedro.piedra@ejemplo.com\"}],\"url\": \"https://khipu.com/payment/show/ueqahp3e4xq3\"}, {\"id\": \"9823ojq8ahs\",\"destinataries\":[{\"name\": \"Ana Soto\",\"email\": \"ana.soto@ejemplo.com\"}],\"url\": \"https://khipu.com/payment/show/9823ojq8ahs\"}]}" ));
			List<Dictionary<string,string>> destinataries = new List<Dictionary<string, string>> () {
				new Dictionary<string,string> () {
					{"name", "Juan Rojo"},
					{"email", "juan.rojo@ejemplo.com"},
					{"amount", "1000"}
				},
				new Dictionary<string,string> () {
					{"name", "Pedro Piedra"},
					{"email", "pedro.piedra@ejemplo.com"},
					{"amount", "1000"}
				},
				new Dictionary<string,string> () {
					{"name", "Ana Soto"},
					{"email", "ana.soto@ejemplo.com"},
					{"amount", "1000"}
				}
			};
			KhipuResponse resp = endPoint.CreateEmail(new Dictionary<string, object>{
				{"subject", "Un cobro desde Ruby"},
				{"pay_directly", true},
				{"send_emails", true},
				{"destinataries", destinataries} 
			});
			Assert.IsNotNull (resp ["id"]);
			Assert.IsTrue (resp.isList ("payments"));
			Assert.AreEqual (3, resp.list("payments").Count);
			Assert.AreEqual (1, resp.list ("payments") [0].list ("destinataries").Count);
		}
		[Test()]
		public void CreatePaymentUrl(){
			endPoint.BackToRecord(BackToRecordOptions.All);
			endPoint.Replay();
			endPoint.Stub (s => s.DoRequest (Arg<string>.Is.Anything,
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"id\":\"5wb665tyvm1p\", \"bill-id\":\"SSRwa\", \"url\":\"https://khipu.com/payment/show/5wb665tyvm1p\", \"manual-url\":\"https://khipu.com/payment/manual/5wb665tyvm1p\", \"mobile-url\":\"khipu:///pos/5wb665tyvm1p\", \"ready-for-terminal\":false }"));

			KhipuResponse resp = endPoint.CreatePaymentUrl(new Dictionary<string, object>{
				{"subject","Un cobro desde .Net"},
				{"body", "El cuerpo del cobro"},
				{"amount","1000"},
				{"email", "john.doe@gmail.com"}
			});

			Assert.IsNotNull (resp);
			Assert.IsNotNull (resp ["id"]);
			Assert.IsNotNull (resp ["bill-id"]);
			Assert.IsNotNull (resp ["url"]);
			Assert.IsNotNull (resp ["manual-url"]);
			Assert.IsNotNull (resp ["mobile-url"]);
			Assert.IsNotNull (resp ["ready-for-terminal"]);
		}


	}
}

