using System.ComponentModel.DataAnnotations;

namespace Moonlight.App.Models.Forms;

public class NewShardProxyDataModel
{
    [Required(ErrorMessage = "You need to specify a name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "You need to specify a fqdn")]
    public string Fqdn { get; set; }
    
    [Required(ErrorMessage = "You need to specify a key")]
    public string Key { get; set; }
}