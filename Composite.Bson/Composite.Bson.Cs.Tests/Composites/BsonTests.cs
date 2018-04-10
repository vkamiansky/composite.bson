using System;
using System.IO;
using System.Linq;

using Composite.Bson;
using Composite.Test.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Xunit;

namespace Composite.Cs.Tests.Composites
{
    public class BsonTests
    {
        [Fact]
        public void TransformationTest()
        {
            var serObj = new {
                Name = "test result",
                Thing = new { 
                    Name = "bad",
                    Hour = "now"
                },
                HardCase = new [] {
                    "beauty",
                    "another beauty"
                },
                Me = "vk",
                Var = DateTime.Now,
                Try = new[]{
                    new{ 
                        Zx = new[]{
                            "try"
                        }
                    }
                }
            };

            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, serObj);
            }

            var result = Composite.Bson.Comp.ofBson(ms.ToArray());

            var thingsubtree = result.ToComponents().Select(x => x.ToStringShort()).ToArray();

            //var thingOfThings = result.ToComponents().Skip(1).Take(1).Select(x => x.ToStringShort()).ToArray().First();

            //var enumu = result.AsEnumerable().FirstOrDefault(x => x.Key == "Thing.Name");
           // var txtThing = thingsubtree.ToStringShort();
            var txt = result.ToStringShort();
        }
    }
}