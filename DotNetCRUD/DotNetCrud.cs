using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using DotNetCrud.Utils;
using DotNetCrud.Render;
using Microsoft.EntityFrameworkCore.Internal;

namespace DotNetCrud
{
    public class DotNetCrud<T>
        where T : class, new()
    {
        private IQueryable<T> _table;
        // private readonly IQueryable<T> _query;
        private readonly List<string> _fields = new List<string>();
        private string _idField;
        private readonly HttpRequest _request;
        private readonly HttpResponse _response;

        //public DotNetCrud<T> Relation<T1>() where T1 : class, new()
        //{
        //    return this;
        //}

        public DotNetCrud<T> Query(Func<IQueryable<T>, IQueryable<T>> query)
        {
            _table = query(_table);
            return this;
        }

        //public object Relation<T1>(string outerKey, string innerKey, string v3) 
        //    where T1: class, new()
        //{
        //    _table = _table.Join(
        //        _db.Set<T1>(),
        //        o => EF.Property<object>(o, outerKey),
        //        i => EF.Property<object>(i, innerKey),
        //        (o, i) => o);
        //    return this;
        //}

        private readonly HttpContext _context;
        private readonly DbContext _db;
        private Func<IQueryable<T>, string, IQueryable<T>> _search;
        private readonly T _instance = new T();
        private string _tableName;

        public DotNetCrud<T> TableFields(params string[] fields)
        {
            _fields.Clear();
            foreach (var fieldName in fields)
            {
                _fields.Add(fieldName);
            }
            return this;
        }

        private readonly Dictionary<string, string> fieldNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> dbFields = new Dictionary<string, string>();
        private readonly Dictionary<string, string> classFields = new Dictionary<string, string>();
        private readonly List<string> dbPrimaryKeys = new List<string>();

        private readonly Dictionary<string, IQueryable<object>> RelatedQuery = new Dictionary<string, IQueryable<object>>();
        private readonly Dictionary<string, string> RelatedKey = new Dictionary<string, string>();
        private readonly Dictionary<string, string> RelatedCaption = new Dictionary<string, string>();

        public DotNetCrud(HttpContext httpContext, DbContext dbContext)
        {
            _db = dbContext;
            _context = httpContext;
            _request = _context.Request;
            _response = _context.Response;
            _table = dbContext.Set<T>();
        }

        public DotNetCrud<T> ColumnName(Func<T, string> fieldName, string name)
        {
            var field = fieldName(_instance);

            fieldNames[field] = name;
            return this;
        }

        public DotNetCrud<T> Relation(string fieldName, IQueryable<object> relatedTable, string relatedKey, string relatedCaption)
        {
            RelatedQuery.Add(fieldName, relatedTable);
            RelatedKey.Add(fieldName, relatedKey);
            RelatedCaption.Add(fieldName, relatedCaption);

            //relatedTable.Where(i=> EF.Property<object>(i, relatedKey) == )
            return this;
        }

        public string Build()
        {
            return BuildAsync(null).Result;
        }

        public DotNetCrud<T> Key(Func<T, string> idFieldName)
        {
            _idField = idFieldName(_instance);
            return this;
        }

        public DotNetCrud<T> Fields(Func<T, string[]> select)
        {
            _fields.Clear();
            var fieldNames = select.Invoke(_instance);
            foreach (var fieldName in fieldNames)
            {
                _fields.Add(fieldName);
            }
            return this;
        }

        public DotNetCrud<T> TableSearch(Func<IQueryable<T>, string, IQueryable<T>> searchQuery)
        {
            _search = searchQuery;
            return this;
        }

        private void GetEntityInfo()
        {
            var entityType = _db.Model.FindEntityType(typeof(T));

            // Table info 
            _tableName = entityType.GetTableName();

            // Column info 
            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName();
                var columnType = property.GetColumnType();

                dbFields.Add(columnName, columnType);

                var isPrimaryKey = property.IsPrimaryKey();
                var fieldName = property.Name;
                var fieldType = property.ClrType.Name;
                if (fieldType.StartsWith("Nullable"))
                {
                    fieldType = property.ClrType.GenericTypeArguments[0].Name;
                }
                classFields.Add(fieldName, fieldType);

                if (property.IsPrimaryKey())
                {
                    dbPrimaryKeys.Add(fieldName);
                }

            };

            foreach (var item in entityType.GetNavigations())
            {
                var subEntityType = item.GetTargetType();

                foreach (var property in subEntityType.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    var columnType = property.GetColumnType();

                    dbFields.Add($"{item.Name}.{columnName}", columnType);

                    var fieldName = property.Name;
                    var fieldType = property.ClrType.Name;
                    if (fieldType.StartsWith("Nullable"))
                    {
                        fieldType = property.ClrType.GenericTypeArguments[0].Name;
                    }
                    classFields.Add($"{item.Name}.{fieldName}", fieldType);

                };
            }


        }

        public async System.Threading.Tasks.Task<string> BuildAsync(string buildName)
        {
            GetEntityInfo();

            if (string.IsNullOrEmpty(buildName))
            {
                buildName = _tableName;
            }

            if (string.IsNullOrEmpty(_idField))
            {
                _idField = dbPrimaryKeys.FirstOrDefault();
            }

            if (!_fields.Any())
            {
                _fields.AddRange(classFields.Select(i => i.Key).Where(i => !i.Contains(".")));
            }

            Cache.Singleton.Get(buildName)["Types"] = JsonSerializer.Serialize(classFields);
            Cache.Singleton.Get(buildName)["Fields"] = JsonSerializer.Serialize(_fields);
            Cache.Singleton.Get(buildName)["Id"] = _idField;

            var page = new StringBuilder();

            if (_request.Method == "POST")
            {
                if (_request.Query.ContainsKey("datatables"))
                {
                    _response.ContentType = "application/json; charset=utf-8";
                    _response.StatusCode = StatusCodes.Status200OK;
                    await _response.WriteAsync(Datatables());
                    return null;
                }
                else if (_request.Query.ContainsKey("delete"))
                {
                    var key = _request.Query["id"].ToString();
                    var row = _table.Where(i => EF.Property<object>(i, _idField) == key).FirstOrDefault();

                    new DeletePage().Execute(_request, _db, key, row, _fields, classFields);

                    _response.Redirect("/home/index");
                }
                else if (_request.Query.ContainsKey("edit"))
                {
                    var key = _request.Query["id"].ToString();
                    var row = _table.Where(i => EF.Property<object>(i, _idField) == key).FirstOrDefault();

                    new EditPage().Execute(_request, _db, key, row, _fields, classFields);

                    _response.Redirect("/home/index");
                }
                else if (_request.Query.ContainsKey("insert"))
                {
                    var row = new T();

                    new InsertPage().Execute(_request, _db, _idField, row, _fields, classFields);

                    _response.Redirect("/home/index");
                }
            }
            else if (_request.Method == "GET")
            {
                if (_request.Query.ContainsKey("edit"))
                {
                    // Render modify page
                    var key = _request.Query["id"].ToString();
                    var row = _table.Where(i => EF.Property<object>(i, _idField) == key).FirstOrDefault();

                    new EditPage().Render(key, row, page, _fields, classFields);
                }
                else if (_request.Query.ContainsKey("delete"))
                {
                    // Render delete page
                    var key = _request.Query["id"].ToString();
                    var row = _table.Where(i => EF.Property<object>(i, _idField) == key).FirstOrDefault();

                    new DeletePage().Render(key, row, page, _fields, classFields);
                }
                else if (_request.Query.ContainsKey("insert"))
                {
                    // Render new page
                    var key = "";
                    var row = new T();

                    new InsertPage().Render(key, row, page, _fields, classFields);
                }
                else
                {
                    // Render list page
                    new ListPage().Render(page, _fields, classFields);
                }
            }

            Cache.Singleton.Get("")["Page"] = page.ToString();
            return page.ToString();
        }

        private string Datatables()
        {
            string draw = _request.Form["draw"].FirstOrDefault();

            string start = _request.Form["start"].FirstOrDefault();
            string length = _request.Form["length"].FirstOrDefault();
            string sortColumn = _request.Form["columns[" + _request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            string sortColumnDirection = _request.Form["order[0][dir]"].FirstOrDefault();
            string searchValue = _request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // Getting data
            var dataCollection = _table;

            // Total number of rows counts
            int recordsTotal = dataCollection.Count();

            // Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                dataCollection = _search(dataCollection, searchValue);
            }

            // Sorting  
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                if (sortColumnDirection == "asc")
                {
                    dataCollection = dataCollection.OrderBy(p => EF.Property<object>(p, sortColumn));
                }
                else
                {
                    dataCollection = dataCollection.OrderByDescending(p => EF.Property<object>(p, sortColumn));
                }
            }

            // Number of filtered rows
            int recordsFiltered = dataCollection.Count();

            dataCollection.Include("CustomerAddress");

            // Paging
            var data = dataCollection.Skip(skip).Take(pageSize);

            // Select
            if (!_fields.Contains(_idField))
            {
                _fields.Add(_idField);
            }

            // TODO workaround per togliere il campo dalle navigation e lasciare solo quelle
            var _fields2 = new List<string>();
            for (int i = 0; i < _fields.Count; i++)
            {
                _fields2.Add(_fields[i].Split('.')[0]);
                if (_fields[i].Split('.').Length > 1)
                {
                    data = data.Include(_fields[i].Split('.')[0]);
                }
            }
            data = data.SelectMembers(_fields2);

            return JsonSerializer.Serialize(new
            {
                draw,
                recordsFiltered,
                recordsTotal,
                data = data.ToList()
            });
        }

    }
}
