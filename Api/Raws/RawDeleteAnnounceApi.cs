﻿using System;
using QQChannelFramework.Api.Types;

namespace QQChannelFramework.Api.Raws
{
    /// <summary>
    /// 源Api信息 - 删除子频道公告
    /// </summary>
    public struct RawDeleteAnnounceApi : Base.IRawApiInfo
    {
        public string Version => "1.0";

        public bool NeedParam => false;

        public string Url => "/channels/{channel_id}/announces/{message_id}";

        public MethodType Method => MethodType.DELETE;
    }
}
