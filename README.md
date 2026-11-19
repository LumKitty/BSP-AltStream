### Adds AltStream as a possible chat service for BeatSaberPlus.

Beta software - It works, but could use more testing... hopefully this is where you come in!

<img width="404" height="324" alt="image" src="https://github.com/user-attachments/assets/bc0ab695-8dc4-4cdf-b63a-0306bdb24bff" />

### Setup - AltStream website:
1. Visit https://altstream.online/lumkitty/settings?tab=bot
2. Create a new bot, make a note of the token

### Setup - Beat Saber
3. Have BeatSaberPlus configured and working: https://github.com/hardcpp/BeatSaberPlus
4. Copy BSP_AltStream.dll into the Plugins directory of your Beat Saber install
5. Make sure you unblocked it (right click -> properties -> unblock)
6. Start Beat Saber, then close it
7. Open BSP-AltStream.json in your BeatSaber\UserData directory
8. Fill in your channel name and bot token you noted at step 2
9. Start Beat Saber

If successful you should see AltStream messages alongside Twitch ones, chat should be relayed, requests should work etc. Feature partity should be very close to that of Twitch

Basic chat requests have been tested and confirmed working. In-game notifications for subscriptions, tips, follows etc. have not. Please contact me if these aren't working (I'll probably need to see your log files)

Emote parsing is not yet supported

### Credits
Based on [BSP-WS by MilkyDelta](https://github.com/milkydelta/BSP-WS) - This would not have been possible without having this to base things on

### Shameless plug
https://altstream.online/LumKitty  
https://joystick.tv/u/LumKitty  
https://twitch.tv/LumKitty  
