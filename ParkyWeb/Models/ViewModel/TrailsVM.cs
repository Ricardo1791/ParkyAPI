using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Models.ViewModel
{
    public class TrailsVM
    {
        public Trail Trails { get; set; }
        public IEnumerable<SelectListItem> NationalParkList { get; set; }
    }
}
