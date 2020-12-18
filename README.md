# MistTrainGirlsThief

------------

ミストレのリソース保存ツール


### 使い方

1. MistTrainGirlsThief.exeを起動する
1. 表示に従ってブラウザのプロキシを設定する（[Falcon Proxy](https://chrome.google.com/webstore/detail/falcon-proxy/gchhimlnjdafdlkojbffdkogjhhkdepf?hl=ja)などを使うと便利）
1. ミストレを遊ぶ
1. 作業フォルダの中のMistTrainGirlsフォルダの中にリソースが保存される


### 動作確認環境

Windows 10 / .NET 5.0


### 注意事項

本家ゲームの通信はすべてSSL/TLSで暗号化されているため、オレオレ証明書で中間者攻撃による復号をしています。  
本ソフト内で通信内容を故意に書き換えてはいませんが、専ブラなどのツールでよくある「通信内容への干渉はしていません」とは言い切れないのでご注意ください。


### ライセンス

MIT License


### 使用ライブラリ

- [Reactive Extensions](https://github.com/dotnet/reactive)
  - MIT License

- [Titanium Web Proxy](https://github.com/justcoding121/Titanium-Web-Proxy)
  - MIT License
