using NUnit.Framework;
using Khipu.Api;
using Rhino.Mocks;
using System.Collections.Generic;


namespace unittest
{
	[TestFixture ()]
	public class KhipuServiceTest
	{
		KhipuService service;
		[TestFixtureSetUp]
		public void PreTestInitialize ()
		{
			service = MockRepository.GeneratePartialMock<KhipuService> ("1234", "1234");
		}

		[Test()]
		public void hmac_calculate(){
			Assert.AreEqual("d219a3dd877f943be0ee65ef6a34129778d2fd0568ce2d9a8f37fea9d7fcaca3", service.HmacSha256 ("abcd", "1234"));
		}

		[Test()]
		public void concatenate_parameters(){
			Assert.AreEqual ("a=1&b=foo&c=&d=true&e=1200300000",service.Concatenated (new Dictionary<string, object>{ { "a","1" }, { "b","foo" },{"c",null}, {"d",true}, {"e",1200300000} }));
		}

		[Test()]
		public void returnBody(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"Hello World"));

			Dictionary<string, object> resp = service.Post ("endpoint", new Dictionary<string, object> (), false,"");
			Assert.AreEqual("Hello World", resp["raw_response"]);
		}

		[Test()]
		public void returJson(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"text\": \"Hello World\"}"));

			Dictionary<string, object> resp = service.Post ("endpoint", new Dictionary<string, object> (), true,"");
			Assert.AreEqual("Hello World",resp["text"]);
		}

		[Test()]
		[ExpectedException( "System.ApplicationException", ExpectedMessage="Invalid response from endpoint endpoint")]
		public void invalidjson(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(200,"{\"text\": }"));

			service.Post ("endpoint", new Dictionary<string, object> (), true,"");
		}

		[Test()]
		[ExpectedException("System.ApplicationException", ExpectedMessage="Invalid response code 500 from endpoint endpoint")]
		public void invalidHttpResponse(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(500));

			service.Post ("endpoint", new Dictionary<string, object> (), true,"");

		}
		[Test()]
		[ExpectedException("System.ApplicationException", ExpectedMessage="Invalid response from endpoint endpoint")]
		public void invalidHttpResponse400AndInvalidJson(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(400,"{\"error\":}"));

			service.Post ("endpoint", new Dictionary<string, object> (), true,"");

		}
		[Test()]
		[ExpectedException("Khipu.Api.ApiException", ExpectedMessage="Message")]
		public void invalidHttpResponse400(){
			service.BackToRecord(BackToRecordOptions.All);
			service.Replay();

			service.Stub (s => s.DoRequest (Arg<string>.Is.Equal("endpoint"),
				Arg<string>.Is.NotNull,
				Arg<Dictionary<string,object>>.Is.Anything))
				.Return (new DummyResponse(400,"{\"error\": {\"type\":\"Type\", \"message\": \"Message\"}}"));

			service.Post ("endpoint", new Dictionary<string, object> (), true,"");

		}

	}
}

