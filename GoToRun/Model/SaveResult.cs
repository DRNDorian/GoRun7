using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToRun.Model
{
    public class SaveResult
    {
        private IEnumerable<string> _errorMessages = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the error messages, if any, produced by the validation operation.
        /// </summary>
        public IEnumerable<string> ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether validation was successful.
        /// </summary>
        public bool SaveSuccessful { get; set; }
    }
}
