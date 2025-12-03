namespace Hymma.Lm.EndUser
{
    /// <summary>
    /// indicates when this context is being called, during install that we have access to admin and internet or during launch
    /// </summary>
    public enum HandlerStrategy
    {
        /// <summary>
        /// when this context is called during application install 
        /// </summary>
        Install,

        /// <summary>
        /// when this context is called during application launch
        /// </summary>
        Launch,

        /// <summary>
        /// when this context is called during application uninstallation
        /// </summary>
        UnInstall,

    }
}
