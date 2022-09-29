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

	public class TestViewModel2
	{
		[Required]
		[Display(Name ="구매할 아이템 ID")]
		public int Id { get; set; }

		[Range(1,10,ErrorMessage ="아이템 개수는 1 ~ 10")]
		[Display(Name ="수량")]
		public int Count { get; set; }
	}
}
