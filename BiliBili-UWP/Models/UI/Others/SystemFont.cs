using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.UI.Others
{
    public class SystemFont
    {
        public string Name { get; set; }

        public Windows.UI.Xaml.Media.FontFamily FontFamily { get; set; }

        public int FamilyIndex { get; set; }

        public int Index { get; set; }

        public static List<SystemFont> GetFonts()
        {
            var fontList = new List<SystemFont>();

            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var familyCount = fontCollection.FontFamilyCount;

            for (int i = 0; i < familyCount; i++)
            {
                var fontFamily = fontCollection.GetFontFamily(i);
                var familyNames = fontFamily.FamilyNames;
                int index;

                if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index))
                {

                    if (!familyNames.FindLocaleName("en-us", out index))
                    {
                        index = 0;
                    }
                }
                try
                {
                    using (var font = fontFamily.GetFont(index))
                    {
                        if (font.IsSymbolFont)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                string name = familyNames.GetString(index);
                fontList.Add(new SystemFont()
                {
                    Name = name,
                    FamilyIndex = i,
                    Index = index,
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily(name)
                });
            }

            return fontList;
        }

        public override bool Equals(object obj)
        {
            return obj is SystemFont font &&
                   Name == font.Name;
        }

        public List<Character> GetCharacters()
        {
            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var fontFamily = fontCollection.GetFontFamily(FamilyIndex);

            var font = fontFamily.GetFont(Index);

            var characters = new List<Character>();
            var count = 65535;
            for (var i = 0; i < count; i++)
            {
                if (font.HasCharacter(i))
                {
                    characters.Add(new Character()
                    {
                        Char = char.ConvertFromUtf32(i),
                        UnicodeIndex = i
                    });
                }
            }

            return characters;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

    }
    public class Character
    {
        public string Char { get; set; }

        public int UnicodeIndex { get; set; }
    }
}
