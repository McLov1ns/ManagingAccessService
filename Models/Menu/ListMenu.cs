namespace ManagingAccessService.Models.Menu
{
    public class ListMenu
    {
        public List<ItemMenu> Menu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Employees", "Index", "Сотрудники"),
            new ItemMenu("Authorization", "Login", "Войти"),
        };
        public List<ItemMenu> UserMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Organizations", "Index", "Организации"),
            new ItemMenu("Employees", "Index", "Сотрудники"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
        public List<ItemMenu> AdminMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Organizations", "Index", "Организации"),
            new ItemMenu("Employees", "Index", "Сотрудники"),
            new ItemMenu("UserAccounts", "Index", "Акаунты"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
    }
}
