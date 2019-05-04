namespace DependencyInjectionWorkshop.Models
{
    using System.Text;

    public interface IHash
    {
        string GetHash(string plainText);
    }

    public class Hash : IHash
    {
        public string GetHash(string plainText)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}