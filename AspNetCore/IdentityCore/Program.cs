using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCore
{
	// # 미들웨어 커스터마이징

	// [Request]        [Response]
	// [Middleware]   [Middleware]
	//            [MVC]

	// 미들웨어는 양방향 처리
	// 미들웨어가 Response를 생성하면 Shortcut 역할

	// 1) Run을 이용한 초간단 미들웨어
	// 2) Branching 미들웨어 파이프라인
	//  - 순차적으로 실행 되어야 하는 것은 아니다
	//  - 흐름을 가로챘다고 해서 꼭 위로 올려 보내는 것도 아님
	//  - Map
	// 3) Use를 이용한 초간단 양방향 미들웨어

	// Custom Middleware Component
	//  - 생성자에 RequestDelegate를 인자로 받아야 함 (next)
	//  - public Task Invoke(HttpContext context);
	
	public class TestMiddleware
	{
		RequestDelegate _next;
		ILogger<TestMiddleware> _logger;


		public TestMiddleware(RequestDelegate next, ILogger<TestMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			_logger.LogInformation("TestMiddleware!");
			await _next(context);
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseStartup<Startup>()
						.ConfigureLogging((ctx,builder) => 
						{
							builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
							builder.AddConsole();
							builder.AddFile();
							builder.AddSeq();
						});
				});
	}
}
