namespace DAL
{
    public class Md5Algorithms
  {
    public static string CreateMD5(string input)
    {
      using (var provider = System.Security.Cryptography.MD5.Create())
      {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        foreach (byte b in provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input)))
          builder.Append(b.ToString("x2").ToLower());

        return builder.ToString();
      }
    }
  }
}