namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IRoomRepository Room { get; }
        IReservationRepository Reservation { get; }
        IImageRepository Image { get; }
        void Save();
    }
}
