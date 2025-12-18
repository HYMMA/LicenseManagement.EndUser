using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LicenseManagement.EndUser.Models
{
    public class DbModelWithDate  
    {
        DateTime? _updated;
        DateTime? _created;
        /// <summary>
        /// the date this object was first created in db
        /// </summary>
        public virtual DateTime? Created
        {
            get => _created; set
            {
                if (_created != value)
                {
                    _created = value;
                }
            }
        }

        /// <summary>
        /// represents the date this row was updated in db
        /// </summary>
        public virtual DateTime? Updated
        {
            get => _updated; set
            {
                if (_updated != value)
                {
                    _updated = value;
                }
            }
        }
    }
}
