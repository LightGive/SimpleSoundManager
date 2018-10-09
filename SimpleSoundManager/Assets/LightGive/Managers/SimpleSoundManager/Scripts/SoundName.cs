public static class SoundName
{
	public const string BGM_SampleBGM1 = "SampleBGM1";
	public const string BGM_SampleBGM2 = "SampleBGM2";
	public const string BGM_SampleBGM3_Main = "SampleBGM3_Main";
	public const string BGM_SampleBGM3_Main_Intro = "SampleBGM3_Main_Intro";
	
	public const string SE_Enter = "Enter";
	public const string SE_Jump = "Jump";
	public const string SE_Shoot = "Shoot";
}
	
public enum SoundNameBGM
{
	None,
	SampleBGM1 = 3,
	SampleBGM2 = 4,
	SampleBGM3_Main = 5,
	SampleBGM3_Main_Intro = 6,
}
	
public enum SoundNameSE
{
	None,
	Enter = 1,
	Jump = 2,
	Shoot = 3,
}
