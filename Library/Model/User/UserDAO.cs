namespace LibraryWithAPI
{
    internal class UserDAO
    {
        private static UserDAO _instance;

        private const string _tableName = "user_table";
        private readonly string[] _attribute = { "id", "password", "name", "age", "phonenumber", "address" };

        public static UserDAO Get()
        {
            if (_instance == null)
            {
                _instance = new UserDAO();
            }

            return _instance;
        }

        public int Insert(UserInsertDTO userInsert) // 회원 가입
        {
            User user = new User(userInsert.ID, userInsert.Password, userInsert.Name, userInsert.Age, userInsert.PhoneNumber, userInsert.Address);

            return MySQLConnector.InsertInto(_tableName, user.ToQueryArray());
        }

        public int Update(UserUpdateDTO userUpdate) // 정보 수정
        {
            string[] attribute = new string[5];
            for (int i = 0; i < 5; i++)
            {
                attribute[i] = _attribute[i + 1];
            }

            string[] value = new string[5];
            value[0] = Converter.ToQuery(userUpdate.Password);
            value[1] = Converter.ToQuery(userUpdate.Name);
            value[2] = Converter.ToQuery(userUpdate.Age);
            value[3] = Converter.ToQuery(userUpdate.PhoneNumber);
            value[4] = Converter.ToQuery(userUpdate.Address);

            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(userUpdate.ID)}";

            return MySQLConnector.UpdateSetWhere(_tableName, attribute, value, conditionQuery);
        }

        public int Delete(UserDeleteDTO userDelete) // 계정 삭제
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(userDelete.ID)}";

            return MySQLConnector.DeleteFromWhere(_tableName, conditionQuery);
        }

        public UserSelectResponseDTO Select(UserSelectRequestDTO userSelect) // 기존 정보, 존재하는 회원
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(userSelect.ID)}";

            List<string[]> userTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            if (userTable.Count == 0)
            {
                return null;
            }

            User user = new User(userTable[0]);

            return new UserSelectResponseDTO(user.ID, user.Name, user.Age, user.PhoneNumber, user.Address);
        }

        public UserSelectResponseDTO SelectIDPassword(UserSelectIDPasswordRequestDTO userSelect) // 로그인
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(userSelect.ID)} AND {_attribute[1]} = {Converter.ToQuery(userSelect.Password)}";

            List<string[]> userTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            if (userTable.Count == 0)
            {
                return null;
            }

            User user = new User(userTable[0]);

            return new UserSelectResponseDTO(user.ID, user.Name, user.Age, user.PhoneNumber, user.Address);
        }

        public List<UserSelectResponseDTO> SelectAll() // 유저 목록
        {
            List<UserSelectResponseDTO> userList = new List<UserSelectResponseDTO>();

            List<string[]> userTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, "");

            for (int i = 0; i < userTable.Count; i++)
            {
                User user = new User(userTable[i]);
                userList.Add(new UserSelectResponseDTO(user.ID, user.Name, user.Age, user.PhoneNumber, user.Address));
            }

            return userList;
        }
    }
}
