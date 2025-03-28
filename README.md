# RenardSample
RenardをベースにMediapipeを使ったサンプルコンテンツ

## 注意事項
大本のソースは「SignageHADO」という株式会社meleapで私が在籍中に作成したコンテンツです。  
ご厚意により、ポートフォリオとしてソースコードを開示利用してもよいという承諾を得て使用しております。   
一部演出や表記、会社情報に関わるような重要情報的なものは除外や変更させております。

## 概要
[Renard](https://github.com/rencotsuki/Renard)というUnityPackageManagerのパッケージで、  
カジュアルコンテンツの基礎となる機能パッケージ化したものを利用している。  
Renardは、元々個人的に何かゲームコンテンツを作ろうと思い、  
その基盤となるものとして作っていたものを、  
流用して組み上げられている経緯があります。  

業務で計画されていたカジュアルコンテンツにおいては、  
このRenardをベースにインゲームと演出部分を組み替えることで、  
バリエーションを増やしていく計画で利用されました。  
それのモデルベースになるものだったのが「SignageHADO」です。  

## 動作環境について
実機動作環境は、**iPhoneSE2以降の端末でiOS16以降**をターゲットとして作成しております。  

UnityEditorでは、Windows、Mac(OSX)どちらでも動作確認できます。  
※PCで動作する際はWebカメラが必要です。  

Androidにおいては、ビルドが出来る状態になってはいますが、
mediapipeの調整が完了していないため、
トラッキングによる動作は実機で確認はできない状態にあります。
申し訳ございません、時間を取って対応はしていくつもりです。

## 使用方法について
ビルドしたアプリケーションの使用方法は、  
サイネージモニタ（物理的に縦置きができるモニタ） と カメラ付き動作端末（iPhoneやWindowsPC＋webカメラ） が必要で、  
動作端末にアプリケーションをインストールして準備(※1)を行った後、  
画像のようにモニタと動作端末を映像ケーブルで繋いで使用します。  
<img width="512" alt="使用図" src="https://github.com/rencotsuki/RenardSample/blob/11a8b08cebb3a294a4b9f2ef430fcdd392e60be0/DocumentSamples/%E4%BD%BF%E7%94%A8%E5%9B%B3.jpg">  
※1 アプリケーションを動作させるにはライセンスのアクティベートとAssetBundleを端末にダウンロードする準備が必要です。  

実際にアプリケーションを立ち上げ動作させると、モニタと動作端末の画面にはそれぞれ違った表示がされます。  
モニタ側でプレイ画面が表示され、動作端末側ではトラッキングのためのカメラ表示や管理操作するための画面が表示されます。  
<img width="180" alt="モニタ画面" src="https://github.com/rencotsuki/RenardSample/blob/11a8b08cebb3a294a4b9f2ef430fcdd392e60be0/DocumentSamples/%E5%A4%96%E9%83%A8%E3%83%87%E3%82%A3%E3%82%B9%E3%83%97%E3%83%AC%E3%82%A4%E5%87%BA%E5%8A%9B%E5%81%B4%E3%81%AE%E7%94%BB%E9%9D%A2.jpg">　<img width="180" alt="動作端末画面" src="https://github.com/rencotsuki/RenardSample/blob/11a8b08cebb3a294a4b9f2ef430fcdd392e60be0/DocumentSamples/%E5%8B%95%E4%BD%9C%E7%AB%AF%E6%9C%AB%E5%81%B4%E3%81%AE%E7%94%BB%E9%9D%A2.jpg">  
（左：モニタの画面、右：動作端末の画面）  

実際の動作させてプレイしている動画がこちらです。  
⇒[プレイ中の動画](https://drive.google.com/file/d/1wbb4lTbfZR02hn_THypDpddlKjyyOrcb/view?usp=drive_link)  
⇒[通しプレイ画面の動画](https://drive.google.com/file/d/1sl0XN9ofkdylaPNJ0MmVHTIzuo6uTch_/view?usp=drive_link)  

実際にSignageHADOの業務で私が作成した開発の仕様書も、  
見せられる範囲のもので一部ですがこちらになります。    
<img width="100" alt="仕様書①" src="https://github.com/rencotsuki/RenardSample/blob/1ca690d2ef4a621c983476a042d08a7d522de817/DocumentSamples/%5Besa%5DSignageHADO%E4%BB%95%E6%A7%98%E6%9B%B8_01.png"> <img width="100" alt="仕様書②" src="https://github.com/rencotsuki/RenardSample/blob/1ca690d2ef4a621c983476a042d08a7d522de817/DocumentSamples/%5Besa%5DSignageHADO%E4%BB%95%E6%A7%98%E6%9B%B8_02.png"> <img width="100" alt="仕様書③" src="https://github.com/rencotsuki/RenardSample/blob/1ca690d2ef4a621c983476a042d08a7d522de817/DocumentSamples/%5Besa%5DSignageHADO%E4%BB%95%E6%A7%98%E6%9B%B8_03.png"> <img width="100" alt="仕様書④" src="https://github.com/rencotsuki/RenardSample/blob/1ca690d2ef4a621c983476a042d08a7d522de817/DocumentSamples/%5Besa%5DSignageHADO%E4%BB%95%E6%A7%98%E6%9B%B8_04.png">    

※実際動作させて確認するためには、  
ライセンス発行やAssetBundleの端末へのダウンロードなどの事前準備が必要です。  
上記の仕様書画像で４枚目の「機材構成・導入設置」にてまとめられている仕様と同じ内容なので、  
お手数ですが、添付画像の方をご確認の上お試しいただけると幸いです。  

## 補足
このリポジトリでは、１ファイルが大きい(10MB over)データを取り扱っているため  
**GitLFS** の設定がされています。  

StreamingAssetsにある「mediapipe」フォルダの中身はmediapipeを動かすために必要なものです。  
必ずビルドに含めることと、フォルダ名・パスを変更しないようにお願いします。  

ナビゲーターモデルに使用しているVRMファイルに関しては、  
Cloneしてきた際に展開出来ずエラーになることがあります。  
その時はUnityEditor上でVRMファイルを選択して、  
Reimportを行うことで改善されると思います。  
もし改善されない場合は、VRM関連のインポートが正しくされているか確認の上、  
再度VRMファイルを展開していただきますようお願いします。  
