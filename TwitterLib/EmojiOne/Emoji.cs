using Newtonsoft.Json;
using System.Collections.Generic;

namespace Codegen {

    // example from emoji.json
    //
    // "1f642": {
    //     "name": "slightly smiling face",
    //     "unicode_version": 7,
    //     "category": "people",
    //     "order": 1114,
    //     "display": 1,
    //     "shortname": ":slight_smile:",
    //     "shortname_alternates": [
    //         ":slightly_smiling_face:"
    //     ],
    //     "ascii": [
    //         ":)",
    //         ":-)",
    //         "=]",
    //         "=)",
    //         ":]"
    //     ],
    //     "diversity": null,
    //     "diversities": [],
    //     "gender": null,
    //     "genders": [],
    //     "code_points": {
    //         "base": "1f642",
    //         "fully_qualified": "1f642",
    //         "non_fully_qualified": "1f642",
    //         "output": "1f642",
    //         "default_matches": [
    //             "1f642"
    //         ],
    //         "greedy_matches": [
    //             "1f642"
    //         ],
    //         "decimal": ""
    //     },
    //     "keywords": [
    //         "face",
    //         "smile"
    //     ]

    /// <summary>
    /// 
    /// </summary>
    public class Emoji {

        /// <summary>
        /// 
        /// </summary>
        public Emoji() {
            ShortnameAlternates = new string[0];
            Ascii = new string[0];
            Diversities = new string[0];
            Genders = new string[0];
            CodePoints = new CodePoints();
            Keywords = new string[0];
        }

        /// <summary>
        /// The emoji name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Floating-point number indicating initial Unicode release.
        /// </summary>
        [JsonProperty("unicode_version")]
        public double UnicodeVersion { get; set; }

        /// <summary>
        /// Key for category property in categories.json.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("order")]
        public int Order { get; set; }

        /// <summary>
        /// Determines whether an emoji should be shown on a keyboard.
        /// </summary>
        [JsonProperty("display")]
        public int Display { get; set; }

        /// <summary>
        /// Colon-encapsulated, snake_case representation of the emoji name.
        /// </summary>
        [JsonProperty("shortname")]
        public string Shortname { get; set; }

        /// <summary>
        /// Alternative (including previously-used) shortnames.
        /// </summary>
        [JsonProperty("shortname_alternates")]
        public string[] ShortnameAlternates { get; set; }

        /// <summary>
        /// Ascii representation(s) of the emoji.
        /// </summary>
        [JsonProperty("ascii")]
        public string[] Ascii { get; set; }

        /// <summary>
        /// Is either <c>null</c> or the base code point of the corresponding Fitzpatrick Emoji Modifier.
        /// </summary>
        [JsonProperty("diversity")]
        public string Diversity { get; set; }

        /// <summary>
        /// Contains the base code points of the diversity children for a diversity parent (non-diverse, diversity base).
        /// </summary>
        [JsonProperty("diversities")]
        public string[] Diversities { get; set; }

        /// <summary>
        /// Is either <c>null</c> or the base code point of the corresponding male/female emoji symbol.
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Contains the base code points of the gender children for a gender parent (gender-neutral, gender base).
        /// </summary>
        [JsonProperty("genders")]
        public string[] Genders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("code_points")]
        public CodePoints CodePoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Shortname + " (" + CodePoints.Base + ")";
        }
    }

    public class CodePoints {
        /// <summary>
        /// 
        /// </summary>
        public CodePoints() {
            DefaultMatches = new string[0];
            GreedyMatches = new string[0];
        }

        /// <summary>
        /// Full unicode code point minus VS16 and ZWJ.
        /// </summary>
        [JsonProperty("base")]
        public string Base { get; set; }

        /// <summary>
        /// Represents code point according to http://unicode.org/Public/emoji/11.0/emoji-test.txt.
        /// </summary>
        [JsonProperty("fully_qualified")]
        public string FullyQualified { get; set; }

        /// <summary>
        /// Derived from same documentation as <see cref="FullyQualified"/>. NFQ code point convention is used for PNG file names in font file builds.
        /// </summary>
        [JsonProperty("non_fully_qualified")]
        public string NonFullyQualified { get; set; }

        /// <summary>
        /// The recommended code point to use for conversion to native unicode.
        /// </summary>
        [JsonProperty("output")]
        public string Output { get; set; }

        /// <summary>
        /// Contains one or more code points used to identify native unicode.
        /// </summary>
        [JsonProperty("default_matches")]
        public string[] DefaultMatches { get; set; }

        /// <summary>
        /// Contains one or more code points used to identify potential native unicode variants.
        /// </summary>
        /// <remarks>The greedy_matches code point(s) may replace non-emoji variants producing undesired results.<remarks
        [JsonProperty("greedy_matches")]
        public string[] GreedyMatches { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("decimal")]
        public string Decimal { get; set; }

        /// <summary>
        /// Combination of <see cref="Base"/> and <see cref="DefaultMatches"/> codepoints used to identify native unicode.
        /// </summary>
        public string[] AlternateMatches {
            get {
                var codepoints = new List<string>(DefaultMatches);
                if (codepoints.Contains(Base)) {
                    codepoints.Remove(Base);
                }
                return codepoints.ToArray();
            }
        }

        /// <summary>
        /// Combination of <see cref="Base"/> and <see cref="DefaultMatches"/> codepoints used to identify native unicode.
        /// </summary>
        public string[] BaseAndDefaultMatches {
            get {
                var codepoints = new List<string>(DefaultMatches);
                if (!codepoints.Contains(Base)) {
                    codepoints.Insert(0, Base);
                }
                return codepoints.ToArray();
            }
        }

    }
}
