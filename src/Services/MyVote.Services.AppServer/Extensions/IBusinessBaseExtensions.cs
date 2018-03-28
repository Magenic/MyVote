using Csla;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MyVote.Services.AppServer.Extensions
{
	internal static class IBusinessBaseExtensions
	{
		internal static async Task<IActionResult> PersistAsync(this IBusinessBase @this)
		{
			return await @this.PersistAsync(() => { return new NoContentResult(); });
		}

		internal static async Task<IActionResult> PersistAsync(this IBusinessBase @this,
			Func<IActionResult> resultCreator)
		{
			if (!@this.IsSavable)
			{
				return new BadRequestObjectResult(
					new
					{
						IsDirty = @this.IsDirty,
						BrokenRules = @this.GetBrokenRules().ToString()
					});
			}
			else
			{
				await @this.SaveAsync();
				return resultCreator();
			}
		}

		internal static async Task<IActionResult> PersistAsync<T>(this IBusinessBase @this,
			 Func<T, IActionResult> resultCreator)
			 where T : class, IBusinessBase
		{
			if (!@this.IsSavable)
			{
				return new BadRequestObjectResult(
					 new
					 {
						 IsDirty = @this.IsDirty,
						 BrokenRules = @this.GetBrokenRules().ToString()
					 });
			}
			else
			{
				var result = await @this.SaveAsync() as T;
				return resultCreator(result);
			}
		}
	}
}