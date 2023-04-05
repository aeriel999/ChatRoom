using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Command_And_Members
{
    [AddINotifyPropertyChangedInterface]
    public class MemberInfo
    {
        private HashSet<(IPEndPoint, string)> _members = new HashSet<(IPEndPoint, string)>();//make identity

        public MemberInfo() { }

        public MemberInfo(string login)
        {
            Login = login;

            Initial = Login.ToCharArray().First().ToString();

            IsRequest = false;
        }
        public string Login { get; set; }
        public string Post { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRequest { get; set; }
        public string Initial { get; }
        public HashSet<(IPEndPoint, string)> Members => _members;

        public void AddNewMemberToChat(IPEndPoint ip, string login)
        {
            _members.Add((ip, login));
        }

        public void RemoveMemberFromChat(IPEndPoint memberIp)
        {
            foreach (var m in _members)
            {
                if (m.Item1 == memberIp)
                    _members.Remove(m);
            }
        }
    }
}
