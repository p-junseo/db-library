namespace LibraryWithAPI
{
    internal class RequestedBookDAO
    {
        private static RequestedBookDAO _instance;

        private const string _tableName = "requestedbook_table";
        private readonly string[] _attribute = { "id", "title", "author", "publisher", "price", "releasedate", "isbn", "information" };

        public static RequestedBookDAO Get()
        {
            if (_instance == null)
            {
                _instance = new RequestedBookDAO();
            }

            return _instance;
        }

        public int Insert(RequestedBookInsertDTO requestedBookInsert) // 도서 요청
        {
            RequestedBook requestedBook = new RequestedBook(0, requestedBookInsert.Title, requestedBookInsert.Author, requestedBookInsert.Publisher, 
                requestedBookInsert.Price, requestedBookInsert.ReleaseDate, requestedBookInsert.ISBN, requestedBookInsert.Information);

            return MySQLConnector.InsertInto(_tableName, requestedBook.ToQueryArray());
        }

        public int Delete(RequestedBookDeleteDTO requestedBookDelete) // 도서 추가
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(requestedBookDelete.ID)}";

            return MySQLConnector.DeleteFromWhere(_tableName, conditionQuery);
        }

        public RequestedBookSelectResponseDTO Select(RequestedBookSelectRequestDTO requestedBookSelect) // 도서 추가
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(requestedBookSelect.ID)}";

            List<string[]> requestedBookTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            if (requestedBookTable.Count == 0)
            {
                return null;
            }

            RequestedBook requestedBook = new RequestedBook(requestedBookTable[0]);

            return new RequestedBookSelectResponseDTO(requestedBook.ID, requestedBook.Title, requestedBook.Author, requestedBook.Publisher, 
                requestedBook.Price, requestedBook.ReleaseDate, requestedBook.ISBN, requestedBook.Information);
        }

        public List<RequestedBookSelectResponseDTO> SelectAll() // 요청 도서 내역
        {
            List<RequestedBookSelectResponseDTO> requestedBookList = new List<RequestedBookSelectResponseDTO>();

            List<string[]> userTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, "");

            for (int i = 0; i < userTable.Count; i++)
            {
                RequestedBook requestedBook = new RequestedBook(userTable[i]);
                requestedBookList.Add(new RequestedBookSelectResponseDTO(requestedBook.ID, requestedBook.Title, requestedBook.Author, requestedBook.Publisher, 
                    requestedBook.Price, requestedBook.ReleaseDate, requestedBook.ISBN, requestedBook.Information));
            }

            return requestedBookList;
        }

        public int SetAutoIncrement() // 요청 도서 아이디 재정렬
        {
            return MySQLConnector.SetUpdateSet(_tableName, _attribute[0]);
        }
    }
}
