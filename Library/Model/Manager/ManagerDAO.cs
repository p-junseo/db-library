namespace LibraryWithAPI
{
    internal class ManagerDAO
    {
        private static ManagerDAO _instance;

        private const string _tableName = "manager_table";
        private readonly string[] _attribute = { "id", "password" };

        public static ManagerDAO Get()
        {
            if (_instance == null)
            {
                _instance = new ManagerDAO();
            }

            return _instance;
        }

        public ManagerSelectResponseDTO Select(ManagerSelectRequestDTO managerSelect)
        {
            string conditionQuery = $"WHERE {_attribute[0]} = {Converter.ToQuery(managerSelect.ID)} AND {_attribute[1]} = {Converter.ToQuery(managerSelect.Password)}";

            List<string[]> managerTable = MySQLConnector.SelectFromWhere(_tableName, _attribute, conditionQuery);

            if (managerTable.Count == 0)
            {
                return null;
            }

            Manager manager = new Manager(managerTable[0]);

            return new ManagerSelectResponseDTO(manager.ID, manager.Password);
        }
    }
}