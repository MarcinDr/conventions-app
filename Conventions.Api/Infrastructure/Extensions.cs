using System.Text.Json;
using System.Text.Json.Serialization;

namespace Conventions.Api.Infrastructure;

public static class Extensions
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		Converters = { new JsonStringEnumConverter() },
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	public static string Serialize(this object value) => JsonSerializer.Serialize(value, JsonSerializerOptions);

	public static TValue? Deserialize<TValue>(this string json) => JsonSerializer.Deserialize<TValue>(json, JsonSerializerOptions);
}