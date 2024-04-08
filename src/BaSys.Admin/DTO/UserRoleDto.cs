namespace BaSys.Admin.DTO
{
    public sealed class UserRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsChecked { get; set; }

        public override string ToString()
        {
            return $"{IsChecked}/{Name}";
        }
    }
}
