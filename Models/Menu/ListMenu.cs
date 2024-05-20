namespace ManagingAccessService.Models.Menu
{
    public class ListMenu
    {
        public List<ItemMenu> HeaderMenu = new List<ItemMenu>()
        {
            new ItemMenu("Home","Index","Главная"),
            new ItemMenu("Home", "Employee", "Сотрудники"),
            new ItemMenu("Authorization", "Login", "Войти"),
        };
    }
}
