using System.Security.Cryptography;
using System.Text;

namespace HanClothesShopProject.CommonUtil
{
    public class PasswordHelper
    {
        //生成随机的盐值，避免被破解
        public static byte[] GenerateSalt()
        {
            return Encoding.UTF8.GetBytes("LeoYi*a.123!-=_@~"); // 固定的盐值
        }
        //加密获得MD5加密字符串
        public static string HashPasswordWithMD5(string password, byte[] salt)
        {
            using (var md5 = MD5.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var passwordWithSaltBytes = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSaltBytes, passwordBytes.Length, salt.Length);

                var hashBytes = md5.ComputeHash(passwordWithSaltBytes);

                var hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];
                Buffer.BlockCopy(hashBytes, 0, hashWithSaltBytes, 0, hashBytes.Length);
                Buffer.BlockCopy(salt, 0, hashWithSaltBytes, hashBytes.Length, salt.Length);

                return Convert.ToBase64String(hashWithSaltBytes);
            }
        }
    }
}
