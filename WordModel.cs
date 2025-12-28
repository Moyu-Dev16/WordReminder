using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordReminder
{
    // 对应 JSON 根节点
    public class WordRoot
    {
        [JsonProperty("data")]
        public List<WordItem> Data { get; set; }
    }

    // 对应每一个单词对象
    public class WordItem
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string UsPhonetic { get; set; } // 美式音标

        // 解析复杂的 translations 结构
        public Translations Translations { get; set; }

        // 获取一个简单的中文释义用于显示
        public string SimpleDefinition
        {
            get
            {
                if (Translations?.Meanings == null) return "暂无释义";
                List<string> defs = new List<string>();
                foreach (var key in Translations.Meanings.Keys)
                {
                    // 把词性（如 N）和释义拼接，例如：N: 革命, 旋转
                    string meanings = string.Join(", ", Translations.Meanings[key]);
                    defs.Add($"{key}. {meanings}");
                }
                return string.Join("\n", defs); // 换行显示不同词性
            }
        }
    }

    public class Translations
    {
        // 因为 JSON 里 meanings 下面的键是不固定的（N, V, ADJ...），使用 Dictionary
        public Dictionary<string, List<string>> Meanings { get; set; }
    }
}
