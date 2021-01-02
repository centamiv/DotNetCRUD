using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCrud.Utils
{
    class FieldType
    {
        // On Entity Framework
        public string ClassName { get; set; }
        public string ClassType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }
        
        // On DotNetCrud
        public object DefaultValue { get; set; }
        public bool ListVisible { get; set; }
        public bool InsertVisible { get; set; }
        public bool EditVisible { get; set; }
        public bool EditEnabled { get; set; }
        
        // On Database
        public string DbName { get; set; }
        public string DbType { get; set; }

        // Relation 1-N
        public string RelatedDbTable { get; set; }
        public string RelatedDbField { get; set; }
        public string RelatedDbDisplayField { get; set; }
    }
}
