namespace MvcBlanket.Security.Models
{
    class RoleInformation
    {
        public string RoleName { get; set; }
        public bool Access { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as RoleInformation;
            if (other == null) return false;
            return RoleName == other.RoleName;
        }

        public override int GetHashCode()
        {
            return RoleName.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", RoleName,
                Access ? "Granted" : "Denied");
        }
    }
}
