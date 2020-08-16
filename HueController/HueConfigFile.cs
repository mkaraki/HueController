using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace HueController
{
    class HueConfigFile
    {
        [YamlMember(Alias = "appkey")]
        public string ApplicationKey { get; set; }
    }
}
