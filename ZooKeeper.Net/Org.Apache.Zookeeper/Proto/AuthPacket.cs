using log4net;
using Org.Apache.Jute;
using System;
using ZooKeeper.Net.IO;

namespace Org.Apache.Zookeeper.Proto
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthPacket : IRecord, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthPacket));

        /// <summary>
        /// 
        /// </summary>
        public AuthPacket()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scheme"></param>
        /// <param name="auth"></param>
        public AuthPacket(int type, string scheme, byte[] auth)
        {
            Type = type;
            Scheme = scheme;
            Auth = auth;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Auth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Serialize(IOutputArchive a_, String tag)
        {
            a_.StartRecord(this, tag);
            a_.WriteInt(Type, "type");
            a_.WriteString(Scheme, "scheme");
            a_.WriteBuffer(Auth, "auth");
            a_.EndRecord(this, tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_"></param>
        /// <param name="tag"></param>
        public void Deserialize(IInputArchive a_, String tag)
        {
            a_.StartRecord(tag);
            Type = a_.ReadInt("type");
            Scheme = a_.ReadString("scheme");
            Auth = a_.ReadBuffer("auth");
            a_.EndRecord(tag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, ms, System.Text.Encoding.UTF8))
                {
                    BinaryOutputArchive a_ = new BinaryOutputArchive(writer);
                    a_.StartRecord(this, string.Empty);
                    a_.WriteInt(Type, "type");
                    a_.WriteString(Scheme, "scheme");
                    a_.WriteBuffer(Auth, "auth");
                    a_.EndRecord(this, string.Empty);
                    ms.Position = 0;
                    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "ERROR";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            BinaryOutputArchive archive = new BinaryOutputArchive(writer);
            Serialize(archive, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadFields(EndianBinaryReader reader)
        {
            BinaryInputArchive archive = new BinaryInputArchive(reader);
            Deserialize(archive, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            AuthPacket peer = (AuthPacket)obj;
            if (peer == null)
            {
                throw new InvalidOperationException("Comparing different types of records.");
            }
            int ret = 0;
            ret = (Type == peer.Type) ? 0 : ((Type < peer.Type) ? -1 : 1);
            if (ret != 0) return ret;
            ret = Scheme.CompareTo(peer.Scheme);
            if (ret != 0) return ret;
            ret = Auth.CompareTo(peer.Auth);
            if (ret != 0) return ret;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            AuthPacket peer = (AuthPacket)obj;
            if (peer == null)
            {
                return false;
            }
            if (ReferenceEquals(peer, this))
            {
                return true;
            }
            bool ret = false;
            ret = (Type == peer.Type);
            if (!ret) return ret;
            ret = Scheme.Equals(peer.Scheme);
            if (!ret) return ret;
            ret = Auth.Equals(peer.Auth);
            if (!ret) return ret;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = 17;
            int ret = GetType().GetHashCode();
            result = 37 * result + ret;
            ret = Type;
            result = 37 * result + ret;
            ret = Scheme.GetHashCode();
            result = 37 * result + ret;
            ret = Auth.GetHashCode();
            result = 37 * result + ret;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Signature()
        {
            return "LAuthPacket(isB)";
        }
    }
}