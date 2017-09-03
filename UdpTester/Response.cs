using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpTester
{
    public class Response
    {
        public ushort Id { get; set; }

        public uint ReceivedFrames { get; private set; }

        public uint SentFrames { get; private set; }

        public uint ErrorCount { get; private set; }

        public uint RepeatCount { get; private set; }


        public static Response Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                using (var br = new BinaryReader(stream))
                {
                    var result = new Response();

                    result.Id = br.ReadUInt16();
                    result.ReceivedFrames = br.ReadUInt32();
                    result.SentFrames = br.ReadUInt32();
                    result.ErrorCount = br.ReadUInt32();
                    result.RepeatCount = br.ReadUInt32();

                    return result;
                }
            }
        }
        public override string ToString()
        {
            return $"{nameof(Id)} - {Id}, {nameof(ReceivedFrames)} - {ReceivedFrames}, {nameof(SentFrames)} - {SentFrames}, {nameof(ErrorCount)} - {ErrorCount}, {nameof(RepeatCount)} - {RepeatCount}, ";
        }
    }
}
