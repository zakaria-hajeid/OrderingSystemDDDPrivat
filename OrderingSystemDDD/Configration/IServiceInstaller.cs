namespace OrderingSystemDDD.Configration
{
    public interface IServiceInstaller
    {
        void Instal(IServiceCollection services, IConfiguration configuration);
    }
}
