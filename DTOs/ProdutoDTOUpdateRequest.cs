namespace APICatalogo.DTOs;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ProdutoDTOUpdateRequest : IValidatableObject
{
    [Range(1, 9999, ErrorMessage = "O estoque deve estar entre 1 e 9999.")]
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataCadastro <= DateTime.Now.Date)
        {
            yield return new ValidationResult("A data de cadastro deve ser uma data futura.", new[] { nameof(DataCadastro) });
        }
    }
}
