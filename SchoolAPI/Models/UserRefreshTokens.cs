using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolAPI.Models
{
	public class UserRefreshTokens
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		public string UserName { get; set; }
		public string Password { get; set; }
		[Required]
		public string RefreshToken { get; set; }
    }
}
