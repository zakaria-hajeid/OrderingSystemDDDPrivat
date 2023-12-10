using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdenityApi.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string CardNumber { get; set; }
    [Required]
    public string SecurityNumber { get; set; }

    [Required]
    public string CardHolderName { get; set; }
    [Required]

    public string Name { get; set; }
}

