namespace LicenseManagement.EndUser.Models
{
    /// <summary>
    /// this is used to updated a license and tell the system that someone has paid for the product. Hence the receipt code field
    /// </summary>
    public class PatchLicenseModel {
        /// <summary>
        /// id of the license that needs to be updated. Id refers to database id
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// this is in effect a random but unique identifier that allows the system to tell a paid and trial license apart.
        /// </summary>
        ///<remarks>if set to null will result in a computers (it's receipt) unregister from a license, this is useful when a user un-installs the software from a computer and we want to make sure computer is not linked to a specific receipt</remarks>
        public string Code { get; set; }
    }

}