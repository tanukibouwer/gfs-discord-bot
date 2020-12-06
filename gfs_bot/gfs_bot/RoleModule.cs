using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace gfs_bot
{
	public class RoleModule : ModuleBase<SocketCommandContext>
	{
//		[RequireUserPermission(GuildPermission.ManageRoles)]
		[Command("create_role_message")]
		[Summary("Creates a message for people to get roles.")]
		public async Task CreateRoleMessage()
		{
			SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);
			if (!user.GuildPermissions.ManageRoles) {
				Console.WriteLine("create_role_message was called, but the user didn't have the manage roles permission!");
				return;
			}

			if (RoleMessage.Message != null)
			{
				await ReplyAsync("A role message already exists! Please remove that one first by calling `!remove_role_message`.");
				return;
			}
			await Context.Message.DeleteAsync();
			RoleMessage.SetUpMessage(await ReplyAsync("Hold on..."));
			await RoleMessage.RefreshMessage();
		}

		[RequireUserPermission(GuildPermission.ManageRoles)]
		[Command("refresh_role_message")]
		[Summary("Refreshed the role message if it exists.")]
		public async Task RefreshRoleMessage()
		{
			bool result = await RoleMessage.RefreshMessage();
			if (!result)
			{
				await ReplyAsync("Error: No role message found.");
			}
			else
			{
				await Context.Message.AddReactionAsync(new Emoji("👍"));
			}
		}

		[RequireUserPermission(GuildPermission.ManageRoles)]
		[Command("remove_role_message")]
		[Summary("Removes the message for people to get roles.")]
		public async Task RemoveRoleMessage()
		{
			if (RoleMessage.Message == null)
			{
				await ReplyAsync("No role message exists! Create one by calling `!create_role_message`.");
				return;
			}
			await RoleMessage.Message.DeleteAsync();
			RoleMessage.Message = null;
			Config.Reload();
			Program.config.RoleMessage.MessageID = 0;
			Program.config.RoleMessage.ChannelID = 0;

			Config.Save();
			await Context.Message.AddReactionAsync(new Emoji("👍"));
		}
	}

	static public class RoleMessage
	{
		// Currently the bot has a (major?) design flaw where it can only have one role message per instance. This is...not good, obviously, but in the currrent way that the bot works, it should be fine.
		// Besides, the whole role system already only works with one server, so, uhm, that just means we have an even BIGGER design flaw! Ohhh boy!
		// But yeah, I don't know whether it's worth fixing this right now or not since this bot is only really useful in the gfs server anyway.
		static public IUserMessage Message = null;

		static public async void SetUpMessage(IUserMessage _message)
		{
			Message = _message;
			foreach (KeyValuePair<string, ConfigRole> role in Program.config.Roles)
			{
				await Message.AddReactionAsync(new Emoji(role.Value.Emoji));
			}
			Config.Reload();
			Program.config.RoleMessage.MessageID = Message.Id;
			Program.config.RoleMessage.ChannelID = Message.Channel.Id;

			Config.Save();
		}

		static public async Task<bool> RefreshMessage()
		{
			if (Message == null)
			{
				return false;
			}
			await Message.ModifyAsync(x => x.Content = GenerateMessageText());
			return true;
		}

		static public string GenerateMessageText()
		{
			string message = "**React to this message with an emoji to get a role:**\n\n";

			foreach (KeyValuePair<string, ConfigRole> role in Program.config.Roles)
			{
				message += String.Format("{0}: {1}\n", role.Value.Emoji, role.Key);
			}

			return message;
		}

		static public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel messageChannel, SocketReaction socketReaction)
		{
			if (Message != null && socketReaction.MessageId == Message.Id && socketReaction.User.Value != null && !socketReaction.User.Value.IsBot)
			{
				foreach (KeyValuePair<string, ConfigRole> roleInfo in Program.config.Roles)
				{
					if (socketReaction.Emote.Equals(new Emoji(roleInfo.Value.Emoji)))
					{
						var user = socketReaction.User.Value as IGuildUser;
						var role = user.Guild.GetRole(roleInfo.Value.RoleID);
						await user.AddRoleAsync(role);
					}
				}
			}
		}

		static public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel messageChannel, SocketReaction socketReaction)
		{
			if (Message != null && socketReaction.MessageId == Message.Id && socketReaction.User.Value != null && !socketReaction.User.Value.IsBot)
			{
				foreach (KeyValuePair<string, ConfigRole> roleInfo in Program.config.Roles)
				{
					if (socketReaction.Emote.Equals(new Emoji(roleInfo.Value.Emoji)))
					{
						var user = socketReaction.User.Value as IGuildUser;
						var role = user.Guild.GetRole(roleInfo.Value.RoleID);
						await user.RemoveRoleAsync(role);
					}
				}
			}
		}
	}
}
