using Csla.Reflection;
using Spackle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.Services.AppServer.Tests
{
  internal static class EntityCreator
  {
    internal static T Create<T>()
      where T : new()
    {
      var generator = new RandomObjectGenerator();

      var entity = new T();

      foreach (var property in
        (from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
         where p.CanWrite
         select p))
      {
        try
        {
          property.SetValue(entity,
            typeof(RandomObjectGenerator)
              .GetMethod("Generate", Type.EmptyTypes)
              .MakeGenericMethod(new[] { property.PropertyType })
              .Invoke(generator, null));
        }
        catch (TargetInvocationException) { }
      }

      return entity;
    }

    internal static T Create<T>(Action<T> modifier)
      where T : new()
    {
      var entity = EntityCreator.Create<T>();
      modifier(entity);
      return entity;
    }
  }

  public static class ObjectExtensions
  {
    public static string GetPropertyName<T>(this T @this, Expression<Func<T, object>> propertyExpression)
    {
      return Reflect<T>.GetProperty(propertyExpression).Name;
    }
  }

}
