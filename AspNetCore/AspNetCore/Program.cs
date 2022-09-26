using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AspNetCore
{
	// �� Program�� Startup���� ������ ������ �ϴ°�
	// Program�� �Ž����� ���� (Http ����, IIS ��뿩�ε�.. ���� �ٲ��� ����)
	// Startup�� �������� ���� (�̵���� ����, Dependency Injection ��.. �ʿ信���� ����)
	public class Program
	{
		//�ܼ־�
		public static void Main(string[] args)
		{
			// 3) IHost�� �����
			// 4) ����(Run) < �̶����� Listen�� ����
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			// 1) ���� �ɼ� ������ ����
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					// 2) Startup Ŭ���� ����
					webBuilder.UseStartup<Startup>();
				});
	}
}
