using System.Runtime.Serialization;

namespace QT.Authentication
{
    [DataContract]
    public class VotyUserData
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        //public List<Role> Roles { get; set; }
        public string Role { get; set; }
        
        public VotyUserData()
        {
            Active = true;
            //Roles = new List<Role>();
        }
    }
}