using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using Models;

namespace Values
{
    public static class BooksInfo
    {
        private const string CountKey = "Books.Count";

        public static bool HaveBooks => Count > 0;
        public static string Status => Count >= 20 ? "Guru" : "Beginner";

        [Serializable]
        private class BookListWrapper
        {
            public List<BookModel> Books = new List<BookModel>();
        }

        private static int Count
        {
            get => PlayerPrefs.GetInt(CountKey, 0);
            set
            {
                PlayerPrefs.SetInt(CountKey, value);

                switch (value)
                {
                    case 1:
                        RewardInfo.SetFirstBookReward();
                        break;
                    case 2:
                        RewardInfo.SetReadingTwoBooksReward();
                        break;
                    case 5:
                        RewardInfo.SetReadingFiveBooksReward();
                        break;
                    case 20:
                        RewardInfo.SetNewStatusReward();
                        break;
                    case 25:
                        RewardInfo.SetTopReaderReward();
                        break;
                }

                PlayerPrefs.Save();
            }
        }

        public static void SaveCount(int count) => Count = count;

        public static Task SaveBookModelAsync(List<BookModel> books, string path)
        {
            foreach (var book in books)
            {
                bool containReward = book.GenreIndexes != null &&
                                     book.GenreIndexes.Contains(5) &&
                                     book.GenreIndexes.Contains(6);

                if (containReward)
                {
                    RewardInfo.SetFanHorrorAndFantasyReward();
                    break;
                }
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            SaveBooksSync(books, path);
            return Task.CompletedTask;
#else
            return Task.Run(() => SaveBooksSync(books, path));
#endif
        }

        private static void SaveBooksSync(List<BookModel> books, string path)
        {
            // защита от null
            books ??= new List<BookModel>();

            try
            {
                var wrapper = new BookListWrapper { Books = books };
                string json = JsonUtility.ToJson(wrapper);

                // гарантируем что директория существует (на всякий)
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving books: " + e);
                throw;
            }
        }

        public static List<BookModel> LoadBooks(string path)
        {
            if (!File.Exists(path))
                return new List<BookModel>();

            try
            {
                string json = File.ReadAllText(path);

                // ✅ КЛЮЧЕВО: если документ пустой — считаем что книг нет
                if (string.IsNullOrWhiteSpace(json))
                    return new List<BookModel>();

                var wrapper = JsonUtility.FromJson<BookListWrapper>(json);

                // если файл не нашего формата или битый, wrapper может быть null
                return wrapper?.Books ?? new List<BookModel>();
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading books: " + e);

                // ✅ Чтобы больше не падать на битом файле, можно его сбросить
                // (особенно актуально при миграции Binary->JSON)
                try
                {
                    File.Delete(path);
                }
                catch { /* ignore */ }

                return new List<BookModel>();
            }
        }

        // ✅ Удобно: миграционный ресет, можно вызвать один раз при старте, если надо
        public static void ResetBooks(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch { /* ignore */ }

            SaveCount(0);
        }
    }
}