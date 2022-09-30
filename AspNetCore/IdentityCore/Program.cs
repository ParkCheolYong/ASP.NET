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
	// # �̵���� Ŀ���͸���¡

	// [Request]        [Response]
	// [Middleware]   [Middleware]
	//            [MVC]

	// �̵����� ����� ó��
	// �̵��� Response�� �����ϸ� Shortcut ����

	// 1) Run�� �̿��� �ʰ��� �̵����
	// 2) Branching �̵���� ����������
	//  - ���������� ���� �Ǿ�� �ϴ� ���� �ƴϴ�
	//  - �帧�� ����ë�ٰ� �ؼ� �� ���� �÷� ������ �͵� �ƴ�
	//  - Map
	// 3) Use�� �̿��� �ʰ��� ����� �̵����

	// Custom Middleware Component
	//  - �����ڿ� RequestDelegate�� ���ڷ� �޾ƾ� �� (next)
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
