namespace ManagingAccessService.Models.Menu
{
    public class ListMenu
    {
        public List<ItemMenu> Menu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employee", "Сотрудники"),
            new ItemMenu("Home", "Employees", "Сотрудникиb"),
            new ItemMenu("Authorization", "Login", "Войти"),
        };
        public List<ItemMenu> UserMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employee", "Сотрудники"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
        public List<ItemMenu> AdminMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employee", "Сотрудники"),
            new ItemMenu("Authorization", "Exit", "Выйти"),
        };
    }
}
