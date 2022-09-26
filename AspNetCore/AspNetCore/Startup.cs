using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// 각종 서비스 추가 (DI)
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			// DI 서비스란? SRP (Single Responsibility Principle)
			// ex) 랭킹 관련 기능이 필요하면 -> 랭킹 서비스
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// HTTP Request Pipeline (NodeJS와 유사)
		// 어떤 HTTP 요청이 왔을 때, 앱이 어떻게 응답하는지에 대한 일련의 과정
		// 1) IIS, Apache 등에 HTTP 요청
		// 2) ASP.NET Core 서버 (Kestrel) 전달
		// 3) 미들웨어 적용
		//   - 미들웨어 : HTTP request / response를 처리
		// 4) Controller에 전달
		// 5) Controller에서 처리하고 View로 전달
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();

			//CSS, Javascript, 이미지 등 요청 받을 때 처리
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			// 라우팅(Routing) : 길잡이
			// HTTP request <-> 담당 handler 매핑하는 것

			// ASP.NET 초기 버전에서는 /hello.aspx와 같이 처리하는 파일 자체를 URL에 입력
			//  1) 파일 이름이 바뀌면 -> 클라이언트 쪽 처리를 같이 하지 않으면 접속 불가능
			//  2) 인자들을 넘길 때 /hello.aspx?method1=&id=3 와 같이 QueryString 방식의 URL
			//     -> 지금 방식은 /hello/get/3

			// 기본 관례(Convention)는 Controller/Action/Id 형식
			// 다른 이름 지정하고 싶을 때 
			//  -> API 서버로 사용할 때, URL 주소가 어떤 역할을 하는지 더 명확하게 힌트를 줌
			//  -> 굳이 Controller를 수정하지 않고 연결된 URL만 교체하고 싶을때

			// Routing이 적용되려면 미들웨어 파이프라인에 의해 전달이 되어야 함.
			//  -> 중간에 에러가 나거나, 특정 미들웨어가 흐름을 가로챘다면 라우팅 적용이 안됨.

			// 파이프라인 끝까지 도달했으면, MapControllerRout에 의해 Routing 규칙이 결정
			//  - 패턴을 이용한 방식으로 Routing
			//  - Attribute Routing

			// Route Template (Pattern)
			// name : "default" -> name으로 다수의 라우팅 패턴을 사용할 수 있음

			app.UseEndpoints(endpoints =>
			{
				// api : literal value (고정 문자열 값, 꼭 필요)
				// {controller} {action} : route parameter (꼭 필요)
				// {controller=Home} {action=Index} : optional route parameter (없으면 알아서 기본값 설정)
				// {id?} : optional route parameter (없어도 됨)
				// [주의] controller랑 action은 무조건 정해져야함 (매칭 or 기본값을 통해서)

				// Constraint 관련
				// {controller=Home}/{action=Index}/{id?}
				// id가 광범위하다는 문제가 있음  /1/2/3
				// {cc:int} 정수만
				// {cc:min(18)} 18이상 정수만
				// {cc:lenth(5)} 5글자 string

				// Default Value와 Constraint를 설정하는 2번째 방법 (Anonymous Object)

				// Match-All (조커카드)
				// {*joker} *를 붙이면 모든 문자열을 다 매칭시켜준다


				// Redirection : 다른 URL로 토스
				// Redirect(url) << URL 직접 만들어서
				//  - Url.Action
				//  - Url.RouteUrl
				// RedirectToAction()
				// RedirectToRoute()

				endpoints.MapControllerRoute(
					name: "test",
					pattern: "api/{test}",
					defaults: new { controller = "Home", action = "Privacy"},
					constraints: new { test = new IntRouteConstraint()});

				//라우팅 패턴 설정
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");

				endpoints.MapControllerRoute(
					name: "joker",
					pattern: "{*joker}",
					defaults: new { controller = "Home", action = "Error" });
			});
		}
	}
}
