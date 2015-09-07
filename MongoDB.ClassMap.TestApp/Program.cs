using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.ClassMap.TestApp.Data;
using MongoDB.ClassMap.TestApp.Data.ClassMaps;
using MongoDB.ClassMap.TestApp.Domain;
using MongoDB.Driver;

namespace MongoDB.ClassMap.TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //only happens once on startup
            InitialiseMongoDatabase();

            //now, use mongodb as you normally would -
            //not doing anything clever here / abstracting this part (yet)
            var connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase("classMapTest"); // "test" is the name of the database

            var collection = database.GetCollection<MyClass>("myClass");

            var myClass = new MyClass()
            {
                SomeProperty = "this will be saved",
                SomeOtherProperty = "this will be ignored"
            };

            Task t = Insert(collection, myClass);
            t.Wait();

            Console.WriteLine("MyClass saved");
            Console.WriteLine("SomeProperty = " + myClass.SomeProperty);
            Console.WriteLine("SomeOtherProperty = " + myClass.SomeOtherProperty);

            Console.WriteLine();

            var item = collection.Find(_ => true).ToListAsync().Result.First();

            Console.WriteLine("MyClass retrieved");
            Console.WriteLine("SomeProperty = " + item.SomeProperty);
            Console.WriteLine("SomeOtherProperty = " + item.SomeOtherProperty); //this will be blank

            Console.Read();
        }

        private static async Task Insert(IMongoCollection<MyClass> collection, MyClass myClass)
        {
            await collection.InsertOneAsync(myClass);
        }

        private static void InitialiseMongoDatabase()
        {
            //How you get this assembly is up to you
            //It could be this assembly
            //Or it could be a collection of assemblies, in which case, wrap this block in a foreach and iterate
            var assembly = Assembly.GetAssembly(typeof(MyClassClassMap));

            //get all types that have our MongoDbClassMap as their base class
            var classMaps = assembly
                .GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == typeof(MongoDbClassMap<>));

            //automate the new *ClassMap()
            foreach (var classMap in classMaps)
                Activator.CreateInstance(classMap);
        }
    }
}