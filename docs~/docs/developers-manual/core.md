# Runtime/core

Runtime/coreは、プレイヤー音声の設定を抽象化し、複数のプレイヤー音声設定変更手段を共存するための仕組みを提供します。

## Overview

Runtime/coreにおける処理の流れを、以下に示します。


## VoiceProtocol

Runtime/coreの機能を用い、音声設定を変更する機能を作成する際は、`VoiceProtocol`を継承したコンポーネントを作成してください。
`VoiceProtocol`は、以下を提供します。

- Runtime/core に対し、音声設定の優先度を指定
- Runtime/coreに対し、プレイヤーが新規登録された際のイベント関数呼び出し

## PlayerVoice

`PlayerVoice`は
