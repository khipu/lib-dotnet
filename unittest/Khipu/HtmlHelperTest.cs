using System;
using NUnit.Framework;
using Khipu.Api;
using Rhino.Mocks;
using System.IO;

namespace unittest
{
	[TestFixture ()]
	public class HtmlHelperTest
	{
		HtmlHelper endPoint;
		[TestFixtureSetUp]
		public void PreTestInitialize ()
		{
			endPoint = MockRepository.GeneratePartialMock<HtmlHelper> ("1234", "123456");
		}

		[Test()]
		public void CreatePaymentForm(){
			string form = endPoint.CreatePaymentForm (new System.Collections.Generic.Dictionary<string, object> {
				{"subject","Un cobro desde .Net"},
				{"body", "El cuerpo del cobro"},
				{"amount","1000"},
				{"email", "john.doe@gmail.com"}
			});
			Console.Out.WriteLine (form);
			Assert.IsNotNull (form);
		}
	}
}

