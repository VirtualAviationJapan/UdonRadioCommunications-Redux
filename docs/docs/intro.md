---
sidebar_position: 1
---

# Introduction

## 概要

UdonRadioCommunications-Redux は、VRChat における UdonSharp (U#) で動作する、高度な音声コミュニケーションシステムです。

## 背景

従来型の UdonRadioCommunication には、以下のような課題がありました。

- 更新の停滞とバグの存在
- 複雑なシステムと依存関係による運用性の低さ
- 拡張性の不足

UdonRadioCommunications-Redux は、これらの課題を解決し、より柔軟で拡張性の高い無線システムを提供するために開発されました。

## 特徴

UdonRadioCommunications-Redux は、以下の主要な機能を備えています。

### 柔軟な送受信システム
- 送受信モジュールの抽象化により、様々な無線システムに対応可能。
- 複数の送受信モジュールを同時に管理し、複雑な通信環境を構築可能。

### 高度な拡声機能
- 特定の人物に対する拡声/標準/サイレントの切り替え機能。
- ゾーン情報を利用した拡声状態の自動制御。
- ゾーンを跨いでの拡声状態の変更や、機内音声による拡声状態のオーバーライド機能。

### 音声 On/Off 機能
- 特定音声の On/Off を任意に設定可能。
- 設定に基づいた GameObject の On/Off 制御 (旧 ATIS/VOR 機能の代替)。

## 今後の展望

UdonRadioCommunications-Redux は、VRChat における様々なコミュニケーションニーズに対応できる、強力なツールとなることを目指しています。