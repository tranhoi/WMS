namespace WebUIFinal.Core
{
    public static class CommonHelpers
    {
        public static T ParseEnum<T>(string value) 
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            else
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
        }

        public static String EnumConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }
    }
}
