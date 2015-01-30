using System;
using RestSharp;

namespace Khipu.Api
{
	public class DummyResponse : IRestResponse
	{
		#region IRestResponse implementation

		public IRestRequest Request {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string ContentType {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public long ContentLength {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string ContentEncoding {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string Content {
			get {
				return _content;
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public System.Net.HttpStatusCode StatusCode {
			get;
			set;
		}

		public string StatusDescription {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public byte[] RawBytes {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Uri ResponseUri {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string Server {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public System.Collections.Generic.IList<RestResponseCookie> Cookies {
			get {
				throw new NotImplementedException ();
			}
		}

		public System.Collections.Generic.IList<Parameter> Headers {
			get {
				throw new NotImplementedException ();
			}
		}

		public ResponseStatus ResponseStatus {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string ErrorMessage {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Exception ErrorException {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion

		private string _content;
		public DummyResponse (int code,String content)
		{
			this.StatusCode = (System.Net.HttpStatusCode)code;
			this._content = content;
		}
		public DummyResponse (int code)
		{
			this.StatusCode = (System.Net.HttpStatusCode)code;
		}

	}
}

