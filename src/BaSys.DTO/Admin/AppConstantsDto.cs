using BaSys.DAL.Models.Admin;

namespace BaSys.DTO.Admin
{
    public sealed class AppConstantsDto
    {
        public Guid Uid { get; set; }
        public Guid DataBaseUid { get; set; }
        public string ApplicationTitle { get; set; } = string.Empty;
        public bool UseWorkflowsScheduler { get; set; }
        public string AppVersion { get; set; } = string.Empty;

        public AppConstantsDto()
        {
        }

        public AppConstantsDto(AppConstants model)
        {
            Uid = model.Uid;
            DataBaseUid = model.DataBaseUid;
            ApplicationTitle = model.ApplicationTitle;
            UseWorkflowsScheduler = model.UseWorkflowsScheduler;
        }

        public AppConstants ToModel()
        {
            return new AppConstants
            {
                Uid = Uid,
                DataBaseUid = DataBaseUid,
                ApplicationTitle = ApplicationTitle,
                UseWorkflowsScheduler = UseWorkflowsScheduler
            };
        }
    }
}
