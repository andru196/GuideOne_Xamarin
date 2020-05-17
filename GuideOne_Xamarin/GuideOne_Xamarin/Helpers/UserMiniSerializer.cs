using GuideOne_Xamarin.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuideOne_Xamarin.Helpers
{
	class UserMiniSerializer : JsonConverter<User>
	{
		public override User Read(ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			return null;
		}

		public override void Write(
			Utf8JsonWriter writer, User user,
			JsonSerializerOptions options)
		{
			writer.WriteStartObject("User");
			writer.WriteNumber("Id", user.Id);
			writer.WriteNumber("AuthId", user.AuthId);
			writer.WriteEndObject();
		}
	}
}
