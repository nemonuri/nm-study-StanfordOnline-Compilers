// Original source: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/SwitchExpressionException.cs

#if !NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS0809

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
    file static class SR
    {
        internal const string Arg_SwitchExpressionException = "Non-exhaustive switch expression failed to match its input.";
        internal const string SwitchExpressionException_UnmatchedValue = "Unmatched value was {0}.";
    }

    file static class Obsoletions
    {
        internal const string LegacyFormatterImplMessage = "This API supports obsolete formatter-based serialization. It should not be called or extended by application code.";
        internal const string LegacyFormatterImplDiagId = "SYSLIB0051";
        internal const string SharedUrlFormat = "https://aka.ms/dotnet-warnings/{0}";
    }

    /// <summary>
    /// Indicates that a switch expression that was non-exhaustive failed to match its input
    /// at runtime, e.g. in the C# 8 expression <code>3 switch { 4 => 5 }</code>.
    /// The exception optionally contains an object representing the unmatched value.
    /// </summary>
    [Serializable]
    [TypeForwardedFrom("System.Runtime.Extensions, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed class SwitchExpressionException : InvalidOperationException
    {
        public SwitchExpressionException()
            : base(SR.Arg_SwitchExpressionException) { }

        public SwitchExpressionException(Exception? innerException) :
            base(SR.Arg_SwitchExpressionException, innerException)
        {
        }

        public SwitchExpressionException(object? unmatchedValue) : this()
        {
            UnmatchedValue = unmatchedValue;
        }

        [Obsolete(Obsoletions.LegacyFormatterImplMessage)]
        private SwitchExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            UnmatchedValue = info.GetValue(nameof(UnmatchedValue), typeof(object));
        }

        public SwitchExpressionException(string? message) : base(message ?? SR.Arg_SwitchExpressionException) { }

        public SwitchExpressionException(string? message, Exception? innerException)
            : base(message ?? SR.Arg_SwitchExpressionException, innerException) { }

        public object? UnmatchedValue { get; }

        [Obsolete(Obsoletions.LegacyFormatterImplMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }

        public override string Message
        {
            get
            {
                if (UnmatchedValue is null)
                {
                    return base.Message;
                }

                string valueMessage = string.Format(SR.SwitchExpressionException_UnmatchedValue, UnmatchedValue);
                return base.Message + Environment.NewLine + valueMessage;
            }
        }
    }
}

#endif