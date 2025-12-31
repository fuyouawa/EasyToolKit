using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    public enum MessageBoxType
    {
        /// <summary>
        ///   <para>Neutral message.</para>
        /// </summary>
        None,
        /// <summary>
        ///   <para>Info message.</para>
        /// </summary>
        Info,
        /// <summary>
        ///   <para>Warning message.</para>
        /// </summary>
        Warning,
        /// <summary>
        ///   <para>Error message.</para>
        /// </summary>
        Error
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MessageBoxAttribute : InspectorAttribute
    {
        public string VisibleIf { get; set; }
        public string Message { get; set; }
        public MessageBoxType MessageType { get; set; }

        public MessageBoxAttribute(string message, MessageBoxType messageType = MessageBoxType.Info, string visibleIf = null)
        {
            Message = message;
            MessageType = messageType;
            VisibleIf = visibleIf;
        }
    }
}
