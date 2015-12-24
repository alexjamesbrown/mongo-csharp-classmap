using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.ClassMap.TestApp.Domain;

namespace MongoDB.ClassMap.TestApp.Data.ClassMaps
{
    public class MyClassClassMap : MongoDbClassMap<MyClass>
    {
        public override void Map(BsonClassMap<MyClass> cm)
        {
            cm.AutoMap();

            //every doc has to have an id
            cm.MapIdField(x => x.Id).SetIdGenerator(new StringObjectIdGenerator());

            cm.MapProperty(x => x.SomeProperty)
                .SetElementName("sp"); // will set the element name to "sp" in the stored documents

            //unmap the property.. now we won't save it
            cm.UnmapProperty(x => x.SomeOtherProperty);
        }
    }
}