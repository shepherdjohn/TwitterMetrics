//  The MIT License (MIT)
//  Copyright (c) 2019 Linus Birgerstam
//    
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:
//    
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//    
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmojiOne {

    /// <summary>
    /// Helper class for converting emoji to different formats.
    /// </summary>
    public static partial class EmojiOne {

        /// <summary>
        /// Used only to direct CDN path for non-sprite PNG usage. Available options are 32, 64, and 128.
        /// </summary>
        public static int EmojiSize { get; set; } = 32;

        /// <summary>
        /// Used only to direct CDN path. This is a 2-digit version (e.g. 3.1). Not recommended for usage below 3.0.
        /// </summary>
        public static string EmojiVersion { get; set; } = "4.0";

        /// <summary>
        /// Defaults to .png. Set to .svg when using premium local assets.
        /// </summary>
        public static string FileExtension { get; set; } = ".png";

        /// <summary>
        /// CDN (jsdeliver) path. 
        /// </summary>
        private static string DefaultPath = "https://cdn.jsdelivr.net/emojione/assets/" + EmojiVersion + "/";

        /// <summary>
        /// Defaults to CDN (jsdeliver) path. Change this when using premium local assets.
        /// </summary>
        public static string ImagePath { get; set; } = DefaultPath;

        /// <summary>
        /// Gets or sets a value indicating whether output images should have a title attribute or not. Default is <c>true</c>.
        /// </summary>
        public static bool ImageTitleTag { get; set; } = true;

        /// <summary>
        /// Takes an input string containing both native unicode emoji and shortnames, and translates it into emoji images for display.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="ascii"><c>true</c> to also convert ascii emoji to images.</param>
        /// <param name="unicodeAlt"><c>true</c> to use the unicode char instead of the shortname as the alt attribute (makes copy and pasting the resulting text better).</param>
        /// <param name="sprite"><c>true</c> to enable sprite mode instead of individual images.</param>
        /// <returns>A string with appropriate html for rendering emoji.</returns>
        public static string ToImage(string str, bool ascii = false, bool unicodeAlt = true, bool sprite = false) {
            // first pass changes unicode characters into emoji markup
            str = UnicodeToImage(str, unicodeAlt, sprite);
            // second pass changes any shortnames into emoji markup
            str = ShortnameToImage(str, ascii, unicodeAlt, sprite);
            return str;
        }

        /// <summary>
        /// Unifies all emoji to their standard unicode types. 
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="ascii"><c>true</c> to also convert ascii emoji to unicode.</param>
        /// <returns>A string with standardized unicode.</returns>
        public static string UnifyUnicode(string str, bool ascii = false) {
            // transform all unicode into a standard shortname
            str = ToShort(str);
            // then transform the shortnames into unicode
            str = ShortnameToUnicode(str, ascii);
            return str;
        }

        /// <summary>
        /// Converts shortname emojis to unicode, useful for sending emojis back to mobile devices.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="ascii"><c>true</c> to also convert ascii emoji in the inpur string to unicode.</param>
        /// <returns>A string with unicode replacements</returns>
        public static string ShortnameToUnicode(string str, bool ascii = false) {
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + SHORTNAME_PATTERN, match => {
                    var shortname = match.Value;
                    // check if the emoji exists in our dictionary
                    if (SHORTNAMES.ContainsKey(shortname)) {
                        // convert codepoint to unicode char
                        return ToUnicode(SHORTNAMES[shortname]);
                    }

                    // we didn't find a replacement so just return the entire match
                    return match.Value;

                }, RegexOptions.IgnoreCase);
            }
            if (ascii) {
                str = AsciiToUnicode(str);
            }
            return str;
        }

        /// <summary>
        /// This will replace shortnames with their ascii equivalent, e.g. :wink: -> ;). 
        /// This is useful for systems that don't support unicode or images.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>A string with ascii replacements.</returns>
        public static string ShortnameToAscii(string str) {
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + SHORTNAME_PATTERN, match => {
                    var shortname = match.Value;
                    // check if the emoji exists in our dictionary
                    if (SHORTNAMES.ContainsKey(shortname)) {
                        var emoji = GetEmoji(SHORTNAMES[shortname]);
                        if (emoji != null) {
                            var ascii = emoji[ASCII_INDEX];
                            if (ascii != null) {
                                return ascii;
                            }
                        }
                    }

                    // we didn't find a replacement so just return the entire match
                    return match.Value;

                }, RegexOptions.IgnoreCase);
            }
            return str;
        }

        /// <summary>
        /// Takes input containing emoji shortnames and converts it to emoji images.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ascii"><c>true</c> to also convert ascii emoji to images.</param>
        /// <param name="unicodeAlt"><c>true</c> to use the unicode char instead of the shortname as the alt attribute (makes copy and pasting the resulting text better).</param>
        /// <param name="sprite"><c>true</c> to enable sprite mode instead of individual images.</param>
        /// <returns>A string with appropriate html for rendering emoji.</returns>
        public static string ShortnameToImage(string str, bool ascii = false, bool unicodeAlt = true, bool sprite = false) {
            if (ascii) {
                str = AsciiToShortname(str);
            }
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + SHORTNAME_PATTERN, match => {
                    var shortname = match.Value;
                    // check if the emoji exists in our dictionary
                    if (SHORTNAMES.ContainsKey(shortname)) {
                        var codepoint = SHORTNAMES[shortname];
                        var emoji = GetEmoji(codepoint);
                        if (emoji != null) {
                            string title = ImageTitleTag ? $@" title=""{shortname}""" : "";
                            string alt = unicodeAlt ? ToUnicode(codepoint) : shortname;
                            if (sprite) {
                                string category = codepoint.IndexOf("-1f3f") >= 0 ? "diversity" : emoji[CATEGORY_INDEX];
                                return $@"<span class=""emojione emojione-{EmojiSize}-{category} _{codepoint}""{title}>{alt}</span>";
                            } else {
                                var path = DefaultPath != ImagePath ? ImagePath : DefaultPath + EmojiSize + "/";
                                return $@"<img class=""emojione"" alt=""{alt}""{title} src=""{path}{codepoint}{FileExtension}"" />";
                            }
                        }
                    }

                    // we didn't find a replacement so just return the entire match
                    return match.Value;
                }, RegexOptions.IgnoreCase);
            }
            return str;
        }

        /// <summary>
        /// Converts unicode emoji to shortnames.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>A string with shortname replacements.</returns>
        public static string ToShort(string str)
        {
            if (str != null)
            {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + UNICODE_PATTERN, match => {
                    // check if the emoji exists in our dictionary
                    var unicode = match.Groups[1].Value;
                    var codepoint = ToCodePoint(unicode);
                    var emoji = GetEmoji(codepoint);
                    if (emoji != null) {
                        return emoji[SHORTNAME_INDEX];
                    }

                    // we didn't find a replacement so just return the entire match
                    return match.Value;
                });
            }
            return str;
        }

        public static List<string> ToEmojiValue(string str)
        {
            List<string> shortNames = new List<string>();
            if (str != null)
            {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + UNICODE_PATTERN, match => {
                    // check if the emoji exists in our dictionary
                    var unicode = match.Groups[1].Value;
                    var codepoint = ToCodePoint(unicode);
                    var emoji = GetEmoji(codepoint);
                    if (emoji != null)
                    {
                        shortNames.Add(emoji[SHORTNAME_INDEX]);
                    }
                    return match.Value;

                    // we didn't find a replacement so just return the entire match
                   // return match.Value;
                });     
            }
            return shortNames;
        }


        /// <summary>
        /// Takes native unicode emoji input, such as that from your mobile device, and outputs image markup (png or svg).
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="unicodeAlt"><c>true</c> to use the unicode char instead of the shortname as the alt attribute (makes copy and pasting the resulting text better).</param>
        /// <param name="sprite"><c>true</c> to enable sprite mode instead of individual images.</param>
        /// <returns>A string with appropriate html for rendering emoji.</returns>
        public static string UnicodeToImage(string str, bool unicodeAlt = true, bool sprite = false) {
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + UNICODE_PATTERN, match => {
                    // check if the emoji exists in our dictionary
                    var codepoint = ToCodePoint(match.Groups[1].Value);
                    var emoji = GetEmoji(codepoint);
                    if (emoji != null) {
                        var shortname = emoji[SHORTNAME_INDEX];
                        string title = ImageTitleTag ? $@" title=""{shortname}""" : "";
                        string alt = unicodeAlt ? ToUnicode(codepoint) : shortname;
                        if (sprite) {
                            string category = codepoint.IndexOf("-1f3f") >= 0 ? "diversity" :emoji[CATEGORY_INDEX];
                            return $@"<span class=""emojione emojione-{EmojiSize}-{category} _{codepoint}""{title}>{alt}</span>";
                        } else {
                            var path = DefaultPath != ImagePath ? ImagePath : DefaultPath + EmojiSize + "/";
                            return $@"<img class=""emojione"" alt=""{alt}""{title} src=""{path}{codepoint}{FileExtension}"" />";
                        }
                    }

                    // we didn't find a replacement so just return the entire match
                    return match.Value;
                });
            }
            return str;
        }

        /// <summary>
        /// Converts ascii emoji to unicode, e.g. :) -> 😄
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AsciiToUnicode(string str) {
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + ASCII_PATTERN, match => {
                    // check if the emoji exists in our dictionary
                    var ascii = match.Value;
                    if (ASCII.ContainsKey(ascii)) {
                        // convert codepoint to unicode char
                        return ToUnicode(ASCII[ascii]);
                    }
                    // we didn't find a replacement so just return the entire match
                    return match.Value;
                });
            }
            return str;
        }

        /// <summary>
        /// Converts ascii emoji to shortname, e.g. :) -> :smile:
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AsciiToShortname(string str) {
            if (str != null) {
                str = Regex.Replace(str, IGNORE_PATTERN + "|" + ASCII_PATTERN, match => {
                    // check if the emoji exists in our dictionaries
                    var ascii = match.Value;
                    if (ASCII.ContainsKey(ascii)) {
                        var codepoint = ASCII[ascii];
                        var emoji = GetEmoji(codepoint);
                        if (emoji != null) {
                            return emoji[SHORTNAME_INDEX];
                        }
                    }
                    // we didn't find a replacement so just return the entire match
                    return match.Value;
                });
            }
            return str;
        }

        /// <summary>
        /// Get emoji array for the specified codepoint.
        /// </summary>
        /// <param name="codepoint"></param>
        /// <returns></returns>
        internal static string[] GetEmoji(string codepoint) {
            if (EMOJI.ContainsKey(codepoint)) {
                return EMOJI[codepoint];
            }
            if (ALTERNATES.ContainsKey(codepoint) && EMOJI.ContainsKey(ALTERNATES[codepoint])) {
                return EMOJI[ALTERNATES[codepoint]];
            }
            return null;
        }

        /// <summary>
        /// Convert a unicode character to its code point/code pair(s)
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        internal static string ToCodePoint(string unicode) {
            string codepoint = "";
            for (var i = 0; i < unicode.Length; i += char.IsSurrogatePair(unicode, i) ? 2 : 1) {
                if (i > 0) {
                    codepoint += "-";
                }
                codepoint += string.Format("{0:X4}", char.ConvertToUtf32(unicode, i));
            }
            return codepoint.ToLower();
        }

        /// <summary>
        /// Converts unicode code point/code pair(s) to a unicode character.
        /// </summary>
        /// <param name="codepoint"></param>
        /// <returns></returns>
        internal static string ToUnicode(string codepoint) {
            if (codepoint.Contains('-')) {
                var pair = codepoint.Split('-');
                string[] hilos = new string[pair.Length];
                char[] chars = new char[pair.Length];
                for (int i = 0; i < pair.Length; i++) {
                    var part = Convert.ToInt32(pair[i], 16);
                    if (part >= 0x10000 && part <= 0x10FFFF) {
                        var hi = Math.Floor((decimal)(part - 0x10000) / 0x400) + 0xD800;
                        var lo = ((part - 0x10000) % 0x400) + 0xDC00;
                        hilos[i] = new string(new char[] { (char)hi, (char)lo });
                    } else {
                        chars[i] = (char)part;
                    }
                }
                if (hilos.Any(x => x != null)) {
                    return string.Concat(hilos);
                } else {
                    return new string(chars);
                }

            } else {
                var i = Convert.ToInt32(codepoint, 16);
                return char.ConvertFromUtf32(i);
            }
        }
    }

}
