using System;
using System.IO;
using System.Reflection;
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

		static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();
			_commandService = new CommandService();
			var commands = new CommandHandler(_client, _commandService);

			_client.Log += Log;
			_commandService.Log += Log;

			
			// Get bot token from a text file with the bot token in it.
			string token = File.ReadAllText("token.txt");

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			await commands.InstallCommandsAsync();

			await Task.Delay(-1);

		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
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

			if (!(message.HasCharPrefix('!', ref argPos) || 
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
