# System Conqueror

This project contains a Unity game. To enable the optional fog-of-war behaviour that hides far away planets, a `Show Far Stars` toggle can be added to the setup panel.

## Setup instructions

1. Open the Unity project and locate the **SetupPanel** UI object (where you choose AI count, map size, etc.).
2. Create a `Toggle` UI element under this panel. Rename it `ShowFarStarsToggle` for clarity.
3. Select the object that has the **GameSetupUI** component. In its inspector, assign the new toggle to the `Far Stars Toggle` field (drag the toggle into the slot).
4. Ensure the **Play** button calls `GameSetupUI.OnPlayClicked` (use the Button component's `On Click` event list).
5. Verify that the **GameSetupUI** component actually shows your toggle in this field before you press *Play*. If the field is `None`, the option won't be read.
6. When starting the game, uncheck the toggle if you want to restrict visibility to conquered planets and their neighbours.

After conquering a new planet, the fog-of-war will update automatically and newly visible planets will show their names.
