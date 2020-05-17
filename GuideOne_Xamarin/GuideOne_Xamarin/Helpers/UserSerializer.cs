using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GuideOne_Xamarin.Helpers
{
	class UserSerializer : UserMiniSerializer
	{
		public override void Write(
			Utf8JsonWriter writer, Model.User user,
			JsonSerializerOptions options)
		{
			writer.WriteStartObject("User");
			writer.WriteNumber("Id", user.Id);
			writer.WriteNumber("AuthId", user.AuthId);
			if (user.Name != null)
				writer.WriteString("Name", user.Name);
			if (user.SecondName != null)
				writer.WriteString("SecondName", user.SecondName);
			if (user.HourPrice.HasValue)
				writer.WriteNumber("HourPrice", user.HourPrice.Value);
			if (user.CompanyId.HasValue)
				writer.WriteNumber("CompanyId", user.CompanyId.Value);
			writer.WriteEndObject();
		}
	}
}
