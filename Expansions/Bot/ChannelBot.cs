﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using QQChannelFramework.Api;
using QQChannelFramework.Models;
using QQChannelFramework.Models.Types;
using QQChannelFramework.Models.WsModels;
using QQChannelFramework.WS;

namespace QQChannelFramework.Expansions.Bot;

/// <summary>
/// QQ频道机器人
/// </summary>
public sealed partial class ChannelBot : FunctionWebSocket
{
    private string _url;

    public ChannelBot(OpenApiAccessInfo openApiAccessInfo) : base(openApiAccessInfo)
    {
        ReceivedAtMessage += (message) =>
        {
            var realContent = message.Content.Trim();

            if(message.Mentions.Count > 0)
            {
                foreach (var user in message.Mentions)
                {
                    realContent = realContent.Replace($"<@!{user.Id}> ", string.Empty);
                }
            }

            var rawData = realContent.Split(" ");

            CommandInfo commandInfo = new();
            commandInfo.Key = rawData[0];
            commandInfo.Param = rawData.ToList();
            commandInfo.Param.RemoveAt(0);
            commandInfo.MessageId = message.Id;
            commandInfo.Sender = message.Author;

            InvokeCommand(commandInfo);
        };

        OnError += (ex) =>
        {
            if(ex is System.Net.WebSockets.WebSocketException)
            {
                Resume();
            }
        };
    }

    /// <summary>
    /// 机器人上线
    /// </summary>
    public async void OnlineAsync()
    {
        QQChannelApi qQChannelApi = new(_openApiAccessInfo);
        _url = await qQChannelApi.UseBotIdentity().GetWebSocketApi().GetUrlAsync();

        Connect(_url);
    }

    /// <summary>
    /// 执行重连 (如果处于连接状态将会主动断开后连接)
    /// </summary>
    public void Resume()
    {
        if (webSocket.State != System.Net.WebSockets.WebSocketState.Open)
        {
            Connect(_url);
            OnConnected += ResumeAction;
        }
    }

    private void ResumeAction()
    {
        Load load = new();
        load.op = (int)OpCode.Resume;
        load.d = new Dictionary<string, object>()
        {
            {"token",_openApiAccessInfo.BotToken},
            {"session_id",_sessionInfo.SessionId },
            {"seq",1337 }
        };

        SendAsync(JsonConvert.SerializeObject(load));

        OnConnected -= ResumeAction;
    }

}