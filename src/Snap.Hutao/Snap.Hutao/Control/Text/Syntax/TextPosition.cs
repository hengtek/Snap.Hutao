// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Control.Text.Syntax;

[DebuggerDisplay("[{Start}..{End}]")]
internal readonly struct TextPosition
{
    public readonly int Start;
    public readonly int End;

    public TextPosition(int start, int end)
    {
        Start = start;
        End = end;
    }

    public readonly int Length
    {
        get => End - Start;
    }

    public static TextPosition operator <<(TextPosition position, int offset)
    {
        return new(position.Start - offset, position.End - offset);
    }

    public static TextPosition operator >>(TextPosition position, int offset)
    {
        return new(position.Start + offset, position.End + offset);
    }
}