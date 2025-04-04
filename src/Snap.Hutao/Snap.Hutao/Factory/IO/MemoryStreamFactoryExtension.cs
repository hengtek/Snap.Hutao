// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Factory.IO;

internal static class MemoryStreamFactoryExtension
{
    public static async ValueTask<MemoryStream> GetStreamAsync(this IMemoryStreamFactory memoryStreamFactory, Stream stream, bool resetSourcePosition = false)
    {
        MemoryStream targetStream = memoryStreamFactory.GetStream();
        await stream.CopyToAsync(targetStream).ConfigureAwait(false);
        targetStream.Position = 0;
        if (resetSourcePosition && stream.CanSeek)
        {
            stream.Position = 0;
        }

        return targetStream;
    }
}