using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Design_Patterns
{
    public class Rectangle
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle()
        {

        }

        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
        }
    }

    public class Square : Rectangle
    {
        public new int Width
        {
            set
            {
                base.Width = base.Height = value;
            }
        }

        public new int Height
        {
            set
            {
                base.Width = base.Height = value;
            }
        }
    }

    public class Cat
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public float Weight { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class Program
    {
        public static int Area(Rectangle? r)
        {
            if (r == null)
            {
                return 0;
            }

            return r.Width * r.Height;
        }

        static void Main(string[] args)
        {


            var selects = new string[] { "Birthday", "Age", "Name" };


            var propsDict = new Dictionary<string, PropertyInfo>();

            var props = typeof(Cat).GetProperties();

            foreach (var prop in props)
            {
                propsDict.Add(prop.Name, prop);
            }

            //var catAccessor = new Accessor<Cat>();
            var accessors = new Dictionary<string, Func<Cat, object>>
            {
                {"Age", Expressions.BuildGetAccessor("Age")},
                {"Birthday", Expressions.BuildGetAccessor("Birthday")},
                {"Name", Expressions.BuildGetAccessor("Name")},
                {"Weight", Expressions.BuildGetAccessor("Weight")}
            };

            // foreach (var select in selects)
            // {
            //     var propInfo = typeof(Cat).GetProperties().Single(prop => prop.Name == select);
            //     var getMethod = propInfo.GetGetMethod();
            //     if (getMethod == null) continue;
            //     var access = Expressions.BuildGetAccessor<Cat, object>(cat => cat.Age);
            //     accessors.Add(select, access);
            // }

            var keepGoing = true;
            while (keepGoing)
            {
                var cats = new List<Cat>();

                for (int i = 0; i < 1000000; i++)
                {
                    cats.Add(new Cat
                    {
                        Age = i,
                        Birthday = DateTime.Now
                    });
                }

                var flatCats = new List<Dictionary<string, object>>();

                var stopwatch = new Stopwatch();

                stopwatch.Start();
                foreach (var cat in cats)
                {
                    var catDict = new Dictionary<string, object>();

                    foreach (var select in selects)
                    {
                        //cat.GetType().GetProperty(select);//
                        // var propInfo = propsDict[select];

                        // if (propInfo == null) continue;

                        // var currentVal = propInfo.GetValue(cat);

                        // if (currentVal == null)
                        // {
                        //     continue;
                        // }
                        var accessor = accessors[select];

                        var currentVal = accessor(cat);

                        catDict.Add(select, currentVal);
                    }

                    // Console.WriteLine(JsonSerializer.Serialize(catDict, new JsonSerializerOptions {
                    //     WriteIndented = true
                    // }));
                    flatCats.Add(catDict);
                }

                stopwatch.Stop();

                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                // foreach (var cat in stuff)
                // {
                //     Console.WriteLine(JsonSerializer.Serialize(cat, new JsonSerializerOptions
                //     {
                //         WriteIndented = true
                //     }));
                // }

                var userValue = Console.ReadLine();

                if (userValue == "b")
                {
                    keepGoing = false;
                }
            }

            /*  var cb = new CodeBuilder("Person").AddField("Name", "string").AddField("Age", "int");
              Console.WriteLine(cb);*/

            // var t1 = Task.Run(() =>
            // {
            //     Console.WriteLine("Starting task 1");

            //     Thread.Sleep(6000);

            //     Console.WriteLine("Task 1 complete");
            // });

            // var t2 = Task.Run(() =>
            // {
            //     Console.WriteLine("Starting task 2");

            //     Thread.Sleep(4000);

            //     Console.WriteLine("Task 2 complete");
            // });

            // await Task.WhenAll(t1, t2);

            // Console.WriteLine("All done!");
        }
    }

    // public class Accessor<S>
    // {
    //     public static Accessor<S, T> Create<T>(Expression<Func<S, T>> memberSelector)
    //     {
    //         return new GetterSetter<T>(memberSelector);
    //     }

    //     public Accessor<S, T> Get<T>(Expression<Func<S, T>> memberSelector)
    //     {
    //         return Create(memberSelector);
    //     }

    //     public Accessor()
    //     {

    //     }

    //     class GetterSetter<T> : Accessor<S, T>
    //     {
    //         public GetterSetter(Expression<Func<S, T>> memberSelector) : base(memberSelector)
    //         {

    //         }
    //     }
    // }

    // public class Accessor<S, T> : Accessor<S>
    // {
    //     Func<S, T> Getter;
    //     Action<S, T> Setter;

    //     public bool IsReadable { get; private set; }
    //     public bool IsWritable { get; private set; }
    //     public T this[S instance]
    //     {
    //         get
    //         {
    //             if (!IsReadable)
    //                 throw new ArgumentException("Property get method not found.");

    //             return Getter(instance);
    //         }
    //         set
    //         {
    //             if (!IsWritable)
    //                 throw new ArgumentException("Property set method not found.");

    //             Setter(instance, value);
    //         }
    //     }

    //     protected Accessor(Expression<Func<S, T>> memberSelector) //access not given to outside world
    //     {
    //         var prop = memberSelector.GetPropertyInfo();
    //         IsReadable = prop.CanRead;
    //         IsWritable = prop.CanWrite;
    //         AssignDelegate(IsReadable, ref Getter, prop.GetGetMethod());
    //         AssignDelegate(IsWritable, ref Setter, prop.GetSetMethod());
    //     }

    //     void AssignDelegate<K>(bool assignable, ref K assignee, MethodInfo assignor) where K : class
    //     {
    //         if (assignable)
    //             assignee = assignor.CreateDelegate<K>();
    //     }
    // }

    static class Expressions
    {
        public static Func<Cat, object> BuildGetAccessor(string propName)//(Expression<Func<S, T>> propertySelector)
        {
            //return propertySelector.GetPropertyInfo().GetGetMethod().CreateDelegate<Func<S, T>>();
            return typeof(Cat).GetProperty(propName).GetGetMethod().CreateDelegate(typeof(Cat));
        }

        public static Action<S, T> BuildSetAccessor<S, T>(Expression<Func<S, T>> propertySelector)
        {
            return propertySelector.GetPropertyInfo().GetSetMethod().CreateDelegate<Action<S, T>>();
        }

        // a generic extension for CreateDelegate
        public static T CreateDelegate<T>(this MethodInfo method) where T : class
        {
            return Delegate.CreateDelegate(typeof(T), method) as T;
        }

        public static PropertyInfo GetPropertyInfo<S, T>(this Expression<Func<S, T>> propertySelector)
        {
            var body = propertySelector.Body as MemberExpression;
            if (body == null)
                throw new MissingMemberException("something went wrong");

            return body.Member as PropertyInfo;
        }

        // public static Func<object, object> BuildGetAccessor(MethodInfo method)
        // {
        //     var obj = Expression.Parameter(typeof(object), "o");

        //     Expression<Func<object, object>> expr =
        //         Expression.Lambda<Func<object, object>>(
        //             Expression.Convert(
        //                 Expression.Call(
        //                     Expression.Convert(obj, method.DeclaringType),
        //                     method),
        //                 typeof(object)),
        //             obj);

        //     return expr.Compile();
        // }
    }

    public static class Helpers
    {
        public static Func<T, T> DynamicSelectGenerator<T>(string Fields = "")
        {
            string[] EntityFields;
            if (Fields == "")
                // get Properties of the T
                EntityFields = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray();
            else
                EntityFields = Fields.Split(',');

            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(T), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(T));

            // create initializers
            var bindings = EntityFields.Select(o => o.Trim())
                .Select(o =>
                {

                    // property "Field1"
                    var mi = typeof(T).GetProperty(o);

                    // original value "o.Field1"
                    var xOriginal = Expression.Property(xParameter, mi);

                    // set value "Field1 = o.Field1"
                    return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }
    }
}
