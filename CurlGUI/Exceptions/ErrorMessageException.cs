using System;
using System.Runtime.Serialization;

namespace CurlGUI.Exceptions
{
    /// <summary>
    /// エラーメッセージ表示用例外クラス
    /// </summary>
    [Serializable]
    public class ErrorMessageException : Exception
    {
        public ErrorMessageException()
            : base()
        {
        }

        public ErrorMessageException(string message)
            : base(message)
        {
        }

        public ErrorMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ErrorMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
