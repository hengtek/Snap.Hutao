// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoRootSyntax : MiHoYoSyntaxNode
{
    public MiHoYoRootSyntax(string text, int start, int end)
        : base(MiHoYoSyntaxKind.Root, text, start, end)
    {
    }

    public bool IsInterpolated { get => Text.AsSpan()[0] is '#'; }

    public TextPosition ContentPosition { get => IsInterpolated ? new(Position.Start + 1, Position.End) : Position; }
}