using System;
using System.Collections.Generic;
using System.Text;
using GuideOne_Xamarin.Helpers;
using Newtonsoft.Json;
using SQLite;

namespace GuideOne_Xamarin.Model
{
	[Table("Users")]
	public class User : Entity
	{
		[PrimaryKey]
		[JsonProperty("userId")]
		public override int Id { get; set; }
		[JsonProperty("authId")]
		public int AuthId { get; set; }
		public string Phone { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string SecondName { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

		public double? HourPrice { get; set; }
		[JsonProperty("access_token")]
		public string Token { get; set; }
		[JsonIgnore]
		public string PrivateKey { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
