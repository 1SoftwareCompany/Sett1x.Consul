namespace One.Settix
{
    public static class SettixKeyExtentions
    {
        public static string ToConsulKey(this string rawKey)
        {
            var key = Key.Parse(rawKey);
            return $"{ConsulForSettix.RootFolder}/{key.ApplicationName}/{key.Cluster}/{key.Machine}/{key.SettingKey}";
        }

        public static string ToApplicationKeyPrefix(this ISettixContext context)
        {
            return $"{ConsulForSettix.RootFolder}/{context.ApplicationName}".ToLower();
        }

        public static Key FromConsulKey(this string consulKey)
        {
            string[] parts = consulKey.Split('/');
            if (parts.Length != 5)
            {
                return null;
            }
            return new Key(parts[1], parts[2], parts[3], parts[4]);
        }
    }
}
