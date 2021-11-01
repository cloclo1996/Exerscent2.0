using UnityEngine;
using System.Collections;

public class md5Sum:MonoBehaviour{



	// NOTE: THIS SCRIPT IS NOT USED ANYMORE - DELETE IF YOU WANT TO
	// LOOK INTO THIS PAGE IF YOU WANT TO ENCRYPT THE URL'S: http://wiki.unity3d.com/index.php?title=Server_Side_Highscores

public string md5Summary(string strToEncrypt)
{
	System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
	byte[] bytes = ue.GetBytes(strToEncrypt);

	// encrypt bytes
	System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
	byte[] hashBytes = md5.ComputeHash(bytes);

	// Convert the encrypted bytes back to a string (base 16)
	string hashString = "";

	for (int i = 0; i < hashBytes.Length; i++)
	{
		hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
	}

	return hashString.PadLeft(32, '0');
	}
}