using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace Password_Gen_and_Storage
{
	class Crypt
	{
		static readonly byte[] Key = Encoding.ASCII.GetBytes("RfUjXn2r4u7x!A%D");
		static readonly byte[] IV = Encoding.ASCII.GetBytes("y$B&E)H@McQfTjWn");
		public static byte[] EncryptString(string plainText)
		{
			
			byte[] encrypted;
			using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;
				aesAlg.Padding = PaddingMode.PKCS7;
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}
			return encrypted;
		}
		public static string DecryptString(byte[] cipherText)
		{
			string plaintext = null;

			using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;
				aesAlg.Padding = PaddingMode.PKCS7;

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}
			return plaintext;
		}
	}
}
