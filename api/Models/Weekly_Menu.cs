using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace Api.Models
{
   public class Weekly_Menu
{
    public int id { get; set; }
    public int menu_id { get; set; }
    public string day_of_week { get; set; }

    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }

    public int menu_table_id { get; set; }
    public Weekly_Menu_Table Weekly_Menu_Table { get; set; }

    public Menu Menu { get; set; }
}

}
