using System.Dynamic;
using System.Reflection;
using Contracts;

namespace Services.DataShaping;

public class DataShaper<T> : IDataShaper<T>
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);

        return FetchData(entities, requiredProperties);
    }

    public ExpandoObject ShapeData(T entity, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);

        return FetchDataForEntity(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldString)
    {
        var listOfProperty = new List<PropertyInfo>();

        if(string.IsNullOrEmpty(fieldString))
        {
            return listOfProperty = Properties.ToList();
        }

        var queryString = fieldString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach(var field in queryString)
        {
            var property = Properties.FirstOrDefault(item => item.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if(property is null)
            {
                continue;
            }

            listOfProperty.Add(property);
        }

        return listOfProperty;
    }

    private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> listOfProperty)
    {
        var shapeData = new List<ExpandoObject>();

        foreach(var entity in entities)
        {
            shapeData.Add(FetchDataForEntity(entity, listOfProperty));
        }

        return shapeData;
    }

    private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> listOfProperty)
    {
        var shapedObject = new ExpandoObject();


        foreach(var property in listOfProperty)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.TryAdd(property.Name, objectPropertyValue);
        }

        return shapedObject;
    }
}
