using System;
using System.Reflection;

namespace MaidBeats.Models
{
    public static class MaidBeatsInfo
    {
        public static Lazy<string> Name => new Lazy<string>(() => GetAssemblyInfo<AssemblyTitleAttribute>().Title);

        public static Lazy<string> Description => new Lazy<string>(() => GetAssemblyInfo<AssemblyDescriptionAttribute>().Description);

        public static Lazy<string> Copyright => new Lazy<string>(() => GetAssemblyInfo<AssemblyCopyrightAttribute>().Copyright);

        public static Lazy<string> Version => new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version.ToString());

        private static T GetAssemblyInfo<T>() where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        }
    }
}