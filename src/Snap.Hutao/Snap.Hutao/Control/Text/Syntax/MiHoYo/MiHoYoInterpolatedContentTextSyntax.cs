// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoInterpolatedContentTextSyntax : MiHoYoInterpolatedTextSyntax
{
    public MiHoYoInterpolatedContentTextSyntax(MiHoYoInterpolationKind interpolationKind, string text, in TextPosition position)
        : base(interpolationKind, text, position)
    {
    }

    public TextPosition ContentPosition { get; }

    public ReadOnlySpan<char> ContentSpan => Text.AsSpan(ContentPosition);
}