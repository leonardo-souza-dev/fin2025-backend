using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Api.Models;

public class Config
{
    public int? Id { get; set; }

    public required string Key { get; set; }

    public required string Value { get; set; }

    public bool IsActive { get; set; }
}
