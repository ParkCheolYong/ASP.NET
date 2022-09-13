using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspnetNote.MVC6.Models
{
	public class Note
	{
		/// <summary>
		/// 게시물 번호
		/// </summary>
		[Key] //PK 설정
		public int NoteNo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Required] // Not Null 설정
		public string NoteTitle { get; set; }

		/// <summary>
		/// 게시물 제목
		/// </summary>
		[Required] // Not Null 설정
		public string NoteContents { get; set; }

		/// <summary>
		/// 작성자 번호
		/// </summary>
		[Required] // Not Null 설정
		public int UserNo { get; set; }

		[ForeignKey("UserNo")]
		public virtual User User { get; set; }
	}
}
