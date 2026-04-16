using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "username é obrigatorio")]
    public string? Username { get; set; }
    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatorio")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Password é obrigatorio")]
    public string? Password { get; set; }
}
