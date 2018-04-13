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
                    Name = "name1",
                    Hour = "now"
                },
                HardCase = new [] {
                    "item",
                    "another item"
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

            var result = BsonComposite.FromBytes(ms.ToArray());
            var txt = result.ToStringShort();

            var bytes =  result.ToBson();

            var readBackStuff = BsonComposite.FromBytes(bytes).ToStringShort();

            Assert.Equal(txt, readBackStuff);
        }
    }
}