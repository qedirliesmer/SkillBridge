using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Application.DTOs.SkillDTOs;

public class UpdateSkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public IFormFileCollection? NewImages { get; set; }
    public List<int>? RemoveMediaIds { get; set; }
}
