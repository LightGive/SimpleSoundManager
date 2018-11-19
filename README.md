# SimpleSoundManager [![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?style=flat)](http://mit-license.org)<br>
Unityで使用できるシンプルなSoundManager<br>
小規模な開発向けで、基本的な機能は大体揃ってます。<br>

## Update <br>
<dl>
  <dt>ver1.1.0　(2018.10.21)</dt>
  <dd>・Windowsでスクリプト生成時にエラーが出ていたので全体的に修正</dd>
  <dd>・音楽ファイルを更新した時Enumの順番がズレてしまうことからInspector上で設定したSoundNameSEの値が変わってしまう不具合を修正</dd>
  <dd>・Android,iOSでの動作確認</dd>
  <dd>・SEのフェードイン,アウトの設定した時間が音より長い時の対策</dd>
</dl> 

## Example <br>
### 01.Example_PlaySE_2D
![Image1](https://66.media.tumblr.com/525e46e5d76428f9633c5b8690a60d9e/tumblr_pgzbflHM8b1u4382eo2_1280.png)<br>
### 02.Example_PlayBGM
![Image2](https://66.media.tumblr.com/d3b8c4ebc5469a2a5f920857b7566cbe/tumblr_pgzbflHM8b1u4382eo4_1280.png)<br>
### 03.Example_PlaySE_3D
![Image3](https://66.media.tumblr.com/c96060c638d2ab0687d3024f40d508cb/tumblr_pgzbflHM8b1u4382eo3_1280.png)<br>
### 04.Example_SettingUI_Sound
![Image4](https://66.media.tumblr.com/ec9e55b87e6d44ef0dfda43aea1a8700/tumblr_pgzbflHM8b1u4382eo1_1280.png)<br><br>
### Inspector
![Image](https://66.media.tumblr.com/09d51d44d99c6536326080189b71bbf9/tumblr_pgzbw7FNBR1u4382eo2_400.png)<br>
![Image](https://66.media.tumblr.com/4bd875ee5bb2e9f31eed8fa4921d6cda/tumblr_pgzbw7FNBR1u4382eo1_640.png)<br>
<br>
以下のリンクからWebGLの実行ファイルに移動出来ます。<br>
どんな機能があるか等の確認ができます。<br>
[SimpleSoundManager_Example](https://lightgive.github.io/MyPage/Examples/SimpleSoundManagerExample/index.html)<br>

## Download (UnityPackage)
[SimpleSoundManager(1.1.0)_IncludedExample](https://www.dropbox.com/s/6z16nkfglorzvj4/SimpleSoundManager%281.1.1%29_IncludeExample.unitypackage?dl=0)<br>

## Function
・オーディオファイルから音楽名のリスト（string,Enum）のスクリプトの自動生成<br>
・インスペクタ上でのテスト再生<br>
・ボリューム変更時の自動保存<br>

### 【SE】
・基本的なPause、Resume<br>
・ボリューム指定再生<br>
・フェードイン、フェードアウトの時間指定での再生<br>
・回数指定のループ再生<br>
・ピッチ指定の再生<br>
・再生開始、終了などのタイミングでのコールバック指定<br>
・座標指定をして3D再生<br>
・オブジェクトに追従して再生<br>
<br>
### 【BGM】
・基本的なPause、Resume<br>
・ボリューム指定再生<br>
・フェードイン、フェードアウトの時間指定での再生<br>
・クロスフェードの重ねる割合の設定<br>
・再生開始、終了などのタイミングでのコールバック指定<br>
・イントロ曲を指定して再生<br>

## Bug
・FPSが低下した時にループ再生やイントロからメインに曲が移る時に若干遅延が発生する(ver.1.1.0)<br>

## Future
・軽量化<br>
・UIのボタンなどを操作した時の音を簡単に設定できるように<br>
・Timelineに対応<br>

## UnityVersion
Unity 2018.2.13f1<br>

## License
See [LICENSE](/LICENSE).
