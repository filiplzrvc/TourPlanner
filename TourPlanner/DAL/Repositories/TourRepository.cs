namespace TourPlanner.DAL.Repositories
{
    public class TourRepository : Repository<Tour>
    {
        public TourRepository(TourPlannerContext context) : base(context)
        {
        }
    }
}
