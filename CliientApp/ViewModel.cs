using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CliientApp
{
    public class ViewModel
    {
        private ObservableCollection<MessegeInfo> _messeges = new ObservableCollection<MessegeInfo>();
        private ObservableCollection<MemberInfo> _members = new ObservableCollection<MemberInfo>();

        public IEnumerable<MessegeInfo> Messeges => _messeges;
        public void AddMsg(MessegeInfo info)
        {
            _messeges.Add(info);
        }

        public void DeleteMember(string login)
        {
            foreach (var m in _members)
            {
                if(m.Login == login)
                    _members.Remove(m); 
            };
        }

        public IEnumerable<MemberInfo> Members => _members;
        public void AddMember(MemberInfo info)
        {
            _members.Add(info);
        }

        public void DeleteAllMembers()
        {
            _members.Clear();
        }

        public void NotifyAboutNewChat(string login)
        {
            foreach (var m in _members)
            {
                if (m.Login == login)
                {
                    m.Post = "You have a new post";
                }
            }
        }

        public MemberInfo GetMember(string login)
        {
            foreach (var m in _members)
            {
                if (m.Login == login)
                    return m;
            }

            return new MemberInfo();
        }
    }
}