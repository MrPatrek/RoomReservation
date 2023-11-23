namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IRoomRepository Room { get; }
        IReservationRepository Reservation { get; }
        void Save();
    }
}
