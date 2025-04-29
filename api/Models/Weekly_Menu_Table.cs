using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace Api.Models
{
        public class Weekly_Menu_Table
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTime created_at { get; set; }

        public List<Weekly_Menu> Weekly_Menus { get; set; }
    }
}
