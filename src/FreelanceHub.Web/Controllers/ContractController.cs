using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize]
	public class ContractController : Controller
	{
		private readonly IContractService _contractService;

		public ContractController(IContractService contractService)
		{
			_contractService = contractService;
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var contract = await _contractService.GetDetailsAsync(id, userId);
			return contract is null ? NotFound() : View(new ContractDetailsViewModel { Contract = contract });
		}

		// [HttpPost]
		// [ValidateAntiForgeryToken]
		// [Authorize(Roles = "Freelancer")]
		// public async Task<IActionResult> Complete(int id)
		// {
		// 	if (!TryGetCurrentUserId(out var userId)) return Challenge();

		// 	var result = await _contractService.CompleteAsync(id, userId);
		// 	if (result.NotFound) return NotFound();
		// 	if (!result.Succeeded)
		// 	{
		// 		TempData["ContractError"] = string.Join(" ", result.Errors.Select(error => error.Message));
		// 	}
		// 	else TempData["ContractSuccess"] = "The contract has been marked complete.";
		// 	return RedirectToAction(nameof(Details), new { id });
		// }

		// [HttpPost]
		// [ValidateAntiForgeryToken]
		// [Authorize(Roles = "Freelancer,Client")]
		// public async Task<IActionResult> Terminate(int id)
		// {
		// 	if (!TryGetCurrentUserId(out var userId)) return Challenge();

		// 	var result = await _contractService.TerminateAsync(id, userId);
		// 	if (result.NotFound) return NotFound();
		// 	if (!result.Succeeded)
		// 	{
		// 		TempData["ContractError"] = string.Join(" ", result.Errors.Select(error => error.Message));
		// 	}
		// 	else TempData["ContractSuccess"] = "The contract has been terminated.";
		// 	return RedirectToAction(nameof(Details), new { id });
		// }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SubmitReview(
			int id,
			[Bind(Prefix = nameof(ContractDetailsViewModel.Review))] SubmitReviewViewModel model)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (!ModelState.IsValid)
			{
				return await ReviewViewAsync(id, userId, model);
			}

			var result = await _contractService.SubmitReviewAsync(id, userId, new SubmitReviewRequest
			{
				Rating = model.Rating!.Value,
				Comment = model.Comment
			});

			if (result.NotFound)
			{
				return NotFound();
			}

			if (!result.Succeeded)
			{
				AddErrors(result);
				return await ReviewViewAsync(id, userId, model);
			}

			TempData["ReviewSuccess"] = "Your review was submitted.";
			return RedirectToAction(nameof(Details), new { id });
		}

		private async Task<IActionResult> ReviewViewAsync(int contractId, int userId, SubmitReviewViewModel review)
		{
			var contract = await _contractService.GetDetailsAsync(contractId, userId);
			return contract is null
				? NotFound()
				: View(nameof(Details), new ContractDetailsViewModel { Contract = contract, Review = review });
		}

		private void AddErrors(UpdateOperationResult result)
		{
			foreach (var error in result.Errors)
			{
				var key = string.IsNullOrWhiteSpace(error.FieldName)
					? string.Empty
					: $"{nameof(ContractDetailsViewModel.Review)}.{error.FieldName}";
				ModelState.AddModelError(key, error.Message);
			}
		}

		private bool TryGetCurrentUserId(out int userId)
		{
			return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
		}

		public async Task<IActionResult> Index()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var contracts = await _contractService.GetContractsForUserAsync(userId);
			return View(contracts);
		}
	}
}
