### Adds joystick.tv as a possible chat service for BeatSaberPlus.

Beta software - It works, but could use more testing... hopefully this is where you come in!

<img width="741" height="342" alt="image" src="https://github.com/user-attachments/assets/8c72df0d-2b21-4a42-a368-3f6d2fbd39cb" />

### Setup - JSTV website:
1. Visit https://joystick.tv/applications
2. Create a new bot
3. Fill in a username. This will be displayed in chat and should be specific to you, not something generic like "BeatSaber"
4. Make a note of the Application ID, Client ID and Client Secret
   <img width="604" height="823" alt="image" src="https://github.com/user-attachments/assets/638bfce5-8ea3-488b-b537-02c66f350611" />
5. Give it all the permissions for now, I'll figure out what it actually needs later! (or dont, and tell me the minimum needed to make this work!)
6. Set the OAuth Redirect URL to http://localhost:6970

### Setup - Beat Saber
7. Have BeatSaberPlus configured and working: https://github.com/hardcpp/BeatSaberPlus
8. Copy BSP_JSTV.dll into the Plugins directory of your Beat Saber install
9. Make sure you unblocked it (right click -> properties -> unblock)
10. Start Beat Saber, then close it
11. Open BSP-JSTV.json in your BeatSaber\UserData directory
12. Fill in the Application ID, Client ID and Client Secret you noted at step 4
13. Start Beat Saber - do not wear your VR headset for this part!
14. A webpage will open asking you to grant permissions, allow it and Beat Saber will load normally from now on

If successful you should see Joystick messages alongside Twitch ones, chat should be relayed, requests should work etc. Feature partity should be very close to that of Twitch

Basic chat requests have been tested and confirmed working. In-game notifications for subscriptions, tips, follows etc. have not. Please contact me if these aren't working (I'll probably need to see your log files)

### Credits
Based on [BSP-WS by MilkyDelta](https://github.com/milkydelta/BSP-WS) - This would not have been possible without having this to base things on

### Shameless plug
https://joystick.tv/u/LumKitty  
https://twitch.tv/LumKitty  
https://velora.tv/LumKitty  
