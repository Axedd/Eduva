namespace SchoolSystem.Models
{
    public class StudentRoleViewModel
    {
        public long StudentId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HasPassword { get; set; }
        public List<RoleViewModel> Roles { get; set; } // If you want to hold RoleViewModel
    }
}
