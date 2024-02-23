// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal class MiHoYoInterpolatedTextSyntax : MiHoYoSyntaxNode
{
    public MiHoYoInterpolatedTextSyntax(MiHoYoInterpolationKind interpolationKind, string text, in TextPosition position)
        : base(MiHoYoSyntaxKind.InterpolatedText, text, position)
    {
        InterpolationKind = interpolationKind;
    }

    public MiHoYoInterpolationKind InterpolationKind { get; }
}

internal enum MiHoYoInterpolationKind
{
    None,
    Female,
    LayoutMobile,
    LayoutPC,
    LayoutPS,
    Male,
    MateAvatarSexProInfoMaleFemalePronounBoyGirlD,
    MateAvatarSexProInfoMaleFemalePronounBoyGirlFirst,
    Nickname,
    NonBreakSpace,
    PlayerAvatarSexProInfoMaleFemalePronounHeShe,
    RealNameId1,
    RealNameId1HostOnlyTrue,
}