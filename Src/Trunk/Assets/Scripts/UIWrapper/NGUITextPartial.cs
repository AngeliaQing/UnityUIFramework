#if !UNITY_3_5
#define DYNAMIC_FONT
#endif

// 为实现图文混排做的定制 liugs
// NOTE !!!!!!
// 需要修改GetGlyphWidth()和GetGlyph()两个方法，所以把这两个方法从NGUIText.cs中拷到这里再改，原来的实现注释掉
#define NGUI_CUSTOM

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public partial class NGUIText
{

#if NGUI_CUSTOM
    static public float symbolFontScale = 1f;
#endif

    /// <summary>
    /// Get the width of the specified glyph. Returns zero if the glyph could not be retrieved.
    /// </summary>

    static public float GetGlyphWidth(int ch, int prev, float fontScale)
    {
#if NGUI_CUSTOM
        if (false)
#else
        if (bitmapFont != null)
#endif
        {
            bool thinSpace = false;

            if (ch == '\u2009')
            {
                thinSpace = true;
                ch = ' ';
            }

            BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

            if (bmg != null)
            {
                int adv = bmg.advance;
                if (thinSpace) adv >>= 1;
                return fontScale * ((prev != 0) ? adv + bmg.GetKerning(prev) : bmg.advance);
            }
        }
#if DYNAMIC_FONT
        else if (dynamicFont != null)
        {
            if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
                    return mTempChar.width * fontScale * pixelDensity;
#else
                return mTempChar.advance * fontScale * pixelDensity;
#endif
        }
#endif
        return 0f;
    }

    /// <summary>
    /// Get the specified glyph.
    /// </summary>

    static public GlyphInfo GetGlyph(int ch, int prev, float fontScale = 1f)
    {
#if NGUI_CUSTOM
        if (false)
#else
        if (bitmapFont != null)
#endif
        {
            bool thinSpace = false;

            if (ch == '\u2009')
            {
                thinSpace = true;
                ch = ' ';
            }

            BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

            if (bmg != null)
            {
                int kern = (prev != 0) ? bmg.GetKerning(prev) : 0;
                glyph.v0.x = (prev != 0) ? bmg.offsetX + kern : bmg.offsetX;
                glyph.v1.y = -bmg.offsetY;

                glyph.v1.x = glyph.v0.x + bmg.width;
                glyph.v0.y = glyph.v1.y - bmg.height;

                glyph.u0.x = bmg.x;
                glyph.u0.y = bmg.y + bmg.height;

                glyph.u2.x = bmg.x + bmg.width;
                glyph.u2.y = bmg.y;

                glyph.u1.x = glyph.u0.x;
                glyph.u1.y = glyph.u2.y;

                glyph.u3.x = glyph.u2.x;
                glyph.u3.y = glyph.u0.y;

                int adv = bmg.advance;
                if (thinSpace) adv >>= 1;
                glyph.advance = adv + kern;
                glyph.channel = bmg.channel;

                if (fontScale != 1f)
                {
                    glyph.v0 *= fontScale;
                    glyph.v1 *= fontScale;
                    glyph.advance *= fontScale;
                }
                return glyph;
            }
        }
#if DYNAMIC_FONT
        else if (dynamicFont != null)
        {
            if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
            {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
                    glyph.v0.x = mTempChar.vert.xMin;
                    glyph.v1.x = glyph.v0.x + mTempChar.vert.width;

                    glyph.v0.y = mTempChar.vert.yMax - baseline;
                    glyph.v1.y = glyph.v0.y - mTempChar.vert.height;

                    glyph.u0.x = mTempChar.uv.xMin;
                    glyph.u0.y = mTempChar.uv.yMin;

                    glyph.u2.x = mTempChar.uv.xMax;
                    glyph.u2.y = mTempChar.uv.yMax;

                    if (mTempChar.flipped)
                    {
                        glyph.u1 = new Vector2(glyph.u2.x, glyph.u0.y);
                        glyph.u3 = new Vector2(glyph.u0.x, glyph.u2.y);
                    }
                    else
                    {
                        glyph.u1 = new Vector2(glyph.u0.x, glyph.u2.y);
                        glyph.u3 = new Vector2(glyph.u2.x, glyph.u0.y);
                    }

                    glyph.advance = mTempChar.width;
                    glyph.channel = 0;
#else
                glyph.v0.x = mTempChar.minX;
                glyph.v1.x = mTempChar.maxX;

                glyph.v0.y = mTempChar.maxY - baseline;
                glyph.v1.y = mTempChar.minY - baseline;

                glyph.u0 = mTempChar.uvTopLeft;
                glyph.u1 = mTempChar.uvBottomLeft;
                glyph.u2 = mTempChar.uvBottomRight;
                glyph.u3 = mTempChar.uvTopRight;

                glyph.advance = mTempChar.advance;
                glyph.channel = 0;
#endif
                glyph.v0.x = Mathf.Round(glyph.v0.x);
                glyph.v0.y = Mathf.Round(glyph.v0.y);
                glyph.v1.x = Mathf.Round(glyph.v1.x);
                glyph.v1.y = Mathf.Round(glyph.v1.y);

                float pd = fontScale * pixelDensity;

                if (pd != 1f)
                {
                    glyph.v0 *= pd;
                    glyph.v1 *= pd;
                    glyph.advance *= pd;
                }
                return glyph;
            }
        }
#endif
        return null;
    }

#if NGUI_CUSTOM
    static public bool Print(string text, List<Vector3> verts, List<Vector2> uvs, List<Color> cols, bool show_symbol)
    {
        if (string.IsNullOrEmpty(text)) return true;

        int indexOffset = verts.Count;
        Prepare(text);
        if (indexOffset != verts.Count)
        {
            //Prepare->dynamicFont.RequestCharactersInTexture->UILableSymbolCustom.OnFontChange->OnFill->Print 会引起递归重入
            //避免因递归重入弄脏vers,导致lable绘制时出现重影,偏移等问题.
            return false;
        }
        // Start with the white tint
        mColors.Add(Color.white);
        mAlpha = 1f;

        int ch = 0, prev = 0;
        float x = 0f, y = 0f, maxX = 0f;
        float sizeF = finalSize;

        Color gb = tint * gradientBottom;
        Color gt = tint * gradientTop;
        Color32 uc = tint;
        int textLength = text.Length;

        //Rect uvRect = new Rect();
        //float invX = 0f, invY = 0f;
        float sizePD = sizeF * pixelDensity;

        // Advanced symbol support contributed by Rudy Pangestu.
        bool subscript = false;
        int subscriptMode = 0;  // 0 = normal, 1 = subscript, 2 = superscript
        bool bold = false;
        bool italic = false;
        bool underline = false;
        bool strikethrough = false;
        bool ignoreColor = false;
        const float sizeShrinkage = 0.75f;

        float v0x;
        float v1x;
        float v1y;
        float v0y;
        float prevX = 0;

        //if (bitmapFont != null)
        //{
        //    uvRect = bitmapFont.uvRect;
        //    invX = uvRect.width / bitmapFont.texWidth;
        //    invY = uvRect.height / bitmapFont.texHeight;
        //}

        for (int i = 0; i < textLength; ++i)
        {
            ch = text[i];

            prevX = x;

            // New line character -- skip to the next line
            if (ch == '\n')
            {
                if (x > maxX) maxX = x;

                if (alignment != Alignment.Left)
                {
                    Align(verts, indexOffset, x - finalSpacingX);
                    indexOffset = verts.Count;
                }

                x = 0;
                y += finalLineHeight;
                prev = 0;
                continue;
            }

            // Invalid character -- skip it
            if (ch < ' ')
            {
                prev = ch;
                continue;
            }

            // Color changing symbol
            if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref subscriptMode, ref bold,
                ref italic, ref underline, ref strikethrough, ref ignoreColor))
            {
                Color fc;

                if (ignoreColor)
                {
                    fc = mColors[mColors.size - 1];
                    fc.a *= mAlpha * tint.a;
                }
                else
                {
                    fc = tint * mColors[mColors.size - 1];
                    fc.a *= mAlpha;
                }
                uc = fc;

                for (int b = 0, bmax = mColors.size - 2; b < bmax; ++b)
                    fc.a *= mColors[b].a;

                if (gradient)
                {
                    gb = gradientBottom * fc;
                    gt = gradientTop * fc;
                }
                --i;
                continue;
            }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

            if (symbol != null)
            {
                v0x = x + symbol.offsetX * symbolFontScale;
                v1x = v0x + symbol.width * symbolFontScale;
                v1y = -(y + symbol.offsetY * symbolFontScale);
                v0y = v1y - symbol.height * symbolFontScale;

                // Doesn't fit? Move down to the next line
                if (Mathf.RoundToInt(x + symbol.advance * symbolFontScale) > regionWidth)
                {
                    if (x == 0f) return true;

                    if (alignment != Alignment.Left && indexOffset < verts.Count)
                    {
                        Align(verts, indexOffset, x - finalSpacingX);
                        indexOffset = verts.Count;
                    }

                    v0x -= x;
                    v1x -= x;
                    v0y -= finalLineHeight;
                    v1y -= finalLineHeight;

                    x = 0;
                    y += finalLineHeight;
                    prevX = 0;
                }

                if (show_symbol)
                {
                    verts.Add(new Vector3(v0x, v0y));
                    verts.Add(new Vector3(v0x, v1y));
                    verts.Add(new Vector3(v1x, v1y));
                    verts.Add(new Vector3(v1x, v0y));
                }

                x += finalSpacingX + symbol.advance * symbolFontScale;
                i += symbol.length - 1;
                prev = 0;

                if (show_symbol && uvs != null)
                {
                    Rect uv = symbol.uvRect;

                    float u0x = uv.xMin;
                    float u0y = uv.yMin;
                    float u1x = uv.xMax;
                    float u1y = uv.yMax;

                    uvs.Add(new Vector2(u0x, u0y));
                    uvs.Add(new Vector2(u0x, u1y));
                    uvs.Add(new Vector2(u1x, u1y));
                    uvs.Add(new Vector2(u1x, u0y));
                }

                if (show_symbol && cols != null)
                {
                    if (symbolStyle == SymbolStyle.Colored)
                    {
                        for (int b = 0; b < 4; ++b) cols.Add(uc);
                    }
                    else
                    {
                        Color32 col = Color.white;
                        col.a = uc.a;
                        for (int b = 0; b < 4; ++b) cols.Add(col);
                    }
                }
            }
            else // No symbol present
            {
                GlyphInfo glyph = GetGlyph(ch, prev);
                if (glyph == null) continue;
                prev = ch;

                if (subscriptMode != 0)
                {
                    glyph.v0.x *= sizeShrinkage;
                    glyph.v0.y *= sizeShrinkage;
                    glyph.v1.x *= sizeShrinkage;
                    glyph.v1.y *= sizeShrinkage;

                    if (subscriptMode == 1)
                    {
                        glyph.v0.y -= fontScale * fontSize * 0.4f;
                        glyph.v1.y -= fontScale * fontSize * 0.4f;
                    }
                    else
                    {
                        glyph.v0.y += fontScale * fontSize * 0.05f;
                        glyph.v1.y += fontScale * fontSize * 0.05f;
                    }
                }

                v0x = glyph.v0.x + x;
                v0y = glyph.v0.y - y;
                v1x = glyph.v1.x + x;
                v1y = glyph.v1.y - y;

                float w = glyph.advance;
                if (finalSpacingX < 0f) w += finalSpacingX;

                // Doesn't fit? Move down to the next line
                if (Mathf.RoundToInt(x + w) > regionWidth)
                {
                    if (x == 0f) return true;

                    if (alignment != Alignment.Left && indexOffset < verts.Count)
                    {
                        Align(verts, indexOffset, x - finalSpacingX);
                        indexOffset = verts.Count;
                    }

                    v0x -= x;
                    v1x -= x;
                    v0y -= finalLineHeight;
                    v1y -= finalLineHeight;

                    x = 0;
                    y += finalLineHeight;
                    prevX = 0;
                }

                if (IsSpace(ch))
                {
                    if (underline)
                    {
                        ch = '_';
                    }
                    else if (strikethrough)
                    {
                        ch = '-';
                    }
                }

                // Advance the position
                x += (subscriptMode == 0) ? finalSpacingX + glyph.advance :
                    (finalSpacingX + glyph.advance) * sizeShrinkage;

                // No need to continue if this is a space character
                if (IsSpace(ch)) continue;

                // Texture coordinates
                if (!show_symbol && uvs != null)
                {
                    //if (bitmapFont != null)
                    //{
                    //    glyph.u0.x = uvRect.xMin + invX * glyph.u0.x;
                    //    glyph.u2.x = uvRect.xMin + invX * glyph.u2.x;
                    //    glyph.u0.y = uvRect.yMax - invY * glyph.u0.y;
                    //    glyph.u2.y = uvRect.yMax - invY * glyph.u2.y;

                    //    glyph.u1.x = glyph.u0.x;
                    //    glyph.u1.y = glyph.u2.y;

                    //    glyph.u3.x = glyph.u2.x;
                    //    glyph.u3.y = glyph.u0.y;
                    //}

                    for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                    {
                        uvs.Add(glyph.u0);
                        uvs.Add(glyph.u1);
                        uvs.Add(glyph.u2);
                        uvs.Add(glyph.u3);
                    }
                }

                // Vertex colors
                if (!show_symbol && cols != null)
                {
                    if (glyph.channel == 0 || glyph.channel == 15)
                    {
                        if (gradient)
                        {
                            float min = sizePD + glyph.v0.y / fontScale;
                            float max = sizePD + glyph.v1.y / fontScale;

                            min /= sizePD;
                            max /= sizePD;

                            s_c0 = Color.Lerp(gb, gt, min);
                            s_c1 = Color.Lerp(gb, gt, max);

                            for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                            {
                                cols.Add(s_c0);
                                cols.Add(s_c1);
                                cols.Add(s_c1);
                                cols.Add(s_c0);
                            }
                        }
                        else
                        {
                            for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
                                cols.Add(uc);
                        }
                    }
                    else
                    {
                        // Packed fonts come as alpha masks in each of the RGBA channels.
                        // In order to use it we need to use a special shader.
                        //
                        // Limitations:
                        // - Effects (drop shadow, outline) will not work.
                        // - Should not be a part of the atlas (eastern fonts rarely are anyway).
                        // - Lower color precision

                        Color col = uc;

                        col *= 0.49f;

                        switch (glyph.channel)
                        {
                            case 1: col.b += 0.51f; break;
                            case 2: col.g += 0.51f; break;
                            case 4: col.r += 0.51f; break;
                            case 8: col.a += 0.51f; break;
                        }

                        Color32 c = col;
                        for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
                            cols.Add(c);
                    }
                }

                if (!show_symbol)
                {
                    // Bold and italic contributed by Rudy Pangestu.
                    if (!bold)
                    {
                        if (!italic)
                        {
                            verts.Add(new Vector3(v0x, v0y));
                            verts.Add(new Vector3(v0x, v1y));
                            verts.Add(new Vector3(v1x, v1y));
                            verts.Add(new Vector3(v1x, v0y));
                        }
                        else // Italic
                        {
                            float slant = fontSize * 0.1f * ((v1y - v0y) / fontSize);
                            verts.Add(new Vector3(v0x - slant, v0y));
                            verts.Add(new Vector3(v0x + slant, v1y));
                            verts.Add(new Vector3(v1x + slant, v1y));
                            verts.Add(new Vector3(v1x - slant, v0y));
                        }
                    }
                    else // Bold
                    {
                        for (int j = 0; j < 4; ++j)
                        {
                            float a = mBoldOffset[j * 2];
                            float b = mBoldOffset[j * 2 + 1];

                            float slant = (italic ? fontSize * 0.1f * ((v1y - v0y) / fontSize) : 0f);
                            verts.Add(new Vector3(v0x + a - slant, v0y + b));
                            verts.Add(new Vector3(v0x + a + slant, v1y + b));
                            verts.Add(new Vector3(v1x + a + slant, v1y + b));
                            verts.Add(new Vector3(v1x + a - slant, v0y + b));
                        }
                    }

                    // Underline and strike-through contributed by Rudy Pangestu.
                    if (underline || strikethrough)
                    {
                        GlyphInfo dash = GetGlyph(strikethrough ? '-' : '_', prev);
                        if (dash == null) continue;

                        if (uvs != null)
                        {
                            //if (bitmapFont != null)
                            //{
                            //    dash.u0.x = uvRect.xMin + invX * dash.u0.x;
                            //    dash.u2.x = uvRect.xMin + invX * dash.u2.x;
                            //    dash.u0.y = uvRect.yMax - invY * dash.u0.y;
                            //    dash.u2.y = uvRect.yMax - invY * dash.u2.y;
                            //}

                            float cx = (dash.u0.x + dash.u2.x) * 0.5f;

                            for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                            {
                                uvs.Add(new Vector2(cx, dash.u0.y));
                                uvs.Add(new Vector2(cx, dash.u2.y));
                                uvs.Add(new Vector2(cx, dash.u2.y));
                                uvs.Add(new Vector2(cx, dash.u0.y));
                            }
                        }

                        if (subscript && strikethrough)
                        {
                            v0y = (-y + dash.v0.y) * sizeShrinkage;
                            v1y = (-y + dash.v1.y) * sizeShrinkage;
                        }
                        else
                        {
                            v0y = (-y + dash.v0.y);
                            v1y = (-y + dash.v1.y);
                        }

                        if (bold)
                        {
                            for (int j = 0; j < 4; ++j)
                            {
                                float a = mBoldOffset[j * 2];
                                float b = mBoldOffset[j * 2 + 1];

                                verts.Add(new Vector3(prevX + a, v0y + b));
                                verts.Add(new Vector3(prevX + a, v1y + b));
                                verts.Add(new Vector3(x + a, v1y + b));
                                verts.Add(new Vector3(x + a, v0y + b));
                            }
                        }
                        else
                        {
                            verts.Add(new Vector3(prevX, v0y));
                            verts.Add(new Vector3(prevX, v1y));
                            verts.Add(new Vector3(x, v1y));
                            verts.Add(new Vector3(x, v0y));
                        }

                        if (gradient)
                        {
                            float min = sizePD + dash.v0.y / fontScale;
                            float max = sizePD + dash.v1.y / fontScale;

                            min /= sizePD;
                            max /= sizePD;

                            s_c0 = Color.Lerp(gb, gt, min);
                            s_c1 = Color.Lerp(gb, gt, max);

                            for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                            {
                                cols.Add(s_c0);
                                cols.Add(s_c1);
                                cols.Add(s_c1);
                                cols.Add(s_c0);
                            }
                        }
                        else
                        {
                            for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
                                cols.Add(uc);
                        }
                    }
                }
            }
        }

        if (alignment != Alignment.Left && indexOffset < verts.Count)
        {
            Align(verts, indexOffset, x - finalSpacingX);
            indexOffset = verts.Count;
        }
        mColors.Clear();
        return true;
    }
#endif
}
