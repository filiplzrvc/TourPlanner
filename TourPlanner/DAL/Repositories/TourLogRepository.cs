namespace TourPlanner.DAL.Repositories
{
    public class TourLogRepository : Repository<TourLog>
    {
        public TourLogRepository(TourPlannerContext context) : base(context)
        {
        }
    }
}
