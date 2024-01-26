using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Entities.LinkModels;

namespace Entities.Models;

public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
{
    private readonly string _root = "Entity";
    private readonly IDictionary<string, object> _expando;

    public Entity()
    {
        _expando = new ExpandoObject();
    }

    public object this[string key]
    {
        get
        {
            return _expando[key];
        }
        set
        {
            _expando[key] = value;
        }
    }

    public ICollection<string> Keys => throw new NotImplementedException();

    public ICollection<object> Values => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(string key, object value)
    {
        _expando.Add(key, value);
    }

    public void Add(KeyValuePair<string, object> item)
    {
        _expando.Add(item);
    }

    public void Clear()
    {
        _expando.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return _expando.Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return _expando.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        _expando.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _expando.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _expando.Remove(key);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return _expando.Remove(item);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        return _expando.TryGetValue(key, out value);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if(_expando.TryGetValue(binder.Name, out object value))
        {
            result = value;
            return true;
        }

        return base.TryGetMember(binder, out result);
    }

    public XmlSchema? GetSchema()
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _expando.GetEnumerator();
    }
}