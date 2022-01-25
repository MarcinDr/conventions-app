using System.Text.Json.Serialization;
using Refit;

namespace Conventions.Api.Services;

public interface IBreweriesApi
{
	[Get("/breweries?per_page=10")]
	Task<IList<Brewery>> GetAll();
}

#pragma warning disable 8618
public class Brewery
{
	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("street")]
	public string Street { get; set; }

	[JsonPropertyName("city")]
	public string City { get; set; }

	[JsonPropertyName("postal_code")]
	public string PostalCode { get; set; }

	[JsonPropertyName("country")]
	public string Country { get; set; }

	[JsonPropertyName("phone")]
	public string Phone { get; set; }

	[JsonPropertyName("website_url")]
	public string? WebsiteUrl { get; set; }

	[JsonPropertyName("updated_at")]
	public DateTime UpdatedAt { get; set; }

	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
}