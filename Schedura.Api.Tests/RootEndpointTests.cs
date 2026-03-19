using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Schedura.Api.Tests;

public class RootEndpointTests : IClassFixture<WebApplicationFactory<Program>> {
	private readonly WebApplicationFactory<Program> factory;

	public RootEndpointTests(WebApplicationFactory<Program> factory) {
		this.factory = factory.WithWebHostBuilder(builder => {
			builder.ConfigureAppConfiguration((_, configurationBuilder) => {
				configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
					["ConnectionStrings:DefaultConnection"] = "Server=localhost,1433;Database=ScheduraTests;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
				});
			});
		});
	}

	[Fact]
	public async Task GetRoot_ReturnsOkMessage() {
		var client = factory.CreateClient();

		var response = await client.GetAsync("/");
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode);
		Assert.Contains("Schedura API no ar", content, StringComparison.Ordinal);
	}
}
