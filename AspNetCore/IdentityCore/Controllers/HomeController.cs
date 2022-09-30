using IdentityCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityCore.Controllers
{
	// # Authorization (권한)
	// 인증 통과 해도 모든 권한을 부여하지 않음

	// 1) Request
	// 2) Routing
	// 3) Authentication 미들웨어
	// 4) MVC 미들웨어
	//  - Authorize 필터
	//  - Action
	//  - View

	// 필터 특성상 Global, Controller, Action 등 적용 범위 세부 설정 가능
	// [Authorize] [AllowAnonymous]

	// 권한이 없다면
	// 다음과 같은 IActionResult 생성
	//  - ChallengeResult (로그인하지 않음)
	//  - ForbidResult (로그인은 했는데 그냥 권한이 없음)
	// WebAPI 경우 쿠키가 아니라 토큰을 이요. 개념적으로는 비슷.
	//  - MVC는 ChallengeResult, ForbidResult에 따라 특정 View를 보여준다
	//  - WebAPI는 401, 403번 Status Code를 반환

	// Policy
	//  - Request가 Authorize(권한을 부여받기)되기 위해 필요한 정책
	// [Authorize("AdminPolicy")]
	// AdminPolicy 정책 만드는법 -> ConfigureServices에서 만듦

	// Custom Policy
	// 굉장히 복잡한 규칙이 적용된다면?

	// Custom Policy = Requirement(AND) + Handler(OR)
	// Policy는 하나 이상의 Requirement로 구성
	// Requirement는 하나 이상의 Handler로 구성

	// [PolicyName]
	// - [CanEnterRequirement]
	//   - 첫번째 Handler : 나이 확인
	//   - 두번째 Handler : Vip 여부
	// - [IsNot BlackListRequirement]
	//  - Handler : Banned = false

	// Authorization 수동처리
	// Action 내부에서 조건에 따라 유동적으로 처리하고 싶다면?
	// IAuthorizationService

	public class CanEnterRequirment : IAuthorizationRequirement
	{
		public int MinAge { get; }
		public int MaxAge { get; }

		public CanEnterRequirment(int minAge, int maxAge)
		{
			MinAge = minAge;
			MaxAge = maxAge;
		}
	}

	public class AgeHandler : AuthorizationHandler<CanEnterRequirment>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEnterRequirment requirement)
		{
			Claim claim =  context.User.Claims.FirstOrDefault(c => c.Type == "Age");
			if (claim != null)
			{
				int age = int.Parse(claim.Value);
				if (requirement.MinAge <= age && age <= requirement.MaxAge)
				{
					context.Succeed(requirement);
				}
			}

			// Requirement가 만족되지 않았으면 아무것도 안 함.
			return Task.CompletedTask;
		}
	}

	public class IsVipHandler : AuthorizationHandler<CanEnterRequirment>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEnterRequirment requirement)
		{
			Claim claim = context.User.Claims.FirstOrDefault(c => c.Type == "IsVip");
			if (claim != null)
			{
				bool vip = bool.Parse(claim.Value);
				if (vip)
				{
					context.Succeed(requirement);
				}
			}

			// Requirement가 만족되지 않았으면 아무것도 안 함.
			return Task.CompletedTask;
		}
	}

	public class IsNotBlackListRequirement : IAuthorizationRequirement
	{

	}

	public class IsUnbannedHandler : AuthorizationHandler<IsNotBlackListRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsNotBlackListRequirement requirement)
		{
			Claim claim = context.User.Claims.FirstOrDefault(c => c.Type == "IsBanned");
			if (claim != null)
			{
				context.Fail();
				//bool banned = bool.Parse(claim.Value);
				//if (banned)
				//{
				//	context.Fail();
				//}
				//else
				//{
				//	context.Succeed(requirement);
				//}
			}

			// Requirement가 만족되지 않았으면 아무것도 안 함.
			return Task.CompletedTask;
		}
	}

	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private IAuthorizationService _auth;

		public HomeController(ILogger<HomeController> logger, IAuthorizationService auth)
		{
			_logger = logger;
			_auth = auth;
		}

		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		//[Authorize("EnterPolicy")]
		public async Task<IActionResult> Privacy()
		{
			var result =  await _auth.AuthorizeAsync(User, "EnterPolicy");
			if (!result.Succeeded)
			{
				return new ForbidResult();
			}

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
