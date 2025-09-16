namespace HiringPipelineCore.Entities
{
    /// <summary>
    /// Types of audit log entries
    /// </summary>
    public enum AuditLogType
    {
        /// <summary>
        /// Direct user action (button clicks, form submissions, navigation)
        /// </summary>
        UserAction = 0,

        /// <summary>
        /// System operation (automatic processes, scheduled tasks)
        /// </summary>
        SystemOperation = 1,

        /// <summary>
        /// Background data fetch (API calls for loading page data)
        /// </summary>
        BackgroundFetch = 2,

        /// <summary>
        /// Authentication related actions
        /// </summary>
        Authentication = 3,

        /// <summary>
        /// Database management operations
        /// </summary>
        DatabaseManagement = 4
    }
}
