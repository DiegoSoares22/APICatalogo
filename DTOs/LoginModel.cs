using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class LoginModel
{
    [Required(ErrorMessage = "username é obrigatorio")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Password é obrigatorio")]
    public string? Password { get; set; }
}
