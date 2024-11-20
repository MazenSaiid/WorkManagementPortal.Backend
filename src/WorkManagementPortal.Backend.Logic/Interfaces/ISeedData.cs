namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface ISeedData
    {
        Task SeedRoles(IServiceProvider serviceProvider);
        Task SeedShifts(IServiceProvider serviceProvider);
    }
}