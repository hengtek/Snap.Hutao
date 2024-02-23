﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoPlainTextSyntax : MiHoYoSyntaxNode
{
    public MiHoYoPlainTextSyntax(string text, in TextPosition position)
        : base(MiHoYoSyntaxKind.PlainText, text, position)
    {
    }
}