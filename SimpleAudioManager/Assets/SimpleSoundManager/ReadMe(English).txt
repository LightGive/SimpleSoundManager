Simple Sound Manager
(c) 2017 LightGive


[Version]
1.0.0


[How to use]

-First of all, please put the sound file to use in "Assets / SimpleSoundManager / Source / BGM or SE" folder.

-After that, "AudioName.cs" is automatically generated. This can be used as an argument when playing SE or BGM.

-Playback method
The name argument of the sound to be played can be either String type or Enum type.
AudioName.SE_ExampleSoundEffect(string type)
AudioNameSE.ExampleSoundEffect(enum type)

Simple BGM playback method
SimpleSoundManager.Instance.PlayBGM(AudioName);

BGM cross fade playback
SimpleSoundManager.Instance.PlayCrossFadeBGM(AudioName.BGM_ExampleBGM , 1.0f , 0.5f);

Simple SE playback method
SimpleSoundManager.Instance.PlaySE2D(AudioName);

