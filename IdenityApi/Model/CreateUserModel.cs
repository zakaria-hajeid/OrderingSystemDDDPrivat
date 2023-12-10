using System.ComponentModel.DataAnnotations;

namespace IdenityApi.Model
{
    public record CreateUserModel
    {
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }

        public string CardHolderName { get; set; }

        public string Name { get; set; }
        public  string password { get; set; }
    }
}