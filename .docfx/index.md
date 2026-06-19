---
_layout: landing
title: UdonRadioCommunications-Redux
---

# UdonRadioCommunications-Redux

VRChat Udon worlds のための無線コミュニケーションシステムです。

[Getting Started](docs/intro.md) / [Installation](docs/install.md) / [API Reference](xref:UdonRadioCommunicationRedux)

<img class="landing-logo" src="images/VAJ_Logo_B.svg" alt="Virtual Aviation Japan Logo">

## 概要

UdonRadioCommunications-Redux は、VRChat における UdonSharp (U#) で動作する、高度な音声コミュニケーションシステムです。

[UdonRadioCommunications](https://github.com/esnya/UdonRadioCommunications) の機能改善版として開発されました。

## 特徴

### 柔軟な送受信システム

送受信モジュールを抽象化することで、様々な無線システムに対応できる柔軟性を備えています。複数の送受信モジュールを同時に管理でき、複雑な通信環境を構築できます。

### 高度な拡声機能

特定の人物に対して、拡声・標準・サイレントを切り替えられます。ゾーン情報を利用した拡声状態の自動制御、ゾーンを跨いだ状態変更、機内音声によるゾーン内の拡声状態上書きにも対応します。

### 音声 On/Off 機能

特定の音声の On/Off を任意に設定できます。たとえば ATIS/VOR のような特定の GameObject の On/Off を、設定に基づいて制御できます。

## Documentation

- [Introduction](docs/intro.md)
- [Installation](docs/install.md)
- [Changelog](docs/changelog.md)
- [API Reference](xref:UdonRadioCommunicationRedux)
