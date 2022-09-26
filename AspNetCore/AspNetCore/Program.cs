using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AspNetCore
{
	// 왜 Program과 Startup으로 나눠어 세팅을 하는가
	// Program은 거시적인 설정 (Http 서버, IIS 사용여부등.. 거의 바뀌지 않음)
	// Startup은 세부적인 설정 (미들웨어 설정, Dependency Injection 등.. 필요에따라 수정)
	public class Program
	{
		//콘솔앱
		public static void Main(string[] args)
		{
			// 3) IHost를 만든다
			// 4) 구동(Run) < 이때부터 Listen을 시작
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			// 1) 각종 옵션 설정을 세팅
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					// 2) Startup 클래스 지정
					webBuilder.UseStartup<Startup>();
				});
	}
}
