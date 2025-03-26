# SignageHADO_AssetBundles

## 概要
SignageHADOプロジェクトにおけるAssetBundle素材は、サブモジュールとして別管理とする。  
将来的に、IP素材を取り扱う可能性を考慮して、プロジェクトのリポジトリとは切り離して関しておく。  

IPごとにブランチを切る運用でよいかなと思っている。（現状は）

## 構成
そのためリポジトリは[SignageHADO](https://github.com/meleap/SignageHADO)の  
Editor/AssetBundleResourcesフォルダが指定されています。  
各関連する機能は、それに合わせるような形で構成されいます。  

```
SignageHADO
└── Assets
    ├── Editor
    │   ├── AssetBundleResources ☆
    │　 │　  　├── 01_AssetScenes  }
    │　 │　  　├── 02_Timeline     }
    │　 │　  　├── 03_Objects      } ①
    │　 │　  　├── 04_Sounds       }
    │　 │　  　├── 05_SignageUI    }
    │　 │　  　│　　　： 
    │　 │　  　└── .CreateAssetBundles　② 
    │　 │        　　├── Android
    │　 │        　　├── iOS
    │　 │        　　├── OSX
    │　 │        　　└── Windows 
```
☆：  
この位置がこのリポジトリの展開先となる。  
①：  
AssetBundleにパッケージするためのファルダ構成。  
基本はこのフォルダ構成を手本に作成して、  
素材入稿のルールやフォルダ構成の追加変更はそれぞれで自由に対応する想定。  
②：  
AssetBundleビルドで出来た成果物が格納される場所。  
フォルダ名が「.xxx」(頭に「.(半角)」を付ける)としてUnityが認識しないようにしてある。  
※成果物はプラットフォームごとに出力され各フォルダに展開されます。  
  フォルダ名に｢~｣でなく｢.｣を使うのはSourceTreeのMac環境でフォルダ認識しなくなるため。  

## 利用想定として
SignageHADO側は基本的に「develop」ブランチをみて、  
AssetBundleを作る人がサブモジュールのブランチを切換えて、  
AssetBundleビルドを行い差分をこのリポジトリへプッシュする流れ。  
※SignageHADO側は「develop」ブランチ以外の取込みは行わない。
