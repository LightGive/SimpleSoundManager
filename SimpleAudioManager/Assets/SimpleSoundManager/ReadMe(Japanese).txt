Simple Sound Manager
(c) 2017 LightGive


[バージョン]
1.0.0


[使用方法]

・まず、使用する音ファイルを「Assets / SimpleSoundManager / Source / BGM or SE」のフォルダに入れてください。

・その後、「AudioName.cs」が自動で生成されます。これは、SEやBGMの再生時に引数として使う事が出来ます。

・再生方法
再生する音の名前の引数は、String型とEnum型のどちらでもいいです。
例1：AudioName.SE_ExamplesE(string型)
例2：AudioNameSE.SE_ExamplesE(enum型)

一番シンプルなBGMの再生方法
SimpleSoundManager.Instance.PlayBGM(AudioName.SE_ExampleSE); or SimpleSoundManager.Instance.PlayBGM(AudioNameSE.SE_ExamplesE);

一番シンプルなSEの再生方法
SimpleSoundManager.Instance.PlaySE2D(AudioName.SE_ExampleSE); or SimpleSoundManager.Instance.PlaySE2D(AudioNameSE.SE_ExamplesE)

BGMのクロスフェード再生
SimpleSoundManager.Instance.PlayCrossFadeBGM(AudioName.BGM_ExampleBGM)