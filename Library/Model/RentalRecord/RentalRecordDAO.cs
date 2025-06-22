namespace LibraryWithAPI
{ 
    internal class RentalRecordDAO
    {
        private static RentalRecordDAO _instance;

        private const string _tableName = "rentalrecord_table";
        private readonly string[] _attribute = { "user_id", "book_id", "rentaltime", "returntime", "isreturned" };
        private readonly string[] _joinedAttribute = { "book_id", "title", "author", "publisher", "quantity", "price", "releasedate", "isbn",
            "information", "rentaltime", "returntime" };
        private const string _joinedTableName = "book_table";

        public static RentalRecordDAO Get()
        {
            if (_instance == null)
            {
                _instance = new RentalRecordDAO();
            }

            return _instance;
        }

        public int Insert(RentalRecordInsertDTO rentalRecordInsert) // 도서 대여
        {
            RentalRecord rentalRecord = new RentalRecord(rentalRecordInsert.UserID, rentalRecordInsert.BookID, rentalRecordInsert.RentalTime,
                rentalRecordInsert.ReturnTime, false);

            return MySQLConnector.InsertInto(_tableName, rentalRecord.ToQueryArray());
        }

        public int Update(RentalRecordUpdateDTO rentalRecordUpdate) // 도서 반납
        {
            string[] attribute = new string[2];
            attribute[0] = _attribute[3];
            attribute[1] = _attribute[4];

            string[] value = new string[2];
            value[0] = Converter.ToQuery(rentalRecordUpdate.ReturnTime);
            value[1] = "1";

            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(rentalRecordUpdate.UserID)}" 
                + $" AND {_attribute[1]} = {Converter.ToQuery(rentalRecordUpdate.BookID)}"
                + $" AND {_attribute[2]} = {Converter.ToQuery(rentalRecordUpdate.RentalTime)}";

            return MySQLConnector.UpdateSetWhere(_tableName, attribute, value, conditionQuery);
        }

        public int Delete(RentalRecordDeleteDTO rentalRecordDelete) // 회원 탈퇴 전 작업
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(rentalRecordDelete.UserID)}";

            return MySQLConnector.DeleteFromWhere(_tableName, conditionQuery);
        }

        public List<RentalRecordSelectResponseDTO> SelectIsNotReturned(RentalRecordSelectRequestBookDTO rentalRecordSelect) // 대여 내역
        {
            List<RentalRecordSelectResponseDTO> rentalRecordList = new List<RentalRecordSelectResponseDTO>();

            string conditionQuery = $"INNER JOIN {_joinedTableName} ON {_joinedTableName}.id = {_tableName}.book_id"
                + $" WHERE {_attribute[1]} = {Converter.ToQuery(rentalRecordSelect.BookID)}"
                + $" AND {_attribute[4]} = {0}";

            List<string[]> rentalRecordTable = MySQLConnector.SelectFromWhere(_tableName, _joinedAttribute, conditionQuery);

            for (int i = 0; i < rentalRecordTable.Count; i++)
            {
                rentalRecordList.Add(Convert(rentalRecordTable[i]));
            }

            return rentalRecordList;
        }

        public List<RentalRecordSelectResponseDTO> SelectIsNotReturned(RentalRecordSelectRequestDTO rentalRecordSelect) // 대여 내역
        {
            List<RentalRecordSelectResponseDTO> rentalRecordList = new List<RentalRecordSelectResponseDTO>();

            string conditionQuery = $"INNER JOIN {_joinedTableName} ON {_joinedTableName}.id = {_tableName}.book_id"
                + $" WHERE {_attribute[0]} = {Converter.ToQuery(rentalRecordSelect.UserID)}"
                + $" AND {_attribute[4]} = {0}";

            List<string[]> rentalRecordTable = MySQLConnector.SelectFromWhere(_tableName, _joinedAttribute, conditionQuery);

            for (int i = 0; i < rentalRecordTable.Count; i++)
            {
                rentalRecordList.Add(Convert(rentalRecordTable[i]));
            }

            return rentalRecordList;
        }

        public List<RentalRecordSelectResponseDTO> SelectIsReturned(RentalRecordSelectRequestDTO rentalRecordSelect) // 반납 내역
        {
            List<RentalRecordSelectResponseDTO> returnRecordList = new List<RentalRecordSelectResponseDTO>();

            string conditionQuery = $"INNER JOIN {_joinedTableName} ON {_joinedTableName}.id = {_tableName}.book_id"
                + $" WHERE {_attribute[0]} = {Converter.ToQuery(rentalRecordSelect.UserID)}"
                + $" AND {_attribute[4]} = {1}";

            List<string[]> returnRecordTable = MySQLConnector.SelectFromWhere(_tableName, _joinedAttribute, conditionQuery);

            for (int i = 0; i < returnRecordTable.Count; i++)
            {
                returnRecordList.Add(Convert(returnRecordTable[i]));
            }

            return returnRecordList;
        }

        private RentalRecordSelectResponseDTO Convert(string[] rentalRecord)
        {
            int bookID = Converter.ToInt(rentalRecord[0]);
            string title = rentalRecord[1];
            string author = rentalRecord[2];
            string publisher = rentalRecord[3];
            int quantity = Converter.ToInt(rentalRecord[4]);
            int price = Converter.ToInt(rentalRecord[5]);
            DateOnly releaseDate = Converter.ToDateOnly(rentalRecord[6]);
            string iSBN = rentalRecord[7];
            string information = rentalRecord[8];
            DateTime rentalTime = Converter.ToDateTime(rentalRecord[9]);
            DateTime returnTime = Converter.ToDateTime(rentalRecord[10]);

            return new RentalRecordSelectResponseDTO(bookID, title, author, publisher, quantity, price, releaseDate, iSBN, information, rentalTime, returnTime);
        }
    }
}