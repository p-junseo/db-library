namespace LibraryWithAPI
{
    internal class BookDAO
    {
        private static BookDAO _instance;

        private const string _tableName = "book_table";
        private readonly string[] _attribute = { "id", "title", "author", "publisher", "quantity", "price", "releasedate", "isbn", 
            "information", "isdeleted" };

        public static BookDAO Get()
        {
            if (_instance == null)
            {
                _instance = new BookDAO();
            }

            return _instance;
        }

        public int Insert(BookInsertDTO bookInsert) // 도서 추가
        {
            Book book = new Book(0, bookInsert.Title, bookInsert.Author, bookInsert.Publisher, bookInsert.Quantity, bookInsert.Price, 
                bookInsert.ReleaseDate, bookInsert.ISBN, bookInsert.Information, false);

            return MySQLConnector.InsertInto(_tableName, book.ToQueryArray());
        }

        public int UpdateIsDeleted(BookUpdateIsDeletedDTO bookUpdate) // 도서 삭제
        {
            string[] attribute = new string[2];
            attribute[0] = _attribute[4];
            attribute[0] = _attribute[9];

            string[] value = new string[2];
            value[0] = "0";
            value[1] = "1";

            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(bookUpdate.ID)}";

            return MySQLConnector.UpdateSetWhere(_tableName, attribute, value, conditionQuery);
        }

        public int UpdateQuantity(BookUpdateQuantityDTO bookUpdate) // 도서 대여 및 반납
        {
            string[] attribute = new string[1];
            attribute[0] = _attribute[4];

            string[] value = new string[1];
            value[0] = Converter.ToQuery(bookUpdate.Quantity);

            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(bookUpdate.ID)}";

            return MySQLConnector.UpdateSetWhere(_tableName, attribute, value, conditionQuery);
        }

        public int Update(BookUpdateDTO bookUpdate) // 도서 수정
        {
            string[] attribute = new string[8];
            for (int i = 0; i < 8; i++)
            {
                attribute[i] = _attribute[i + 1];
            }

            string[] value = new string[8];
            value[0] = Converter.ToQuery(bookUpdate.Title);
            value[1] = Converter.ToQuery(bookUpdate.Author);
            value[2] = Converter.ToQuery(bookUpdate.Publisher);
            value[3] = Converter.ToQuery(bookUpdate.Quantity);
            value[4] = Converter.ToQuery(bookUpdate.Price);
            value[5] = Converter.ToQuery(bookUpdate.ReleaseDate);
            value[6] = Converter.ToQuery(bookUpdate.ISBN);
            value[7] = Converter.ToQuery(bookUpdate.Information);

            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(bookUpdate.ID)}";

            return MySQLConnector.UpdateSetWhere(_tableName, attribute, value, conditionQuery);
        }

        public BookSelectResponseDTO Select(BookSelectRequestDTO bookSelect) // 도서 선택
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(bookSelect.ID)} AND {_attribute[9]} = {0}";

            List<string[]> bookTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            if (bookTable.Count == 0)
            {
                return null;
            }

            Book book = new Book(bookTable[0]);

            return new BookSelectResponseDTO(book.ID, book.Title, book.Author, book.Publisher, book.Quantity, book.Price, book.ReleaseDate, book.ISBN, book.Information);
        }

        public List<BookSelectResponseDTO> SelectSearch(BookSelectSearchRequestDTO bookSelect) // 도서 검색
        {
            List<BookSelectResponseDTO> bookList = new List<BookSelectResponseDTO>();

            string conditionQuery = $"WHERE {_attribute[1]} LIKE '%{bookSelect.Title}%'"
                + $" AND {_attribute[2]} LIKE '%{bookSelect.Author}%'"
                + $" AND {_attribute[3]} LIKE '%{bookSelect.Publisher}%' AND {_attribute[9]} = {0}";

            List<string[]> bookTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            for (int i = 0; i < bookTable.Count; i++)
            {
                Book book = new Book(bookTable[i]);
                bookList.Add(new BookSelectResponseDTO(book.ID, book.Title, book.Author, book.Publisher, book.Quantity, book.Price, book.ReleaseDate, book.ISBN, book.Information));
            }

            return bookList;
        }

        public List<BookSelectResponseDTO> SelectAll() // 전체 도서
        {
            List<BookSelectResponseDTO> bookList = new List<BookSelectResponseDTO>();

            string conditionQuery = $"WHERE {_attribute[9]} = {0}";

            List<string[]> bookTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            for (int i = 0; i < bookTable.Count; i++)
            {
                Book book = new Book(bookTable[i]);
                bookList.Add(new BookSelectResponseDTO(book.ID, book.Title, book.Author, book.Publisher, book.Quantity, book.Price, book.ReleaseDate, book.ISBN, book.Information));
            }

            return bookList;
        }
    }
}