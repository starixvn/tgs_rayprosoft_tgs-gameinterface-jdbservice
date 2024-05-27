using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Core.Models;
using Tgs.GameInterface.JDBService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Controllers;

[ApiController]
[Route("jdb")]
public class JDBController : ControllerBase
{
	/// <summary>
	/// Funds the in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe",
	///             "serialNo": "1662452479",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/in", Name = "FundIn")]
	public async ValueTask<FundResultViewModel> FundInAsync(
		[FromServices] IGetService<FundInQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new FundInQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.Agent,
			source.PlayerCode,
			source.SerialNo,
			source.Amount);

		var result = await service
				 .GetAsync(
					 query: query,
					 cancellationToken: HttpContext.RequestAborted)
				 .ConfigureAwait(false);

		return new FundResultViewModel(
			result.SerialNo,
			result.Resp);
	}

	/// <summary>
	/// Funds the out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe",
	///             "serialNo": "1662452442",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/out", Name = "FundOut")]
	public async ValueTask<FundResultViewModel> FundOutAsync(
		[FromServices] IGetService<FundOutQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new FundOutQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.Agent,
			source.PlayerCode,
			source.SerialNo,
			source.Amount);

		var result = await service
				 .GetAsync(
					 query: query,
					 cancellationToken: HttpContext.RequestAborted)
				 .ConfigureAwait(false);

		return new FundResultViewModel(
			result.SerialNo,
			result.Resp);
	}

	/// <summary>
	/// Funds the status asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "serialNo": "1662452442"
	///         }
	/// </remarks>
	[HttpPost("fund/Status", Name = "FundStatus")]
	public ValueTask<bool> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, bool> service,
		FundStatusViewModel source)
		=> service
			.GetAsync(
				query: new FundStatusQuery(
					source.ApiUrl,
					source.Domain,
					source.AesKey,
					source.AesIV,
					source.Agent,
					source.SerialNo),
				cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "playerCode": "4ucmqplxhfoe",
	///             "gameType": "7",
	///             "machineType": "7001",
	///             "lang": "cn"
	///         }
	/// </remarks>
	[HttpPost("game/url", Name = "GameUrl")]
	public async ValueTask<GameUrlResultViewModel> GameUrlAsync(
		[FromServices] IGetService<GameUrlQuery, GameUrlResult> service,
		GameUrlViewModel source)
	{
		var query = new GameUrlQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.PlayerCode,
			source.GameType,
			source.MachineType,
			source.Lang);

		var result = await service
				 .GetAsync(
					 query: query,
					 cancellationToken: HttpContext.RequestAborted)
				 .ConfigureAwait(false);

		return new GameUrlResultViewModel(
			result.IsPlayerExist,
			result.GameUrl);
	}

	/// <summary>
	/// Games the wager asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "startTime": "2022-09-07T18:50:00+08:00",
	///             "endTime": "2022-09-07T18:55:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("game/wager", Name = "GameWager")]
	public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerAsync(
		[FromServices] IQueryService<GameWagerQuery, GameWagerResult> service,
		GameWagerViewModel source)
	{
		var query = new GameWagerQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.Agent,
			source.StartTime,
			source.EndTime);

		var results = await service
			.QueryAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (results.Length == 0)
		{
			yield break;
		}

		foreach (var result in results)
		{
			yield return new GameWagerResultViewModel(
				PlayerCode: result.PlayerCode,
				ExternalRoundId: result.ExternalRoundId,
				ExternalWagerId: result.ExternalWagerId,
				ExternalGameId: result.ExternalGameId,
				FreeGameState: result.FreeGameState,
				GambleState: result.GambleState,
				ExternalCreatedOn: result.ExternalCreatedOn,
				ExternalClosedOn: result.ExternalClosedOn,
				Bet: result.Bet,
				Settle: result.Settle,
				WinLoss: result.WinLoss,
				AdditionalData: result.AdditionalData);
		}
	}

	/// <summary>
	/// Games the wager old asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "startTime": "2022-09-07T18:50:00+08:00",
	///             "endTime": "2022-09-07T18:55:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("game/wager/old", Name = "GameWagerOld")]
	public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerOldAsync(
		[FromServices] IQueryService<GameWagerOldQuery, GameWagerResult> service,
		GameWagerViewModel source)
	{
		var query = new GameWagerOldQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.Agent,
			source.StartTime,
			source.EndTime);

		var results = await service
			.QueryAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false);

		if (results.Length == 0)
		{
			yield break;
		}

		foreach (var result in results)
		{
			yield return new GameWagerResultViewModel(
				PlayerCode: result.PlayerCode,
				ExternalRoundId: result.ExternalRoundId,
				ExternalWagerId: result.ExternalWagerId,
				ExternalGameId: result.ExternalGameId,
				FreeGameState: result.FreeGameState,
				GambleState: result.GambleState,
				ExternalCreatedOn: result.ExternalCreatedOn,
				ExternalClosedOn: result.ExternalClosedOn,
				Bet: result.Bet,
				Settle: result.Settle,
				WinLoss: result.WinLoss,
				AdditionalData: result.AdditionalData);
		}
	}

	/// <summary>
	/// Players the create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe"
	///         }
	/// </remarks>
	[HttpPost("player/create", Name = "PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<PlayerCreateCommand> service,
		PlayerCreateViewModel source)
		=> service
			.ExecuteAsync(
			   command: new PlayerCreateCommand(
					source.ApiUrl,
					source.Domain,
					source.AesKey,
					source.AesIV,
					source.Agent,
					source.PlayerCode),
			   cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Creates the player asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe"
	///         }
	/// </remarks>
	[HttpPost("player/balance", Name = "PlayerBalance")]
	public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
			[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
			PlayerBalanceViewModel source)
	{
		var query = new PlayerBalanceQuery(
			source.ApiUrl,
			source.Domain,
			source.AesKey,
			source.AesIV,
			source.Agent,
			source.PlayerCode);

		var result = await service
				 .GetAsync(
					 query: query,
					 cancellationToken: HttpContext.RequestAborted)
				 .ConfigureAwait(false);

		return new PlayerBalanceResultViewModel(
			result.IsPlayerExist,
			result.Balance);
	}

	/// <summary>
	/// Players the logout asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe"
	///         }
	/// </remarks>
	[HttpPost("player/logout", Name = "PlayerLogout")]
	public ValueTask PlayerLogoutAsync(
		[FromServices] ICommandService<PlayerLogoutCommand> service,
		PlayerKickViewModel source)
	=> service
		.ExecuteAsync(
			command: new PlayerLogoutCommand(
				source.ApiUrl,
				source.Domain,
				source.AesKey,
				source.AesIV,
				source.Agent,
				source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the online asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://api.jygrq.com",
	///             "domain": "ZF",
	///             "aesKey": "831057181dd07807",
	///             "aesIV": "193a3159afebee65",
	///             "agent": "zf055gpi",
	///             "playerCode": "4ucmqplxhfoe"
	///         }
	/// </remarks>
	[HttpPost("player/online", Name = "PlayerOnline")]
	public ValueTask<bool> PlayerOnlineAsync(
		[FromServices] IGetService<PlayerOnlineQuery, bool> service,
		PlayerOnlineViewModel source)
	=> service.GetAsync(
			query: new PlayerOnlineQuery(
				source.ApiUrl,
				source.Domain,
				source.AesKey,
				source.AesIV,
				source.Agent,
				source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted);
}
