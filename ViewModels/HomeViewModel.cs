using ManagingAccessService.Models.DbModels;
using System.Net.Sockets;

namespace ManagingAccessService.ViewModels
{
    public class HomeViewModel
    {
       public UserAccount Account { get; set; }
       public Employee Employee { get; set; }
    }
}
