// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class ComboTokenWrapper
{
    [JsonPropertyName("open_id")]
    public string OpenId { get; set; } = default!;

    [JsonPropertyName("open_token")]
    public string OpenToken { get; set; } = default!;
}
