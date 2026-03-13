// SPDX-FileCopyrightText: 2026 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System.Runtime.CompilerServices;

namespace Nethermind.ZiskBindings;

public static unsafe class IO
{
    private static readonly byte* Input = (byte*)0x4000_0000UL; // INPUT_ADDR
    private static readonly uint* Output = (uint*)0xa001_0000UL; // OUTPUT_ADDR
    private static readonly byte* Uart = (byte*)0xa000_0200UL; // UART_ADDR

    private static ulong _inputPosition = sizeof(ulong); // zkVM offset

    public static ReadOnlySpan<byte> ReadInputLegacy()
    {
        ulong size = *(ulong*)(Input + sizeof(ulong));

        if (size > int.MaxValue)
            Environment.FailFast("Input size exceeds the maximum supported length");

        return new ReadOnlySpan<byte>(Input + 2 * sizeof(ulong), (int)size);
    }

    public static ReadOnlySpan<byte> ReadInput()
    {
        byte* data = Input + checked((nint)_inputPosition); // [len: u64][data][padding]
        ulong len = *(ulong*)data;

        if (len > int.MaxValue)
            Environment.FailFast("Input size exceeds the maximum supported length");

        ulong alignedLen = (len + 7UL) & ~7UL;

        // Advance by padded length
        _inputPosition = checked(_inputPosition + sizeof(ulong) + alignedLen);

        // Return the length of actual data, not including the padding
        return new ReadOnlySpan<byte>(data + sizeof(ulong), (int)len);
    }

    public static void ReadReset() => _inputPosition = sizeof(ulong);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetOutput(int id, uint value)
    {
        if ((uint)id >= 64U)
            Environment.FailFast("Output id must be between 0 and 63");

        Output[(uint)id] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(char value) => *Uart = unchecked((byte)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string value)
    {
        if (value is null)
            return;

        for (int i = 0; i < value.Length; i++)
            *Uart = unchecked((byte)value[i]);
    }

    public static void WriteLine(string value)
    {
        Write(value);
        Write('\n');
    }
}
