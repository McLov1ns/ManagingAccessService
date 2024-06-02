namespace ManagingAccessService.Models.Menu
{
    public class ListMenu
    {
        public List<ItemMenu> Menu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employee", "Сотрудники"),
            new ItemMenu("Authorization", "Login", "Войти"),
        };
        public List<ItemMenu> UserMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
        public List<ItemMenu> AdminMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employees", "Сотрудникиb"),
            new ItemMenu("Home", "CreateUserAccount", "Сотрудникиb"),
            new ItemMenu("Organizations", "Index", "Организации"),
            new ItemMenu("Home", "UserAccount", "Аккаунты"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
    }
}
