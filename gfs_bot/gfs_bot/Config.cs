using System.Collections.Generic;
using Discord.Commands;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Discord;

namespace gfs_bot
{
	public class Config
	{
		public char Prefix = '!';
		public string Api = "";
		public string Token = "";
		public Dictionary<string, ConfigRole> Roles;
		public RoleMessageInfo RoleMessage;

		public static string Reload()
		{
			// Not exactly the best way of doing error handling but this will work for now.
			string configText;
			try
			{
				configText = File.ReadAllText("config.json");
			}
			catch(System.Security.SecurityException)
			{
				return string.Format("No sufficient permission to read `config.json` from '{0}'.", Path.GetFullPath("config.json"));
			}
			catch(System.IO.FileNotFoundException)
			{
				return string.Format("Could not find `config.json` at '{0}'.", Path.GetFullPath("config.json"));
			}
			catch
			{
				return string.Format("Could not read `config.json` from '{0}'. (Does it exist?)", Path.GetFullPath("config.json"));
			}

			try
			{
				Program.config = JsonConvert.DeserializeObject<Config>(configText);
			}
			catch
			{
				return "Error parsing the json data.";
			}
			return "";
		}

		public static string Save()
		{
			string configText = JsonConvert.SerializeObject(Program.config);

			try
			{
				File.WriteAllText("config.json", JsonPrettify(configText));
			}
			catch
			{
				return "Error writing to config.json";
			}

			return "";
		}

		private static string JsonPrettify(string json)
		{
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter())
			{
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
				jsonWriter.WriteToken(jsonReader);
				return stringWriter.ToString();
			}
		}
	}

	public struct ConfigRole
	{
		public string Emoji;
		public ulong RoleID;
	}

	public struct RoleMessageInfo
	{
		public ulong ChannelID;
		public ulong MessageID;
	}

	public class ConfigModule : ModuleBase<SocketCommandContext>
	{
		[RequireUserPermission(GuildPermission.ManageRoles)]
		[Command("reload_config")]
		[Summary("Refreshed the configuration file.")]
		public async Task ReloadConfigFile()
		{
			string result = Config.Reload();
			if (result == "")
			{
				await Context.Message.AddReactionAsync(new Emoji("👍"));
			}
			else
			{
				await ReplyAsync(string.Format("An error occured: {0}", result));
			}
		}
	}
}
