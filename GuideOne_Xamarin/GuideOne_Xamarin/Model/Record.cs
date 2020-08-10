using GuideOne_Xamarin.Helpers;
using System.Text.Json.Serialization;
using System;
using Xamarin.Forms.Maps;
using SQLite;

namespace GuideOne_Xamarin.Model
{
	public class Record : Entity
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
		public string Description { get; set; }

		[JsonPropertyName("Audio")]
		[JsonConverter(typeof(ContentConverter))]
		public byte[] Audio { get; set; }
		//public int Duaration { get; set; }

		[JsonPropertyName("DateTime")]
		public override DateTime dt { get; set; }
		public int UserId { get; set; }
		//public bool IsPublic { get; set; }
		//public bool IsPaid { get; set; }
		//public bool IsAnon { get; set; }
		//[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		//public DateTime? ValidatyTime { get; set; }
		//[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		//public uint? Area { get; set; }
		[JsonConverter(typeof(PositionSerializer))]
		[Ignore]
		public Position Point { get; set; }
		[JsonConverter(typeof(ContentConverter))]
		public byte[] Photo { get; set; }
		//[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		//public string UserWhoCanSee { get; set; }
		//public string Language { get; set; }

		[JsonIgnore]
		public double Latitude
		{
			get
			{
				return Point.Latitude;
			}
			set
			{
				Point = new Position(value, Longitude);
			}
		}
		[JsonIgnore]
		public double Longitude
		{
			get
			{
				return Point.Longitude;
			}
			set
			{
				Point = new Position(Latitude, value);
			}
		}

		[JsonIgnore]
		//[Ignore]
		public bool IsDownloaded { get; set; }
	}
}
