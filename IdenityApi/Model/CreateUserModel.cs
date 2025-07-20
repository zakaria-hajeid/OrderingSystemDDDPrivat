using System.ComponentModel.DataAnnotations;

namespace IdenityApi.Model
{
    public record CreateUserModel
    {
        public string Name { get; set; }
        public  string password { get; set; }
        public  List<string> userRols { get; set; }
    }
}