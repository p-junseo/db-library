namespace LibraryWithAPI
{
    internal class LogueDAO
    {
        private static LogueDAO _instance;

        private const string _tableName = "logue_table";
        private readonly string[] _attribute = { "number", "time", "mode", "information", "action" };

        public static LogueDAO Get()
        {
            if (_instance == null)
            {
                _instance = new LogueDAO();
            }

            return _instance;
        }

        public int Insert(LogueInsertDTO logueInsert) // 로그 추가
        {
            Logue logue = new Logue(0, logueInsert.Time, logueInsert.Mode, logueInsert.Information, logueInsert.Action);

            return MySQLConnector.InsertInto(_tableName, logue.ToQueryArray());
        }

        public int Delete(LogueDeleteDTO logue) // 로그 삭제
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(logue.Number)}";

            return MySQLConnector.DeleteFromWhere(_tableName, conditionQuery);
        }

        public int DeleteAll() // 로그 리셋
        {
            return MySQLConnector.DeleteFromWhere(_tableName, "");
        }

        public List<LogueSelectResponseDTO> SelectAll() // 로그 조회
        {
            List<LogueSelectResponseDTO> logueList = new List<LogueSelectResponseDTO>();

            List<string[]> logueTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, "");

            for (int i = 0; i < logueTable.Count; i++)
            {
                Logue logue = new Logue(logueTable[i]);
                logueList.Add(new LogueSelectResponseDTO(logue.Number, logue.Time, logue.Mode, logue.Information, logue.Action));
            }

            return logueList;
        }

        public int SetAutoIncrement() // 로그 번호 재정렬
        {
            return MySQLConnector.SetUpdateSet(_tableName, _attribute[0]);
        }
    }
}
