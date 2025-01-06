using System.ComponentModel.DataAnnotations;

namespace University.Models;

public class Animal
{
    public long AnimalId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;    
    public int Age { get; set; } = 0;
}
