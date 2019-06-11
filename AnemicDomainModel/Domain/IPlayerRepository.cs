namespace AnemicDomainModel.Domain
{
    public interface IPlayerRepository
    {
        Player GetCurrentPlayer(int gameId);
    }
}
