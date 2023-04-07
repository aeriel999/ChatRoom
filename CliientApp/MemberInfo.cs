using PropertyChanged;
using System.Linq;
using System.Net;

namespace CliientApp
{
    [AddINotifyPropertyChangedInterface]
    public class MemberInfo
    {
        public string Login { get; set; }
        public string Post { get; set; }
        public bool IsSelected { get; set; }
        public string Initial { get; }
        public IPEndPoint Ip { get; set; }

        public MemberInfo() { }

        public MemberInfo(string login)
        {
            Login = login;

            Initial = Login.ToCharArray().First().ToString();
        }
    }
}