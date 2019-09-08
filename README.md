# ラボinしました

## これは何

研究室の来たことを知らせるために、デバイスのボタンを押すと slack の特定のチャンネルに「ラボinしました！」のメッセージが投稿されます。

## 構成と原理

- デバイス側

C++, Arduino (Arduino Nano)

- ホストPC側

C#

windowsでのみ動作確認

Device-HostPC 間はシリアル通信でやり取りをします。デバイスのボタンが押されると PC にボタンが押されたメッセージを送信、PC はメッセージを受信すると Slack へ投稿します。

要するに、たいしたことは何もしていません。

## 処理

Device-HostPC 接続時処理

PC はどのシリアルポートにデバイスが接続されたのかは分からないため、繋がれている全てのシリアルポートに Ping を送信し、正しい応答が返ってきたポートをこのデバイスとして認識します。

```mermaid
sequenceDiagram
Device ->> HostPC : [USB Connection]
HostPC ->> Device : Ping ("MSGPing")
Device ->> HostPC : Ping "MSGPing"
HostPC ->> Device : Connected ("MSGCNCT")
Note over Device : Red LED Lights up
```

ボタンが押された時

```mermaid
sequenceDiagram
Device ->> HostPC : "I got arrived"
HostPC ->> Slack : Post message
Slack ->> HostPC : Responce
HostPC ->> Device : "MSGOK"
Note over Device : Greeen LEDs Blink
```

## Slackへの投稿

Slack への投稿は、あらかじめ取得した Token を用いて SlackAPI で投稿しています。

Token や Slack チャンネル名などは ```config.xml``` に外部ファイルとして置いています。個人情報を含むためリポジトリには空の config ファイルを置いています。

## ハードウェア

### 回路図

[略]

### マイコンのピンアサイン

| 素子         | Arduino Nano pin | I/O |
| ----------- | ---------------- | --- |
| LED Green0  | Digital 2        | out |
| LED Green1  | Digital 3        | out |
| LED Green2  | Digital 4        | out |
| LED Red     | Digital 5        | out |
| Main Switch | Digital 6        | in  |
| Mode Switch | Digital 7        | in  |



## 制作者

ikorin24 ([github](https://github.com/ikorin24))
