namespace StdUtils
{
    public static class StringUtils
    {
        public static string Enquote(string input)
        {
            return string.Format("{0}{1}{0}", '"', input);
        }
    }
}
