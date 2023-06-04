using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.ViewModel.Attributes
{
    public class AffectedByAttribute : Attribute
    {
        /// <summary>
        /// List of Affecting Properties
        /// </summary>
        public string[] AffectedBy { get; }

        public AffectedByAttribute(params string[] affectedBy)
        {
            AffectedBy = affectedBy;
        }
    }
}
