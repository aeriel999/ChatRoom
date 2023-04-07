using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Command_And_Members
{
    public static class Commands
    {
        public static string JOIN_CMD => "<JOIN>";
        public static string LEAVE_CMD = "<LEAVE>";
        public static string PRIVATE_CMD = "<PRIVATE.CHAT>";//start private chat
        

        private static HashSet<(IPEndPoint, string)> _members = new HashSet<(IPEndPoint, string)>();//make identity

        public static HashSet<(IPEndPoint, string)> Members => _members;

        public static void AddNewMemberToChat(IPEndPoint ip, string login)
        {
            _members.Add((ip, login));
        }

        public static void RemoveMemberFromChat(IPEndPoint memberIp)
        {
            foreach (var m in _members)
            {
                if (m.Item1.ToString() == memberIp.ToString())
                    _members.Remove(m);
            }
        }
    }

}

