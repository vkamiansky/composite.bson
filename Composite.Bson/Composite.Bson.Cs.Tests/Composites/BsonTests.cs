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

        [Fact]
        public void SerializationObjectTest()
        {
            var obj = new {
                Simple = new Simple{Number = 3}
            };

            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }

            var resStr = BsonComposite.FromStream(new MemoryStream(ms.ToArray())).ToStringShort();
            Assert.Equal("( Document )[ ( Object )[ ( Object-Property: Simple, Path: .Simple )[ ( BsonInteger-Property: Number, Path: .Simple.Number )3 ] ] ]", resStr);
        }

        [Fact]
        public void SerializationArrayTest()
        {
            var obj = new {
                Array = new []{
                    new Simple{Number = 3}
                }
            };

            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }

            var resStr = BsonComposite.FromStream(new MemoryStream(ms.ToArray())).ToStringShort();
            Assert.Equal("( Document )[ ( Object )[ ( Array-Property: Array, Path: .Array )[ ( Object-Property: [0], Path: .Array.[0] )[ ( BsonInteger-Property: Number, Path: .Array.[0].Number )3 ] ] ] ]", resStr);
        }

        [Fact]
        public void BsonConstructionTest()
        {
            var resStr = BsonComposite
                        .New()
                        .EnsureHasObject("","Me")
                        .EnsureHasArray(".Me", "Words")
                        .SetValue(".Me.Words", "[0]", "star")
                        .SetValue(".Me.Words", "[0]", 1)
                        .SetValue(".Me.Words", "[1]", "star3")
                        .ToStringShort();
            Assert.Equal("( Document )[ ( Object )[ ( Object-Property: Me, Path: .Me )[ ( Array-Property: Words, Path: .Me.Words )[ ( BsonInteger-Property: [0], Path: .Me.Words.[0] )1, ( BsonString-Property: [1], Path: .Me.Words.[1] )star3 ] ] ] ]", resStr);
        }
    }
}