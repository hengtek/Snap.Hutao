// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HighQuality]
internal sealed class UidGameToken : IMappingFrom<UidGameToken, ComboTokenWrapper>
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("token")]
    public string GameToken { get; set; } = default!;

    public static UidGameToken From(ComboTokenWrapper wrapper) => new()
    {
        Uid = wrapper.OpenId,
        GameToken = wrapper.OpenToken,
    };
}
