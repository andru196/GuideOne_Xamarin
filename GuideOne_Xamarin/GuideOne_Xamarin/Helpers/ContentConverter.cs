using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuideOne_Xamarin.Helpers
{
	public class ContentConverter : JsonConverter <byte[]>
	{
		public override byte[] Read(ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			return reader.GetBytesFromBase64();
		}

		public override void Write(
			Utf8JsonWriter writer, byte[] content,
			JsonSerializerOptions options)
		{
			writer.WriteBase64StringValue(content);
		}
	}
}
