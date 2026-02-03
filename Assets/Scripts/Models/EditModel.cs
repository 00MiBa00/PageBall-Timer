using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;
using Values;

namespace Models
{
    public class EditModel
    {
        private string _path;
        private int _index;
        private BookModel _changedBook;

        private const string SelectedBookIndexKey = "EditModel.SelectedBookIndex";

        public bool CanShowAds => !Subscription.HasSubscription();

        // ✅ Безопасные геттеры (если вдруг книги нет)
        public string BookName => _changedBook?.Name ?? string.Empty;
        public string BookDescription => _changedBook?.Description ?? string.Empty;

        public int BookRate => _changedBook != null ? _changedBook.Stars : 0;
        public List<int> GenreIndexes => _changedBook?.GenreIndexes ?? new List<int>();

        // ✅ Можно использовать в UI: если false — показывай "Нет книг, создайте первую"
        public bool HasBook { get; private set; }

        public EditModel()
        {
            // ✅ Должно совпадать с CreateSceneController и BooksInfo (JSON-файл)
            _path = Path.Combine(Application.persistentDataPath, "books.json");

            _index = PlayerPrefs.GetInt(SelectedBookIndexKey, 0);

            // ✅ создаём безопасную модель-буфер
            _changedBook = new BookModel();
            _changedBook.GenreIndexes = _changedBook.GenreIndexes ?? new List<int>();

            // ✅ пробуем загрузить реальную книгу
            var model = GetBookOrNull();

            if (model == null)
            {
                // книг нет или индекс кривой — не падаем
                HasBook = false;

                // оставляем пустую модель, чтобы UI не ловил null reference
                _changedBook.Name = string.Empty;
                _changedBook.Description = string.Empty;
                _changedBook.Stars = 0;
                _changedBook.GenreIndexes = new List<int>();

                return;
            }

            HasBook = true;

            // ✅ копируем данные
            _changedBook.Name = model.Name;
            _changedBook.Description = model.Description;
            _changedBook.Stars = model.Stars;
            _changedBook.GenreIndexes = model.GenreIndexes ?? new List<int>();
        }

        public void SetIndex(int index)
        {
            _index = index;
            PlayerPrefs.SetInt(SelectedBookIndexKey, index);
            PlayerPrefs.Save();
        }

        public void ChangeName(string name)
        {
            _changedBook.Name = name;
        }

        public void ChangeDescription(string description)
        {
            _changedBook.Description = description;
        }

        public void ChangeRate(int count)
        {
            _changedBook.Stars = count;
        }

        public bool? TryAddGenre(int value)
        {
            _changedBook.GenreIndexes ??= new List<int>();

            if (_changedBook.GenreIndexes.Count > 0)
            {
                int existingIndex = _changedBook.GenreIndexes.IndexOf(value);
                bool isInList = existingIndex != -1;

                if (isInList)
                {
                    SubtractGenre(existingIndex);
                    return false;
                }

                return AddGenre(value);
            }

            return AddGenre(value);
        }

        private bool? AddGenre(int value)
        {
            _changedBook.GenreIndexes ??= new List<int>();

            if (_changedBook.GenreIndexes.Count == 3)
                return null;

            _changedBook.GenreIndexes.Add(value);
            return true;
        }

        private void SubtractGenre(int index)
        {
            if (_changedBook.GenreIndexes == null) return;
            if (index < 0 || index >= _changedBook.GenreIndexes.Count) return;

            _changedBook.GenreIndexes.RemoveAt(index);
        }

        public async Task SaveBook()
        {
            // Если книг нет — сохранять нечего (или можно трактовать как Add)
            if (!HasBook)
                return;

            List<BookModel> models = GetBookModels();

            if (models.Count == 0)
            {
                HasBook = false;
                return;
            }

            // ✅ нормализуем индекс перед записью
            NormalizeIndex(models.Count);

            // ✅ гарантируем не-null жанры
            _changedBook.GenreIndexes ??= new List<int>();

            models[_index] = _changedBook;

            await BooksInfo.SaveBookModelAsync(models, _path);
        }

        public bool IsCanSave()
        {
            return HasBook &&
                   !string.IsNullOrEmpty(_changedBook.Name) &&
                   _changedBook.GenreIndexes != null &&
                   _changedBook.GenreIndexes.Count > 0 &&
                   _changedBook.Stars > 0;
        }

        // --- Helpers ---

        private BookModel GetBookOrNull()
        {
            List<BookModel> models = GetBookModels();

            if (models == null || models.Count == 0)
                return null;

            NormalizeIndex(models.Count);

            return models[_index];
        }

        private void NormalizeIndex(int count)
        {
            if (count <= 0)
            {
                _index = 0;
                PlayerPrefs.SetInt(SelectedBookIndexKey, _index);
                PlayerPrefs.Save();
                return;
            }

            if (_index < 0) _index = 0;
            if (_index >= count) _index = count - 1;

            // держим PlayerPrefs в актуальном состоянии
            PlayerPrefs.SetInt(SelectedBookIndexKey, _index);
            PlayerPrefs.Save();
        }

        private List<BookModel> GetBookModels()
        {
            // LoadBooks уже возвращает новый список, но оставим явную копию
            List<BookModel> loaded = BooksInfo.LoadBooks(_path) ?? new List<BookModel>();
            return new List<BookModel>(loaded);
        }
    }
}