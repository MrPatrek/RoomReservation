namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IRoomRepository Room { get; }
        IReservationRepository Reservation { get; }
        IImageRepository Image { get; }
        IUserRepository User { get; }
        Task SaveAsync();
    }
}
