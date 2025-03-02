namespace ConsultaBeneficios.API.Interfaces
{
    public interface IMessageServices
    {
        void Publish(string queue, byte[] message);
    }
}
