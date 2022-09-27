using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Controllers
{
	// # MVC (Model-View-Controller)
	// Model (메모리, 파일, DB등에서 정보를 추출) 재료
	// Controller (데이터가공, 필터링, 유효성 체크, 서비스 호출) 재료 손질
	//  + 각종 서비스(DI 서비스) -> 요리
	// View (최종 결과물을 어떻게 보여줄지) 최종 서빙

	// 이렇게 역할 분담을 할 때의 장점?
	// 1. 유동적으로 기능 변경 가능
	// ex) SPA(Json) Web(HTML) 결과물이 다르면 View Layer만 바꾸고, Controller 재사용 가능

	// Action은 요청에 대한 실제 처리 함수 (Handler)
	// Controller는 Action을 포함하고 있는 그룹

	// Controller 상속이 무조건 필요한 것은 아님
	// View()처럼 이미 정의된 Helper 기능들을 사용하고 싶으면 필요
	// UI (View)와 관련된 기능들을 뺀 ControllerBase -> WebApi

	// MVC에서 Controller 각종 데이터 가공을 담당, UI랑 무관
	// 넘길 때 IActionResult를 넘김
	// 자주 사용되는 IActionResult 종류
	// 1) ViewResult : HTML View 생성
	// 2) RedirectionResult : 요청을 다른 곳으로 토스 (다른 페이지로 연결해 줄 때)
	// 3) FileResult : 파일을 반환
	// 4) ContentResult : 특정 string을 반환
	// 5) StatusCodeResult : HTTP status code 반환
	// 6) NotFoundResult : 404 HTTP status code 반환

	// 결론 : MVC를 사용하면 역할 분담이 확실해 진다.
	// 서로 종속된 코드를 만들면, 하나를 수정할 때 연관된 다른 쪽에서 문제가 터지는 일이 빈번하지만
	// MVC는 역할 분담으로 인해 코드 종속성을 제거함
	// MVC에서 V를 빼고 MC만 사용하면 결국 WebApi가 된다.
	/***********************************************************************************************/

	// # M (Model)
	// 데이터 모델
	// 데이터 종류가 다양함
	//  - Binding Model
	//    클라이언트에서 보낸 Request를 파싱하기 위한 데이터 모델 << 유효성 검증 필수
	//  - Application Model
	//    서버의 각종 서비스들이 사용하는 데이터 모델 (ex. RankingService 라면 RankingData)
	//  - View Model
	//    Response UI를 만들기 위한 데이터 모델
	//  - API Model
	//    WebAPI Controller에서 JSON / XML 포멧으로 응답할 때 필요한 데이터 모델

	// 일반적인 MVC 순서
	// 1) HTTP Request가 옴
	// 2) Routing에 의해 Controller / Action 정해짐
	// 3) Model Binding으로 Request에 있는 데이터를 파싱 (Validation)
	// 4) 담당 서비스로 전달 (Application Model)
	// 5) 담당 서비스가 결과물을 Action에 돌려 주면
	// 6) Action에서 ViewModel을 이용해서 View로 전달
	// 7) View에서 HTML 생성
	// 8) Response로 HTML 결과물을 전송

	// Model Binding
	// 1) Form Values
	//    Request의 Body에서 보낸 값 (HTTP POST 방식의 요청이 왔을 때 사용)
	// 2) Routing Values
	//    URL 매칭, Default Value
	// 3) Query String Values
	//    URL 끝에 붙이는 방법. 

	// Complex Types
	// 넘겨 받을 인자가 너무 많아지면 부담스러워짐 -> 별도의 모델링 클래스 생성

	// Collection
	// List나 Dictionary로도 매핑이 가능

	// Binding Source 지정
	// 기본적으로 Binding Model은 Form, Route, QureyString 사용
	// 위의 삼총사 중 하나를 명시적으로 지정해서 파싱하거나, 다른애로 지정할 수도 있음
	// ex) 대표적으로 Body에서 JSON 형태로 데이터를 보내주고 싶을 때

	// [FromHeader] : HeaderValue에서 찾아라
	// [FromQuery] : QueryString 에서 찾아라
	// [FromRoute] : Route Parameter에서 찾아라
	// [FromForm] : POST Body에서 찾아라
	// [FromBody] : 그냥 Body에서 찾아라 (디폴트 JSON -> 다른 형태로도 세팅 가능)

	// DataAnnotation
	// 공용으로 사용되는 모델 -> 기본 검증 모델을 하나만 만들고 돌려서 쓸 수 있음
	// 세부적인 검사는 하기 힘들다
	// [Required] : 무조건 있어야함
	// [CreditCard] : 올바른 결제카드 번호인지
	// [EmailAddress] : 올바른 이메일 주소인지
	// [StringLength(max)] : String 길이가 최대 max인지
	// [MinLength(min)] : Collection의 크기가 최소 min인지
	// [Phone] : 올바른 전화번호인지
	// [Range(min,max)] : Min-Max 사이의 값인지
	// [Url] : 올바른 URL인지
	// [Compare] : 2개의 Property 비교 ex) Password, ConfirmPassword

	// [!] Validation은 Attribute만 적용하면 알아서 자동으로 적용되지만
	// 결과에 대해서 어떻게 처리할지는 Action에서 정해야 함.
	//  -> ControllerBase의 ModelState에 결과를 저장.
	/***********************************************************************************************/

	// #V (View)

	// Razor View Page (.cshtml)
	// cshtml -> HTML과 유사함
	// HTML은 동적 처리가 애매함 ex) if else 같은 분기문처리나, 특정 리스트 개수에 따라서 html태그 갯수가 변경되어야 하는 경우
	// 따라서 c#으로 동적인 부분 처리

	// HTML : 고정 부분 담당 (실제로 클라에 응답을 줄 HTML)
	// c# : 동적으로 변화하는 부분 담당
	// Razor Template을 만들어 주고 이를 Razor Template Engine이 Tamplate을 분석해서 최종 결과물을 동적 생성

	// 일반적으로 View에 데이터를 건내주는 역할은 Action
	//  1) ViewModel
	//     - 클래스로 만들어서 넘겨주는 방식
	//  2) ViewData
	//     - Dictionary<string, object> key/value를 넘기는 방식
	//  3) ViewBag
	//     - ViewData와 유사하나 Dictionary가 아니라 dynamic문법 사용

	// Layout
	// 보통 웹사이트에서 공동적으로 등장하는 UI (ex. Header, Footer), CSS, Javascript
	// 공통적인 부분만 따로 빼서 관리하는 것 -> Layout
	// Layout도 그냥 Razor Template과 크게 다르지 않지만 무조건 @Renderbody()를 포함해야 한다
	// 그런데 1개의 위치가 아니라 여기저기 Child를 뿌려주고 싶다면? -> RenderSection을 이용해 다른 이름으로 넣어준다.

	// _ViewStart, _ViewImports -> 공통적인 부분을 넣어주는 곳
	// 모든 View마다 어떤 Layout을 적용할지 일일히 넣지 않고 한번에 넣음
	// 모든 View마다 공통적으로 들어가는 부분 (ex. using ~~~)
	// _ViewStart, _ViewImports를 만들어 주면 해당 폴더 안에 있는 모든 View에 일괄 적용

	//PartialView라고 반복적으로 등장하는 View -> 재사용할 수 있게 만들 수 있는데, 이름 앞에 _를 붙여야함.
	// [!] _를 붙이면 _ViewStart가 적용되지 않는다.

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		// 기본적으로 Views\Controller\Action.cshtml을 템플릿으로 사용
		public IActionResult Test()
		{
			//상대경로
			//return View("Privacy");

			//절대경로 -> Views와 .cshtml을 다 추가해줘야함
			//return View("Views/Shared/Error.cshtml");

			TestViewModel testViewModel = new TestViewModel()
			{
				Names = new List<string>()
				{
					 "Faker", "Deft", "Dopa"
				}
			};

			return View(testViewModel); // View -> new ViewResult
		}

		//public IActionResult Test2(TestModel testModel)
		//{
		//	if (!ModelState.IsValid)
		//		return RedirectToAction("Error");

		//	return null;
		//}

		// 1) names[0]=Faker&names[1]=Deft
		// 2) [0]=Faker&[1]Deft
		// 3) names=Faker&names=Deft
		//public IActionResult Test3(List<string> names)
		//{
		//	return null;
		//}

		public IActionResult Index()
		{
			//string url = Url.Action("Privacy", "Home");
			//string url = Url.RouteUrl("test", new { test = 123 });

			//return Redirect(url);

			return RedirectToAction("Privacy");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
