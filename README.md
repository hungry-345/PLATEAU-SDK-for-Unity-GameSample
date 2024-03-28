# PLATEAU-SDK-for-Unity-GameSample

![Readme](https://github.com/Synesthesias/PLATEAU-SDK-for-Unity-GameSample/assets/96822472/55fe9949-d13c-4c88-b3c8-ace77c969345)

## 概要
[PLATEAU SDK for Unity](https://github.com/Project-PLATEAU/PLATEAU-SDK-for-Unity)のサンプルとして提供しているゲームアプリです。  

### ゲームルール
- 制限時間内にロボットに襲われている要救助者を救出するゲームです。
- 敵を避けながら要救助者と接触し、なるべく多くの人数をゴールに連れて行き救助することを目指します。
- ゴールの場所は紙飛行機を取得すると明らかになります。
- 敵は街を歩き回りますが、プレイヤーを見つけると襲ってきます。敵に触れるとゲームオーバーです。
- 要救助者も街を歩き回りますが、プレイヤーと接触すると同行者として付いてくるようになります。
- 同行者がいる状態でゴールとなる建物に接触すると救助となります。

操作方法はタイトルメニューの「遊び方」から閲覧できます。

### 他のサンプル
- [PLATEAU-SDK-for-Unity-GISSample](https://github.com/Project-PLATEAU/PLATEAU-SDK-for-Unity-GISSample)  
  PLATEAUの属性情報を視覚的に表示するサンプルです。

## 導入方法

サンプルゲームを遊んでみたい場合は下記の「ビルドアプリを入手したい場合」をご覧ください。  
Unityでプロジェクトを開きたい場合は下記の「プロジェクトを導入したい場合」をご覧ください。

### ビルドアプリを入手したい場合
- Windows向けのビルドアプリがReleaseページにあります。zipファイルをダウンロードして展開してアプリを起動します。

### プロジェクトを導入したい場合

#### 動作環境

- Unity 2021.3.30f1~

#### 導入方法

- 次のコマンドでgit lfsを導入してからリポジトリをクローンします。  
```
git lfs install
git clone https://github.com/Synesthesias/PLATEAU-SDK-for-Unity-GameSample.git
```
- Unityでプロジェクトを開きます。
- 別途[Starter Assets - ThirdPerson](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)を導入する必要があります。次の方法で導入します。
  - Unity Asset Storeから[Starter Assets - ThirdPerson](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)を開き、`Add to My Assets`ボタンを押します。  
  この際、Unityアカウントへのログインが必要となります。
  - Unityエディタのメニューバーから Window → Package Manager をクリックします。
  - Package Managerウィンドウ左上のドロップダウンメニューから`My Assets`を選択し、`Starter Assets - Third Person Character Controller`をクリックして`Install`または`Download`ボタンを押します。
  - ボタンが`Import`に変化するのでそれをクリックし、表示される`Import Unity Package`ウィンドウの`Import`ボタンを押します。
- シーンファイル `Assets/GameSample/Scenes/GameSample.unity`を開いてPlayすることで実行できます。


## ライセンス
ライセンスは[LICENSE.md](/LICENSE.md)を参照してください。

## 注意事項
- 本リポジトリの内容は予告なく変更・削除する可能性があります。
- 本リポジトリの利用により生じた損失及び損害等について、国土交通省はいかなる責任も負わないものとします。

