using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command_And_Members
{
    public static class Commands
    {
        public static string JOIN_CMD => "<JOIN>";
        public static string LEAVE_CMD = "<LEAVE>";
        public static string ADD_CMD = "<ADD.MEMBER>";
        public static string PRIVATE_CMD = "<PRIVATE>";//start private chat
        public static string NEW_MSG_CMD = "<NEW.MESSAGE>";//notify about new chat
        public static string OPEN_SENT_CHAT_CMD= "<OPEN.SEND.CHAT>";//open new chat for send
        public static string OPEN_REC_CHAT_CMD = "<OPEN.RECEIVED.CHAT>";//open new chat for received
    }
}
