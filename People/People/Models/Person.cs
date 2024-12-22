using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace People.Models
{
    [SQLite.Table("People")]
    public class Person
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100),Unique]
        public string Name { get; set; }
    }
}
