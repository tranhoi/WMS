using Application.DTOs.Request.Account;

namespace WebUI.Pages.Account.Dto
{
    public class FormUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<CreateRoleRequestDTO> SelectedRoles { get; set; } = new List<CreateRoleRequestDTO>();
        public string Status { get; set; }
    }
}
