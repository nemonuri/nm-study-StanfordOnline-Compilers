// Original: https://github.com/dotnet/runtime/blob/main/src/coreclr/tools/Common/TypeSystem/Common/ThreadSafeFlags.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Interlocked = System.Threading.Interlocked;

namespace Nemonuri.LowLevel.Primitives.DotNet
{
    public struct ThreadSafeFlags
    {
        private volatile uint _value;

        public readonly uint Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlags(uint value)
        {
            return (_value & value) == value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFlags(uint flagsToAdd)
        {
            var originalFlags = _value;
            while (Interlocked.CompareExchange(ref _value, originalFlags | flagsToAdd, originalFlags) != originalFlags)
            {
                originalFlags = _value;
            }
        }
    }
}
