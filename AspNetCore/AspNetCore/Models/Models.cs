using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
	public class TestModel
	{
		[Required]
		public int Id { get; set; }

		[Required]
		[StringLength(20)]
		public string Name { get; set; }
	}

	public class TestViewModel
	{
		public List<string> Names { get; set; }
	}
}
