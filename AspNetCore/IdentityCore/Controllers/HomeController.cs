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
	/***********************************************************************************************/

	// # 로깅
	// - 버그를 찾을 때

	// ILogger : 로깅을 하는 주체
	// ILoggerProvider : 어디에 로깅을 할지
	//  - ConsoleLoggerProvider, FileLoggerProvider

	// DI를 이용해 ILogger 주입

	// 로깅과 관련된 요소들
	// - LogLevel : 중요도
	// - Event Category : ILogger<T> 의 T
	// - Message : 로깅 메세지 (Hello Log!)
	//   -- Parameters : 로깅 메세지를 Placeholder로 남길 경우 필요  
	// - Exception : 예외가 일어났을 때 Exception 객체를 같이 넘길 수 있음
	// - EventId : 기본값 0, 일종의 이벤트 그룹 (동일한 유형을 구분하게 부여)

	// LogLevel
	//  - Critical (메모리 부족, 디스크 용량 부족)
	//  - Error (DB 에러, 널 크래시)
	//  - Warning (어떤 코드가 예측한 것과 다르게 동작할때)
	//  - Information (유저가 접속할때 등)
	//  - Debug (개발단계에서 디버깅 용도)

	// LogCategory
	// 기본적으로 Controller 이름
	// DI해서 이름 바꿀 수도 있음

	// Message
	//  - 메시지 내용
	//  - Placeholder를 사용하는게 좋은가?
	//  - 최종 문자열 형태의 Message 결과문은 같음
	//  - Placeholder 사용하면 인자들에 대한 Key/Value 짝도 관리함
	//  - 경우에 따라 그냥 단순한 문자열 결과물만 만드는게 아니라 추가 검색을 위해 인자들을 따로 로그를 찍는 경우도 있음

	// Logging Provider
	//  - 콘솔 로깅은 개발 단계에서만 유용
	// 로그를 파일로 남기고 싶다면 -> NetEscapades

	// LogVerbosity
	// 로깅이 편리하지만 너무 많이지면 보기가 힘들다
	// 원하는 수준만 로그가 찍히도록 규칙을 정한다

	// Structure Logging
	// 순수 문자열 로깅은 문제가 터졌을 때 상세 정보를 알기 힘듬
	// Seq, Elasticsearch

	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger _logger;


		private IAuthorizationService _auth;

		public HomeController(ILoggerFactory factory, IAuthorizationService auth)
		{
			_logger = factory.CreateLogger("TestCategory");
			_auth = auth;
		}

		[AllowAnonymous]
		public IActionResult Index()
		{
			_logger.LogInformation("Hello Log!");

			string name = "pcy";

			_logger.LogInformation($"Hello {name}");
			_logger.LogInformation("Hello {0}", name);

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
