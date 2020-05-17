using GuideOne_Xamarin.Model;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GuideOne_Xamarin.Helpers
{
	static class Conf
	{
		public static User User { get; set; }
		public static RSACryptoServiceProvider CryptoProvider { get; set; }
	}
}
