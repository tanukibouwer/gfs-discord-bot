# Setting up the bot

## Setting up the configuration file

The GFS bot now uses a `config.json` file to set it up. It must be placed in the same folder as the `.exe` file. A template file called `config_example.json` for the configuration file has been provided in the same folder. It can now also be used to specify a custom command prefix for the bot.

### Bot token and API token

In the configuration file there's an entry for the API token called `"Api"` and an entry for the bot token called `"Token"`. The old system with the two .txt files for the bot and API token is now obsolete. 

### Roles

For the role message system, you can specify the roles that people may give themselves through a role message. (More info on the role message itself later.) The roles can be specified through a simple `.json` list. Here's the list from the example file:

```
	"Roles": {
		"Programmer": {
			"Emoji": "🖥",
			"RoleID": 773603028301119519
		},
		"Arist": {
			"Emoji": "🖌️",
			"RoleID": 773603030129180692
		},
		"Musician": {
			"Emoji": "🎵",
			"RoleID": 773603082000400414
		},
		"Ping me": {
			"Emoji": "🏓",
			"RoleID": 774331880953872385
		}
	}
```

For each role entry's name, you specify the name of the role. *This does not have to be the name of the role in Discord!* This can be whatever you'd like, it's just how it'll show up in the role message. Inside the role entry you specify the emoji that's used in the reaction. These are normal Unicode emojis. You can find them on sites like [emojipedia.org](https://emojipedia.org/). If you're on Windows 10, you can also enter an emoji through `WIN + ;`.  After the emoji you specify the ID of the actual role on Discord. This can be found by trying to mention a role (for example by typing `@Programmer` in the case of this example) but putting a backslash at the front. This will make it so that when you send it, it'll show the role's ID instead of mentioning it. 

*Note: It'll look like this: `<@&773603028301119519>`, but you only need the numerical part, so `773603028301119519`.*

Here are some recommendations for roles:

- Skill roles (Programmer, artist, musician, designer...)
- Ping me! (For people who would like to be pinged for new GFS posts.)
- Pronoun roles. (She/him/they, can help avoid confusion)
- Game engine roles (Godot, Unity, UE4...)

## Setting up the bot role

The bot needs a couple of permissions. Even if the bot has been added before the 2.0 update, please make sure its role has been updated to include all of these permissions:

- Manage roles (New in 2.0)
- Send Messages
- Read message History

If you're also using your own application for the bot on Discord, you should also make sure that, in the bot section, under 'Privileged Gateway Intents', 'Presence Intent' and 'Server Members Intent' are both *enabled.* Otherwise, the role message reaction system can't function properly. 

## Setting up the role message

At this point the bot should be ready to start up. If anything goes wrong, the bot will generally output an error to the console, so keep an eye on that if something doesn't work.

A set of new commands have been added for the role message. To use these, you must have the 'Manage Roles' permission on the server. They are:

- `create_role_message` Use this in a channel like `#announcements`, and in that channel the bot will create a role message with the roles in the configuration file. The IDs of this message and its channel get stored in the configuration file, so that the same message will keep working after restarts. (This means that whenever you create a role message, the configuration file will be reloaded and saved, so keep that in mind if you're editing the config file while setting this all up.)
- `remove_role_message` This will delete the current role message the bot uses, if it exists. Note that this command does *not* have to be sent in the same channel as the role message. (Just like the previous command, this will also make changes to the configuration file.)
- `reload_config` With this you can manually make the bot reload its configuration file. Useful if you want to make changes to the configuration file while the bot keeps running.
- `refresh_role_message` If a role message exists, this will 'refresh' the message in the role message to keep it updated with its current configuration. Keep in mind that you should first reload the config file through `reload_config` before calling this command. Also, while this does remove any roles that you may have deleted from the config file from the message text, it will _not_ remove the roles you've deleted from users, and it will _not_ remove any reactions to the message. (Since the bot can't add reactions for other people, the reactions will be kept to prevent accidentally deleting reactions, so you'll have to delete unused reactions manually.)

