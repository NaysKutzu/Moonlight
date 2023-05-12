using System.ComponentModel.DataAnnotations;

namespace Moonlight.App.Models.Forms;

public class NewShardDataModel
{
    [Required(ErrorMessage = "You need to specify a name")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "You need to specify a full qualified domain name")]
    public string Fqdn { get; set; } = "";

    [Required(ErrorMessage = "You need to specify a sftp port")]
    public int SftpPort { get; set; } = 2022;

    [Required(ErrorMessage = "You need to specify a http port")]
    public int HttpPort { get; set; } = 8080;

    [Required(ErrorMessage = "You need to specify a shard port")]
    public int ShardPort { get; set; } = 9999;
    public bool Ssl { get; set; } = false;
}