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
		// ���� ���� �߰� (DI)
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			// DI ���񽺶�? SRP (Single Responsibility Principle)
			// ex) ��ŷ ���� ����� �ʿ��ϸ� -> ��ŷ ����
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// HTTP Request Pipeline (NodeJS�� ����)
		// � HTTP ��û�� ���� ��, ���� ��� �����ϴ����� ���� �Ϸ��� ����
		// 1) IIS, Apache � HTTP ��û
		// 2) ASP.NET Core ���� (Kestrel) ����
		// 3) �̵���� ����
		//   - �̵���� : HTTP request / response�� ó��
		// 4) Controller�� ����
		// 5) Controller���� ó���ϰ� View�� ����
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

			//CSS, Javascript, �̹��� �� ��û ���� �� ó��
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			// �����(Routing) : ������
			// HTTP request <-> ��� handler �����ϴ� ��

			// ASP.NET �ʱ� ���������� /hello.aspx�� ���� ó���ϴ� ���� ��ü�� URL�� �Է�
			//  1) ���� �̸��� �ٲ�� -> Ŭ���̾�Ʈ �� ó���� ���� ���� ������ ���� �Ұ���
			//  2) ���ڵ��� �ѱ� �� /hello.aspx?method1=&id=3 �� ���� QueryString ����� URL
			//     -> ���� ����� /hello/get/3

			// �⺻ ����(Convention)�� Controller/Action/Id ����
			// �ٸ� �̸� �����ϰ� ���� �� 
			//  -> API ������ ����� ��, URL �ּҰ� � ������ �ϴ��� �� ��Ȯ�ϰ� ��Ʈ�� ��
			//  -> ���� Controller�� �������� �ʰ� ����� URL�� ��ü�ϰ� ������

			// Routing�� ����Ƿ��� �̵���� ���������ο� ���� ������ �Ǿ�� ��.
			//  -> �߰��� ������ ���ų�, Ư�� �̵��� �帧�� ����ë�ٸ� ����� ������ �ȵ�.

			// ���������� ������ ����������, MapControllerRout�� ���� Routing ��Ģ�� ����
			//  - ������ �̿��� ������� Routing
			//  - Attribute Routing

			// Route Template (Pattern)
			// name : "default" -> name���� �ټ��� ����� ������ ����� �� ����

			app.UseEndpoints(endpoints =>
			{
				// api : literal value (���� ���ڿ� ��, �� �ʿ�)
				// {controller} {action} : route parameter (�� �ʿ�)
				// {controller=Home} {action=Index} : optional route parameter (������ �˾Ƽ� �⺻�� ����)
				// {id?} : optional route parameter (��� ��)
				// [����] controller�� action�� ������ ���������� (��Ī or �⺻���� ���ؼ�)

				// Constraint ����
				// {controller=Home}/{action=Index}/{id?}
				// id�� �������ϴٴ� ������ ����  /1/2/3
				// {cc:int} ������
				// {cc:min(18)} 18�̻� ������
				// {cc:lenth(5)} 5���� string

				// Default Value�� Constraint�� �����ϴ� 2��° ��� (Anonymous Object)

				// Match-All (��Ŀī��)
				// {*joker} *�� ���̸� ��� ���ڿ��� �� ��Ī�����ش�


				// Redirection : �ٸ� URL�� �佺
				// Redirect(url) << URL ���� ����
				//  - Url.Action
				//  - Url.RouteUrl
				// RedirectToAction()
				// RedirectToRoute()

				endpoints.MapControllerRoute(
					name: "test",
					pattern: "api/{test}",
					defaults: new { controller = "Home", action = "Privacy"},
					constraints: new { test = new IntRouteConstraint()});

				//����� ���� ����
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
