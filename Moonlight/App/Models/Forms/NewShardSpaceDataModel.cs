using System.ComponentModel.DataAnnotations;

namespace Moonlight.App.Models.Forms;

public class NewShardSpaceDataModel
{
    [Required(ErrorMessage = "You need to specify a shard space name")]
    [MaxLength(30, ErrorMessage = "The space name can only be maximum 30 characters long")]
    public string Name { get; set; }
}