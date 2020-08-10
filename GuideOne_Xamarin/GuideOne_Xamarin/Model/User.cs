using System;
using System.Collections.Generic;
using System.Text;
using GuideOne_Xamarin.Helpers;
using System.Text.Json.Serialization;
using SQLite;

namespace GuideOne_Xamarin.Model
{
	[Table("Users")]
	public class User : Entity
	{
		[PrimaryKey]
		[JsonPropertyName("userId")]
		public override int Id { get; set; }
		[JsonPropertyName("authId")]
		public int AuthId { get; set; }
		public string Phone { get; set; }
		public string Name { get; set; }
		public string SecondName { get; set; }

		public double? HourPrice { get; set; }
		[JsonPropertyName("access_token")]
		public string Token { get; set; }
		[JsonIgnore]
		public string PrivateKey { get; set; }
		public uint? CompanyId { get; set; }
		[JsonIgnore]
		public bool IsConfirmed { get; set; }
		[JsonIgnore]
		public bool IsUser { get; set; }
		[NotNull]
		public DateTime dt { get; set; }

		public User()
		{
			dt = DateTime.Now;
		}

	}
}
