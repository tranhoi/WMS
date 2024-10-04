namespace WebUIFinal.Core
{
    public static class CommonHelpers
    {
        public static T ParseEnum<T>(string value) 
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static String ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

    }
}
