using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetNote.MVC6.Controllers
{
	public class UploadController : Controller
	{
		private readonly IWebHostEnvironment _environment;

		public UploadController(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		[HttpPost, Route("api/upload")]
		public async Task<IActionResult> ImageUpload(IFormFile file)
		{
			// # 이미지나 파일을 업로드 할 때 필요한 구성

			// 1. Path(경로) - 어디에다 저장할 지 결정
			var path = Path.Combine(_environment.WebRootPath, @"images\upload");

			// 2. Name(이름) - DateTime, GUID + GUID
			// 3. Extension(확장자) - jpg, png... 
			var fileFullName = file.FileName.Split('.');
			var fileName = $"{Guid.NewGuid()}.{fileFullName[1]}";

			using (var fileStream = new FileStream(Path.Combine(path,fileName), FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			return Ok(new { file = "/images/upload/" + fileName, success = true });
		}
	}
}
