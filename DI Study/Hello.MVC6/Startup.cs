using Hello.BLL;
using Hello.IDAL;
using Hello.MSSQL.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hello.MVC6
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// MVC - 6 의존성 주입을 할 수 있는 3가지.
			// 1. AddSingleTon<T>();
			//   - 웹사이트가 시작하면 사이트가 종료될 때까지 메모리 상에 유지되는 객체 주입

			// 2. AddScoped<T>();
			//   - 웹사이트가 시작되어 1번의 요청이 있을 때 메모리 상에 유지되는 객체 주입

			// 3. AddTransient<T>();
			//   - 웹사이트가 시작되어 각 요청마다 새롭게 생성되는 객체 주입 

			services.AddTransient<UserBll>();
			services.AddTransient<IUserDal, UserDal>();

			services.AddMvc();
			//services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
