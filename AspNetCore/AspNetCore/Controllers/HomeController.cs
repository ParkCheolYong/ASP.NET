using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
	/***********************************************************************************************/

	// # Tag Helper (일종의 HTML Helper)
	// 웹페이지에서 거꾸로 유저가 Submit을 받아서 로직이 이어서 실행이 되어야 할 때

	// DataModel을 이용해서 유저 요청을 파싱할 수 있다 -> 서버 쪽에 데이터가 왔을 때의 처리에 관한 내용
	// 클라에서 어떤 Controller /Action / 데이터를 보낼것인가?

	// HTML로 손수 다 작성해도 되긴 함
	// Tag Helper를 이용하면 쉽게 처리 가능함
	/***********************************************************************************************/

	// # WebAPI
	// MVC의 View가 HTML을 반환하지 않고, JSON / XML 데이터를 반환하면 WebAPI
	// 나머지 Routing, Model Binding, Validation, Response 등은 동일

	// IActionResult 대신 List<string> 데이터를 반환하면 그게 WebAPI
	// 이렇게 바로 Data를 반환하면, ApiModel이 적용되어 Asp.NET Core에서 default로 JSON으로 만들어서 보냄
	// 그렇다고 해서 WebAPI가 꼭 데이터를 반환해야 하는건 아님 ex) 삭제요청 -> 성공여부를 Success 200, Fail 404 로 반환할 수 있음

	// Asp.NET (Core 아님) 에서는 MVC / WebAPI가 분리되어 있었음
	// Asp.NET Core로 넘어오면서 MVC / WebAPI가 동일한 프레임워크를 사용

	/***********************************************************************************************/

	// # Dependency Injection (DI, 종속성 주입)
	// 등장이유 : 디자인패턴에서는 코드간 종속성을 줄이는 것을 중요하게 생각 (Loosely Coupled)

	// 생성자에서 new를 해서 직접 만들어줘야 하는가? -> N

	// 1) Request
	// 2) Routing
	// 3) Controller Activator (DI Container 한테 Controller 생성 + 알맞는 Dependency 연결 위탁)
	// 4) DI Container 임무 실시
	// 5) Controller가 생성 끝!

	// 만약 3번에서 요청한 Dependency를 못 찾으면 -> Error
	//  -> ConfigureServices에 등록을 해야한다

	// 서비스 등록 방법
	// - Service Type (인터페이스 or 클래스)
	// - Implementation Type (클래스)
	// - LifeTime (Transient, Scoped, Singleton)
	// AddTransient, AddScoped, AddSingleton

	// 원한다면 동일한 인터페이스에 대해 다수의 서비스 등록 가능 -> IEnumerable<IBaseLogger>

	// 보통 생성자에서 DI를 하는게 국룰이지만 Action에서도 [FromServices]를 이용해서 DI를 붙일 수 있음

	// Razor View Template에서도 서비스가 필요하다면? 
	// -> 이경우 생상자를 사용할 수 없으니 -> @inject 사용

	// LifeTime
	// DI Container에 특정 서비스를 달라고 요청하면 
	//  1) 새로 만들어서 반환 하거나
	//  2) 이미 만들어져 있는걸 반환 하거나
	// 즉, 서비스 instance를 재사용 할지 말지를 결정

	// Transient - 생명주기 짧음 (항상 새로운 서비스 instance를 만든다. 매번 new)
	// Scoped    - 생명주기 중간 (동일한 요청(HTTP Request) 내에서만 같음. DbContext, Authentication 등) << 가장 일반적으로 사용됨
	// Singleton - 생명주기 김   (항상 동일한 instance를 사용) << 절대바뀌지 않는 수학공식등에 사용할 수 있음
	//  - 웹에서의 싱글톤은 thread-safe 해야 함
	/***********************************************************************************************/

	// # Configuration
	// 외부로 값을 빼서 설정
	//  1) 설정값
	//  2) 비밀값 (ConnectionString)

	// 대부분의 설정들은 CreateDefaultBuilder에서 발생
	//  1) ConfigureAppConfiguration  << App 설정 / 비밀값 관리
	//  2) ConfigureLogging           << Logging
	//  3) UseDefaultServiceProvider  << DI Container 설정
	//  4) UseKestrel				  << Kestrel
	//  5) ConfigureServices		  << Services
	//  6) UseIISIntegration          << IIS를 ReverseProxy 설정

	// Configuration Step
	//  1) ConfigurationBuilder 만든다
	//  2) 각종 ExtensionMethod를 이용해서 설정 방법 추가
	//   - AddJsonFile() -> JsonConfigurationProvider에 의해 설정값을 추가한다
	//   - AddCommendLine() -> CommendLineProvider에 의해 설정값을 추가한다
	//   - AddEnvironmentVariables() - 환경변수에 의해 설정값을 추가한다
	//   - 그 외) XML, Azure Key ...
	//  3) Build() 실행
	//  4) IConfigurationRoot에 결과물 저장
	//  5) IConfiguration에 결과물 저장

	// 실제 ConfigureAppConfiguration 코드 분석 결과 순서
	//  1) JSON file provider (appsettings.json)
	//  2) JSON file provider (appsettings.{ENV}.json)
	//  3) UserSecrets  << Development 환경일때만
	//  4) Env Variable (환경변수)
	//  5) CommandLine
	// 마지막에 등록하는 Provider가 덮어쓴다

	// Secret : 비밀스러운 Config
	// 비밀번호, DB ConnectionString
	// 대안으로 환경 변수 사용을 고려할 수 있음 (appsettings.json 보다 후순위이기 때문에)
	// 개발환경이라면 UserSecrets 사용 -> 솔루션 우클릭 -> 사용자 암호 관리 -> screts.json

	// _configuration["Logging:LogLevel:Default"]; 처럼 문자열로 값을 가져오는 방법 외에
	//  1) 모델링 클래스 (POCO) 하나를 만든다. 만들때 public getter/setter 도 만들어 준다
	//  2) Startup에 등록
	//  3) IOptions<>로 DI
	// 이 방법은 Reload 적용 X
	// Reload가 필요하다면 IOptionsSnapshot<> 사용

	// 개발 환경에 따른 Configuration
	// 개발 단계, Live 단계 등에 따라 로깅을 다르게 한다거나
	// ASP.NET Core에서 현재 환경을 알아내는 방법은 ASPNETCORE_ENVIRONMENT라는 환경변수 이용
	//  - Development
	//  - Staging
	//  - Production
	// 위의 이름을 사용하는걸 추천 : 관련 헬퍼 클래스들이 이미 만들어져 있기 때문 ex) IWebHostEnvironment의 IsDevelopment()
	public class TestObject
	{
		public string Id { get; set; }
		public string Password { get; set; }
	}

	[Route("Home")]
	public class HomeController : Controller
	{
		public IConfiguration _configuration { get; }
		public IOptions<TestObject> _options;

		public HomeController(IConfiguration configuration, IOptions<TestObject> options)
		{
			_configuration = configuration;
			_options = options;
		}

		[Route("Index")]
		[Route("/")]
		public IActionResult Index()
		{
			var test1 = _configuration["Test:Id"];
			var test2 = _configuration["Test:Password"];

			var test3 = _configuration["Logging:LogLevel:Default"];
			var test4 = _configuration.GetSection("Logging")["LogLevel:Default"];
			var test5 = _configuration["secret"];

			var test6 = _options.Value.Id;
			var test7 = _options.Value.Password;

			return Ok();
		}

		[Route("Privacy")]
		public IActionResult Privacy()
		{
			return View();
		}

		[Route("Error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
