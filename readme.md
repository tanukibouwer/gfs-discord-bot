# A Gamefromscratch Discord bot!
A Discord bot to quickly test whether Gamefromscratch has covered a certain topic or not.

People often ask on the GFS discord server whether Gamefromscratch has already covered a certain topic or not. Wouldn't it be great if you could just quickly check without going all the way over to Youtube or his site? That's what this bot is for!

## Usage
When the bot is online, type !check [name of topic here], and the bot will automatically look for a GFS Youtube video on the topic.

For example:
![!check libgdx](https://i.imgur.com/AYpcL8B.png)

## Contributing
### Dependencies
- Discord.Net
- Google.Apis
- Google.Apis.Youtube.v3
### Build
You can just build it through Visual Studio 2019 with the .NET Core workload, or through the command-line using the .NET Core SDK.

The bot needs a Discord bot token (To take control over a bot) and a Google API key (To search the GFS Youtube channel) to work.
You can supply these by adding a `token.txt` file and an `api.txt` file in the project folder and then setting 'Copy to Output Directory' to true or by just directly putting these in the output directory.
`token.txt` should just contain a Discord bot token and nothing else, and `api.txt` should contain a Google API key and nothing else.
The bot should then automatically read these files and be able to properly connect with Discord and Youtube.

## License
[MIT](https://choosealicense.com/licenses/mit/)