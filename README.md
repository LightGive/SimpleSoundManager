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
[Example]("https://lightgive.github.io/MyPage/Examples/SimpleSoundManagerExample/index.html")<br>

## Download (UnityPackage)
[SimpleSoundManager(1.1.0)_IncludedExample]()<br>
[SimpleSoundManager(1.1.0)_NoExample]()<br>

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

## Future
・軽量化<br>
・UIのボタンなどを操作した時の音を簡単に設定できるように<br>
・Timelineに対応
