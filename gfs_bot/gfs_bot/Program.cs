using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace gfs_bot
{
	class Program
	{
		private DiscordSocketClient _client;
		private CommandService _commandService;
		static public Config config;

		static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
			_client = new DiscordSocketClient(_config);
			_commandService = new CommandService();
			var commands = new CommandHandler(_client, _commandService);

			_client.Log += Log;
			_commandService.Log += Log;
			_client.ReactionAdded += RoleMessage.ReactionAdded;
			_client.ReactionRemoved += RoleMessage.ReactionRemoved;
			_client.UserJoined += DownloadUsers;
			_client.UserLeft += DownloadUsers;
			_client.Ready += RoleReady;


			// Load config file
			string configResult = Config.Reload();
			if (configResult == "")
			{
				await Log(new LogMessage(LogSeverity.Info, "Config", "Successfully loaded config file."));
			}
			else
			{
				await Log(new LogMessage(LogSeverity.Error, "Config", String.Format("Error loading config file: {0} Closing after 5 seconds...", configResult)));
				Thread.Sleep(5 * 1000);
				return;
			}

			await _client.LoginAsync(TokenType.Bot, config.Token);
			await _client.StartAsync();

			await commands.InstallCommandsAsync();

			await DownloadUsers();

			await Task.Delay(-1);
		}

		// Uhhhhhhhhhh I'm unsure what to do about this warning, because unless it returns task it won't let me hook up the event.
		private async Task RoleReady()
		{
			if (!config.RoleMessage.Equals(default(RoleMessageInfo)))
			{
				var channel = _client.GetChannel(config.RoleMessage.ChannelID) as IMessageChannel;
				RoleMessage.Message = await channel.GetMessageAsync(config.RoleMessage.MessageID) as IUserMessage;
			}
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task DownloadUsers(SocketGuildUser user)
		{
			await DownloadUsers();
		}

		private async Task DownloadUsers()
		{
			try
			{
				await Log(new LogMessage(LogSeverity.Info, "Main", "(Re)downloading all users..."));
				await _client.DownloadUsersAsync(_client.Guilds);
			}
			catch (ArgumentNullException)
			{
				await Log(new LogMessage(LogSeverity.Error, "Main", "Guilds is null, couldn't download users."));
			}
		}
	}

	
	public class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;

		public CommandHandler(DiscordSocketClient client, CommandService commands)
		{
			_commands = commands;
			_client = client;
		}

		public async Task InstallCommandsAsync()
		{
			_client.MessageReceived += HandleCommandAsync;

			await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
		}

		public async Task HandleCommandAsync(SocketMessage messageParam)
		{
			var message = messageParam as SocketUserMessage;
			if (message == null) return;

			int argPos = 0;

			if (!(message.HasCharPrefix(Program.config.Prefix, ref argPos) || 
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) 
				|| message.Author.IsBot) 
				return;

			var context = new SocketCommandContext(_client, message);

			await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: null);
		}
	}
}
