---
title: 開発環境セットアップ
sidebar_position: 4
---

# 開発環境セットアップ

## 必要な環境

- Unity 2022.3 LTS
- VRChat SDK3 - Worlds
- UdonSharp
- SaccFlightAndVehicles (SaccFlight統合を使用する場合)

## パッケージ構成

```
Packages/
├── jp.virtualaviation.udon-radio-communications-redux/    # メインパッケージ
│   ├── Runtime/
│   │   ├── core/           # コアシステム
│   │   ├── channel/        # チャンネルシステム
│   │   ├── zone/           # ゾーンシステム
│   │   └── utilities/      # ユーティリティ
│   ├── Editor/             # エディタ拡張
│   ├── Sample/             # サンプルシーン
│   └── Tests/              # テスト
└── jp.virtualaviation.udon-radio-communications-redux-sf/ # SaccFlight統合
    └── Runtime/
        ├── DFUNC/          # SaccFlight DFUNC コンポーネント
        └── SFEXT_URC_VHF.cs # SaccFlight拡張
```