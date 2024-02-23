// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxTree
{
    public MiHoYoSyntaxNode Root { get; set; } = default!;

    public static MiHoYoSyntaxTree Parse(string text)
    {
        MiHoYoRootSyntax root = new(text, 0, text.Length);
        ParseComponents(text, root);

        MiHoYoSyntaxTree tree = new()
        {
            Root = root,
        };

        return tree;
    }

    private static void ParseComponents(string text, MiHoYoSyntaxNode syntax)
    {
        TextPosition contentPosition = syntax switch
        {
            MiHoYoRootSyntax rootSyntax => rootSyntax.ContentPosition,
            MiHoYoXmlElementSyntax xmlSyntax => xmlSyntax.ContentPosition,
            _ => syntax.Position,
        };

        ReadOnlySpan<char> contentSpan = text.AsSpan(contentPosition);

        int endOfProcessedAtContent = 0;
        while (true)
        {
            if (endOfProcessedAtContent >= contentSpan.Length)
            {
                return;
            }

            // <tag>some text</tag>
            // ↑   ↑         ↑    ↑
            // |   |         |    right closing
            // |   |         right opening
            // |   left closing
            // left opening
            int indexOfXmlLeftOpeningAtUnprocessedContent = contentSpan[endOfProcessedAtContent..].IndexOf('<');

            // End of content
            if (indexOfXmlLeftOpeningAtUnprocessedContent < 0)
            {
                TextPosition positionAtText = new(contentPosition.Start + endOfProcessedAtContent, contentPosition.End);
                ParseInterpolatedOrPlainText(text, syntax, positionAtText);
                return;
            }

            // Text between xml elements
            if (indexOfXmlLeftOpeningAtUnprocessedContent > 0)
            {
                TextPosition position = new(0, indexOfXmlLeftOpeningAtUnprocessedContent);
                TextPosition positionAtContent = position >> endOfProcessedAtContent;
                TextPosition positionAtText = positionAtContent >> contentPosition.Start;

                ParseInterpolatedOrPlainText(text, syntax, positionAtText);
                endOfProcessedAtContent = positionAtContent.End;
                continue;
            }

            // Peek the next character after left opening
            int indexOfXmlLeftOpeningAtContent = endOfProcessedAtContent + indexOfXmlLeftOpeningAtUnprocessedContent;
            switch (contentSpan[indexOfXmlLeftOpeningAtContent + 1])
            {
                case 'c':
                    {
                        int endOfXmlColorRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfXmlLeftOpeningAtContent..], out int endOfXmlColorLeftClosingAtUnprocessedContent);

                        MiHoYoColorKind colorKind = endOfXmlColorLeftClosingAtUnprocessedContent switch
                        {
                            17 => MiHoYoColorKind.Rgba,
                            15 => MiHoYoColorKind.Rgb,
                            _ => throw Must.NeverHappen(),
                        };

                        TextPosition position = new(0, endOfXmlColorRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position >> endOfProcessedAtContent;
                        TextPosition positionAtText = positionAtContent >> contentPosition.Start;

                        MiHoYoColorTextSyntax colorText = new(colorKind, text, positionAtText);
                        ParseComponents(text, colorText);
                        syntax.Children.Add(colorText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }

                case 'i':
                    {
                        int endOfXmlItalicRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfXmlLeftOpeningAtContent..], out _);

                        TextPosition position = new(0, endOfXmlItalicRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position >> endOfProcessedAtContent;
                        TextPosition positionAtText = positionAtContent >> contentPosition.Start;

                        MiHoYoItalicTextSyntax italicText = new(text, positionAtText);
                        ParseComponents(text, italicText);
                        syntax.Children.Add(italicText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }
            }
        }
    }

    private static void ParseInterpolatedOrPlainText(string text, MiHoYoSyntaxNode syntax, in TextPosition spanPosition)
    {
        if (text.AsSpan()[0] is not '#')
        {
            MiHoYoPlainTextSyntax plainText = new(text, spanPosition);
            syntax.Children.Add(plainText);
            return;
        }

        ReadOnlySpan<char> span = text.AsSpan(spanPosition);

        int endOfProcessedAtSpan = 0;
        while (true)
        {
            if (endOfProcessedAtSpan >= span.Length)
            {
                return;
            }

            int indexOfOpeningAtUnprocessedSpan = span[endOfProcessedAtSpan..].IndexOf('{');

            // End of content
            if (indexOfOpeningAtUnprocessedSpan < 0)
            {
                TextPosition positionAtText = new(spanPosition.Start + endOfProcessedAtSpan, spanPosition.End);

                MiHoYoPlainTextSyntax plainText = new(text, positionAtText);
                syntax.Children.Add(plainText);
                break;
            }

            // Text between interpolation
            if (indexOfOpeningAtUnprocessedSpan > 0)
            {
                TextPosition position = new(0, indexOfOpeningAtUnprocessedSpan);
                TextPosition positionAtSpan = position >> endOfProcessedAtSpan;
                TextPosition positionAtText = positionAtSpan >> spanPosition.Start;

                MiHoYoPlainTextSyntax plainText = new(text, positionAtText);
                syntax.Children.Add(plainText);
                endOfProcessedAtSpan = positionAtSpan.End;
                continue;
            }

            // Peek the next character after left opening
            int indexOfOpeningAtSpan = endOfProcessedAtSpan + indexOfOpeningAtUnprocessedSpan;
            int endOfClosingAtUnprocessedSpan = span[indexOfOpeningAtSpan..].IndexOf('}') + 1;

            TextPosition position2 = new(0, endOfClosingAtUnprocessedSpan);
            TextPosition positionAtSpan2 = position2 >> endOfProcessedAtSpan;
            TextPosition positionAtText2 = positionAtSpan2 >> spanPosition.Start;

            switch (span[indexOfOpeningAtSpan + 1])
            {
                // {F#(.*?)}
                case 'F':
                    {
                        MiHoYoInterpolatedContentTextSyntax contentTextSyntax = new(MiHoYoInterpolationKind.Female, text, positionAtText2);
                        syntax.Children.Add(contentTextSyntax);
                        endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                        break;
                    }

                // {LAYOUT_MOBILE#(.+?)}
                // {LAYOUT_PC#(.+?)}
                // {LAYOUT_PS#(.+?)}
                case 'L':
                    {
                        MiHoYoInterpolationKind kind = span[indexOfOpeningAtSpan + 9] switch
                        {
                            'O' => MiHoYoInterpolationKind.LayoutMobile,
                            'P' => MiHoYoInterpolationKind.LayoutPC,
                            'S' => MiHoYoInterpolationKind.LayoutPS,
                            _ => throw Must.NeverHappen($"Unexpected span content {span}"),
                        };

                        MiHoYoInterpolatedContentTextSyntax contentTextSyntax = new(kind, text, positionAtText2);
                        syntax.Children.Add(contentTextSyntax);
                        endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                        break;
                    }

                // {MATEAVATAR#SEXPRO[INFO_MALE_PRONOUN_BOYD|INFO_FEMALE_PRONOUN_GIRLD]}
                // {MATEAVATAR#SEXPRO[INFO_MALE_PRONOUN_BOYFIRST|INFO_FEMALE_PRONOUN_GIRLFIRST]}
                // {M#(.*?)}
                case 'M':
                    {
                        switch (span[indexOfOpeningAtSpan + 2])
                        {
                            case 'A':
                                {
                                    MiHoYoInterpolationKind kind = endOfClosingAtUnprocessedSpan switch
                                    {
                                        69 => MiHoYoInterpolationKind.MateAvatarSexProInfoMaleFemalePronounBoyGirlD,
                                        77 => MiHoYoInterpolationKind.MateAvatarSexProInfoMaleFemalePronounBoyGirlFirst,
                                        _ => throw Must.NeverHappen($"Unexpected span content {span}"),
                                    };

                                    MiHoYoInterpolatedTextSyntax textSyntax = new(kind, text, positionAtText2);
                                    syntax.Children.Add(textSyntax);
                                    endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                                    break;
                                }

                            case '#':
                                {
                                    MiHoYoInterpolatedContentTextSyntax contentTextSyntax = new(MiHoYoInterpolationKind.Male, text, positionAtText2);
                                    syntax.Children.Add(contentTextSyntax);
                                    endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                                    break;
                                }

                            default:
                                throw Must.NeverHappen($"Unexpected span content {span}");
                        }

                        break;
                    }

                // {NICKNAME}
                // {NON_BREAK_SPACE}
                case 'N':
                    {
                        MiHoYoInterpolationKind kind = span[indexOfOpeningAtSpan + 2] switch
                        {
                            'I' => MiHoYoInterpolationKind.Nickname,
                            'O' => MiHoYoInterpolationKind.NonBreakSpace,
                            _ => throw Must.NeverHappen($"Unexpected span content {span}"),
                        };

                        MiHoYoInterpolatedTextSyntax textSyntax = new(kind, text, positionAtText2);
                        syntax.Children.Add(textSyntax);
                        endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                        break;
                    }

                // {PLAYERAVATAR#SEXPRO[INFO_MALE_PRONOUN_HE|INFO_FEMALE_PRONOUN_SHE]}
                case 'P':
                    {
                        MiHoYoInterpolatedContentTextSyntax contentTextSyntax = new(MiHoYoInterpolationKind.PlayerAvatarSexProInfoMaleFemalePronounHeShe, text, positionAtText2);
                        syntax.Children.Add(contentTextSyntax);
                        endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                        break;
                    }

                // {REALNAME[ID(1)|HOSTONLY(true)]}
                // {REALNAME[ID(1)]}
                case 'R':
                    {
                        MiHoYoInterpolationKind kind = endOfClosingAtUnprocessedSpan switch
                        {
                            32 => MiHoYoInterpolationKind.RealNameId1HostOnlyTrue,
                            17 => MiHoYoInterpolationKind.RealNameId1,
                            _ => throw Must.NeverHappen($"Unexpected span content {span}"),
                        };

                        MiHoYoInterpolatedTextSyntax textSyntax = new(kind, text, positionAtText2);
                        syntax.Children.Add(textSyntax);
                        endOfProcessedAtSpan += endOfClosingAtUnprocessedSpan;
                        break;
                    }

                default:
                    throw Must.NeverHappen($"Unexpected span content {span}");
            }
        }
    }

    private static int EndOfXmlClosing(in ReadOnlySpan<char> span, out int endOfLeftClosing)
    {
        endOfLeftClosing = 0;

        int openingCount = 0;
        int closingCount = 0;

        int current = 0;

        // Considering <i>text1</i>text2<i>text3</i>
        // Considering <i>text1<span>text2</span>text3</i>
        while (true)
        {
            int leftMarkIndex = span[current..].IndexOf('<');
            if (span[current..][leftMarkIndex + 1] is '/')
            {
                closingCount++;
            }
            else
            {
                openingCount++;
            }

            current += span[current..].IndexOf('>') + 1;

            if (openingCount is 1 && closingCount is 0)
            {
                endOfLeftClosing = current;
            }

            if (openingCount == closingCount)
            {
                return current;
            }
        }
    }
}