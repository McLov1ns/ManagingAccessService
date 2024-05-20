namespace ManagingAccessService.Models.Menu
{
    public class ItemMenu
    {
        public string Controller = "Home";
        public string Action { get; set; }
        public string Label { get; set; }
        public ItemMenu(string controller, string action, string label)
        {
            Controller = controller;
            Action = action;
            Label = label;
        }
    }
}
