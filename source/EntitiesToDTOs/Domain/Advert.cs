using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an advert.
    /// </summary>
    internal class Advert
    {
        /// <summary>
        /// Advert ID.
        /// </summary>
        public int AdvertID { get; set; }

        /// <summary>
        /// Image of the advert.
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Link URL associated with this advert.
        /// </summary>
        public string LinkURL { get; set; }

    }
}