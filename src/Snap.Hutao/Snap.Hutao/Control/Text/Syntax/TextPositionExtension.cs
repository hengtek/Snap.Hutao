// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax;

internal static class TextPositionExtension
{
    public static ReadOnlySpan<char> AsSpan(this string? text, TextPosition position)
    {
        return text.AsSpan(position.Start, position.Length);
    }
}