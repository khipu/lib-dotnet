using System;

namespace Khipu.Api
{
	public class ApiException : ApplicationException
	{
		private readonly String _errorType;

		public String ErrorType {
			get {
				return _errorType;
			}
		}

		public ApiException (String erroType, String errorMessage) :base(errorMessage)
		{
			this._errorType = erroType;
		}
	}
}

