using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeClient = Substitute.For<IGameApiClient>();

		var query = new FundStatusQuery(
			ApiUrl: new Uri("http://localhost/"),
			Domain: "ZF",
			AesKey: "831057181dd07807",
			AesIV: "193a3159afebee65",
			Agent: "zf055gpi",
			SerialNo: Guid.NewGuid().ToString("N"));

		_ = fakeClient
			.CheckCashTransferAsync(
			Arg.Is(query.ApiUrl),
			Arg.Is(query.Domain),
			Arg.Is(query.AesKey),
			Arg.Is(query.AesIV),
			Arg.Is(query.Agent),
			Arg.Is(query.SerialNo),
			Arg.Any<CancellationToken>()
			).Returns(true);

		var service = new FundStatusGetService(
			fakeClient);

		var actual = await service.GetAsync(query: query);

		Assert.True(actual);
	}
}
