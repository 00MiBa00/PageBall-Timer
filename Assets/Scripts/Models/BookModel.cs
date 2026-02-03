using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class BookModel
    {
        public string Name;
        public string Description;
        public int Stars;
        public List<int> GenreIndexes = new List<int>();
    }
}