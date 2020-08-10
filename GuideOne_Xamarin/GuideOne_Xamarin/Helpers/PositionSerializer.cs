using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xamarin.Forms.Maps;
using System.Text.Json.Serialization;

namespace GuideOne_Xamarin.Helpers
{
	class PositionSerializer : JsonConverter<Position>
	{
		public override void Write(
			Utf8JsonWriter writer, Position point,
			JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("Latitude", point.Latitude);
			writer.WriteNumber("Longitude", point.Longitude); ;
			writer.WriteEndObject();
		}
		public override Position Read(ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			double lat = 0, lon = 0;
			int i = 0;
			while (reader.Read() && i < 2)
			{
				i++;
				var name = reader.GetString().ToLower();
				reader.Read();
				switch (name)
				{
					case "latitude":
						lat = reader.GetDouble();
						break;
					case "longitude":
						lon = reader.GetDouble();
						break;
				}
			}
			if (lat == 0 || lon == 0)
				return new Position(0, 0);
			return new Position(lat, lon);
		}
	}
}
