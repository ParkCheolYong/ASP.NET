using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCoreStudy.Entities
{
	/// <summary>
	/// 모델 클래스
	/// </summary>
	//[Table("s_Users")] //DB의 이름과 다를때 맵핑
	public class User
	{
		public int UserId { get; set; }

		public string UserName { get; set; }

		public string Birth { get; set; }

		public int PositionId { get; set; }

		public Position Position { get; set; }
	}
}
