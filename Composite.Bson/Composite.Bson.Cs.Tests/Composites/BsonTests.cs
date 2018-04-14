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
        public void SerializationCycleTest()
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

            var res = BsonComposite.FromStream(new MemoryStream(ms.ToArray()));
            var resStr = res.ToStringShort();

            res = BsonComposite.FromStream(new MemoryStream(ms.ToArray()));
            var outStream = new MemoryStream();
            res.WriteToStream(outStream);

            var res2 = BsonComposite.FromStream(new MemoryStream(outStream.ToArray()));
            var res2Str = res2.ToStringShort();

            Assert.Equal(resStr, res2Str);
        }
    }
}