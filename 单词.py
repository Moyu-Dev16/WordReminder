import json
import requests


def get_word_definition(day):
    url = f"https://wordforest.cn/api/book/en/zh/word/details?id=3&day={day}&size=30&random=false&customWordData=true"
    response = requests.get(url)
    words = []
    if response.status_code == 200:
        data = response.json()
        words = data["words"]
    return words

if __name__ == "__main__":
    wordsdat = []
    for day in range(0, 45):
        print(f"Day {day} 单词列表:")
        words = get_word_definition(day)
        wordsdat += words

    with open("单词.json", "w", encoding="utf-8") as f:
        json.dump(wordsdat, f, ensure_ascii=False, indent=4)
