using log4net;
using Org.Apache.Zookeeper.Data;
using Org.Apache.Zookeeper.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZooKeeper.Net
{
    /// <summary>
    /// 
    /// </summary>

    [DebuggerDisplay("Id = {Id}")]
    public class ZooKeeper : IDisposable, IZooKeeper
    {
        private readonly ZKWatchManager watchManager = new ZKWatchManager();
        private static readonly ILog log = LogManager.GetLogger(typeof(ZooKeeper));

        /// <summary>
        /// 数据监听集合
        /// </summary>
        public IEnumerable<string> DataWatches
        {
            get
            {
                return watchManager.dataWatches.Keys;
            }
        }

        /// <summary>
        /// 判断存在节点的监听集合
        /// </summary>
        public IEnumerable<string> ExistWatches
        {
            get
            {
                return watchManager.existWatches.Keys;
            }
        }

        /// <summary>
        /// 子节点监听集合
        /// </summary>
        public IEnumerable<string> ChildWatches
        {
            get
            {
                return watchManager.childWatches.Keys;
            }
        }



        private Guid id = Guid.NewGuid();
        private IClientConnection cnxn;
        private volatile States state = States.NOT_CONNECTED;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectstring">
        /// 连接字符串（格式：127.0.0.1:3000,127.0.0.1:3001,127.0.0.1:3002）
        /// </param>
        /// <param name="sessionTimeout">会话超时时间</param>
        /// <param name="watcher">
        ///  接收节点监听通知对象         
        /// </param>
        public ZooKeeper(string connectstring, TimeSpan sessionTimeout, IWatcher watcher)
        {
            log.InfoFormat("Initiating client connection, connectstring={0} sessionTimeout={1} watcher={2}", connectstring, sessionTimeout, watcher);
            watchManager.DefaultWatcher = watcher;
            cnxn = new ClientConnection(connectstring, sessionTimeout, this, watchManager);
            cnxn.Start();
        }

        /// <summary>
        /// 
        /// 初始化</summary>
        /// <param name="connectstring"></param>
        /// <param name="sessionTimeout"></param>
        /// <param name="watcher"></param>
        /// <param name="sessionId"></param>
        /// <param name="sessionPasswd"></param>
        public ZooKeeper(string connectstring, TimeSpan sessionTimeout, IWatcher watcher, long sessionId, byte[] sessionPasswd)
        {
            log.InfoFormat("Initiating client connection, connectstring={0} sessionTimeout={1} watcher={2} sessionId={3} sessionPasswd={4}", connectstring, sessionTimeout, watcher, sessionId, (sessionPasswd == null ? "<null>" : "<hidden>"));

            watchManager.DefaultWatcher = watcher;
            cnxn = new ClientConnection(connectstring, sessionTimeout, this, watchManager, sessionId, sessionPasswd);
            cnxn.Start();
        }

        /// <summary>
        /// Unique ID representing the instance of the client
        /// </summary>
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The session id for this ZooKeeper client instance. The value returned is
        /// not valid until the client connects to a server and may change after a
        /// re-connect.
        /// </summary>
        /// <value>The session id.</value>
        public long SessionId
        {
            get
            {
                return cnxn.SessionId;
            }
        }

        /// <summary>
        /// The session password for this ZooKeeper client instance. The value
        /// returned is not valid until the client connects to a server and may
        ///  change after a re-connect.
        ///
        /// This method is NOT thread safe
        /// </summary>
        /// <value>The sesion password.</value>
        public byte[] SesionPassword
        {
            get
            {
                return cnxn.SessionPassword;
            }
        }

        /// <summary>
        /// The negotiated session timeout for this ZooKeeper client instance. The
        /// value returned is not valid until the client connects to a server and
        /// may change after a re-connect.
        /// 
        /// This method is NOT thread safe
        /// </summary>
        /// <value>The session timeout.</value>
        public TimeSpan SessionTimeout
        {
            get
            {
                return cnxn.SessionTimeout;
            }
        }

        /// <summary>
        /// Add the specified scheme:auth information to this connection.
        ///
        /// This method is NOT thread safe
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="auth">The auth.</param>
        public void AddAuthInfo(string scheme, byte[] auth)
        {
            cnxn.AddAuthInfo(scheme, auth);
        }

        /// <summary>
        /// Specify the default watcher for the connection (overrides the one
        /// specified during construction).
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        public void Register(IWatcher watcher)
        {
            watchManager.DefaultWatcher = watcher;
        }

        /// <summary>
        /// The State of ZooKeeper connection
        /// Thread safe
        /// </summary>
        public States State
        {
            get
            {
                return Interlocked.CompareExchange(ref state, null, null);
            }
            internal set
            {
                Interlocked.Exchange(ref state, value);
            }
        }

        /// <summary>
        /// Close this client object. Once the client is closed, its session becomes
        /// invalid. All the ephemeral nodes in the ZooKeeper server associated with
        /// the session will be removed. The watches left on those nodes (and on
        /// their parents) will be triggered.
        /// </summary>   
        private void InternalDispose()
        {
            var connectionState = State;
            if (null != connectionState && !connectionState.IsAlive())
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Close called on already closed client");
                }
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Closing session: 0x{0:X}", SessionId);
            }

            try
            {
                cnxn.Dispose();
            }
            catch (Exception e)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Ignoring unexpected exception during close", e);
                }
            }

            log.DebugFormat("Session: 0x{0:X} closed", SessionId);
        }
        
        /// <summary>
        /// Prepend the chroot to the client path (if present). The expectation of
        /// this function is that the client path has been validated before this
        /// function is called
        /// </summary>
        /// <param name="clientPath">The path to the node.</param>
        /// <returns>server view of the path (chroot prepended to client path)</returns>
        private string PrependChroot(string clientPath)
        {
            if (cnxn.ChrootPath != null)
            {
                // handle clientPath = "/"
                return clientPath.Length == 1 ? cnxn.ChrootPath : cnxn.ChrootPath + clientPath;
            }
            return clientPath;
        }

        /// <summary>
        /// Create a node with the given path. The node data will be the given data,
        /// and node acl will be the given acl.
        /// 
        /// The flags argument specifies whether the created node will be ephemeral
        /// or not.
        /// 
        /// An ephemeral node will be removed by the ZooKeeper automatically when the
        /// session associated with the creation of the node expires.
        /// 
        /// The flags argument can also specify to create a sequential node. The
        /// actual path name of a sequential node will be the given path plus a
        /// suffix "i" where i is the current sequential number of the node. The sequence
        /// number is always fixed length of 10 digits, 0 padded. Once
        /// such a node is created, the sequential number will be incremented by one.
        /// 
        /// If a node with the same actual path already exists in the ZooKeeper, a
        /// KeeperException with error code KeeperException.NodeExists will be
        /// thrown. Note that since a different actual path is used for each
        /// invocation of creating sequential node with the same path argument, the
        /// call will never throw "file exists" KeeperException.
        /// 
        /// If the parent node does not exist in the ZooKeeper, a KeeperException
        /// with error code KeeperException.NoNode will be thrown.
        /// 
        /// An ephemeral node cannot have children. If the parent node of the given
        /// path is ephemeral, a KeeperException with error code
        /// KeeperException.NoChildrenForEphemerals will be thrown.
        /// 
        /// This operation, if successful, will trigger all the watches left on the
        /// node of the given path by exists and getData API calls, and the watches
        /// left on the parent node by getChildren API calls.
        /// 
        /// If a node is created successfully, the ZooKeeper server will trigger the
        /// watches on the path left by exists calls, and the watches on the parent
        /// of the node by getChildren calls.
        /// 
        /// The maximum allowable size of the data array is 1 MB (1,048,576 bytes).
        /// Arrays larger than this will cause a KeeperExecption to be thrown.
        /// </summary>
        /// <param name="path">The path for the node.</param>
        /// <param name="data">The data for the node.</param>
        /// <param name="acl">The acl for the node.</param>
        /// <param name="createMode">specifying whether the node to be created is ephemeral and/or sequential.</param>
        /// <returns></returns>
        public string Create(string path, byte[] data, IEnumerable<ACL> acl, CreateMode createMode)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath, createMode.Sequential);
            if (acl != null && acl.Count() == 0)
            {
                throw new KeeperException.InvalidACLException();
            }

            string serverPath = PrependChroot(clientPath);
            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.Create;
            CreateRequest request = new CreateRequest(serverPath, data, acl, createMode.Flag);
            CreateResponse response = new CreateResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, null);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            return cnxn.ChrootPath == null ? response.Path : response.Path.Substring(cnxn.ChrootPath.Length);
        }

        /// <summary>
        /// Delete the node with the given path. The call will succeed if such a node
        /// exists, and the given version matches the node's version (if the given
        /// version is -1, it matches any node's versions).
        ///
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if the nodes does not exist.
        ///
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// A KeeperException with error code KeeperException.NotEmpty will be thrown
        /// if the node has children.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by exists API calls, and the watches on the parent
        /// node left by getChildren API calls.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="version">The version.</param>
        public void Delete(string path, int version)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);
            string serverPath;

            // maintain semantics even in chroot case
            // specifically - root cannot be deleted
            // I think this makes sense even in chroot case.
            if (clientPath.Equals(PathUtils.PathSeparator))
            {
                // a bit of a hack, but delete(/) will never succeed and ensures
                // that the same semantics are maintained
                serverPath = clientPath;
            }
            else
            {
                serverPath = PrependChroot(clientPath);
            }

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.Delete;
            DeleteRequest request = new DeleteRequest(serverPath, version);
            ReplyHeader r = cnxn.SubmitRequest(h, request, null, null);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
        }

        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="watcher">The watcher.</param>
        /// <returns>the stat of the node of the given path; return null if no such a node exists.</returns>
        public Stat Exists(string path, IWatcher watcher)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            // the watch contains the un-chroot path
            WatchRegistration wcb = null;
            if (watcher != null)
            {
                wcb = new ExistsWatchRegistration(watchManager, watcher, clientPath);
            }

            string serverPath = PrependChroot(clientPath);
            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.Exists;
            ExistsRequest request = new ExistsRequest(serverPath, watcher != null);
            SetDataResponse response = new SetDataResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, wcb);
            if (r.Err != 0)
            {
                if (r.Err == (int)KeeperException.Code.NONODE)
                {
                    return null;
                }
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }

            return response.Stat.Czxid == -1 ? null : response.Stat;
        }

        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// @param path
        ///                the node path
        /// @param watch
        ///                whether need to watch this node
        /// @return the stat of the node of the given path; return null if no such a
        ///         node exists.
        /// @throws KeeperException If the server signals an error
        /// @throws InterruptedException If the server transaction is interrupted.
        /// </summary>
        public Stat Exists(string path, bool watch)
        {
            return Exists(path, watch ? watchManager.DefaultWatcher : null);
        }

        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is
        /// thrown), a watch will be left on the node with the given path. The watch
        /// will be triggered by a successful operation that sets data on the node, or
        /// deletes the node.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @param path the given path
        /// @param watcher explicit watcher
        /// @param stat the stat of the node
        /// @return the data of the node
        /// @throws KeeperException If the server signals an error with a non-zero error code
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>
        public byte[] GetData(string path, IWatcher watcher, Stat stat)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            // the watch contains the un-chroot path
            WatchRegistration wcb = null;
            if (watcher != null)
            {
                wcb = new DataWatchRegistration(watchManager, watcher, clientPath);
            }

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.GetData;
            GetDataRequest request = new GetDataRequest(serverPath, watcher != null);
            GetDataResponse response = new GetDataResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, wcb);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            if (stat != null)
            {
                DataTree.CopyStat(response.Stat, stat);
            }
            return response.Data;
        }

        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// 
        /// If the watch is true and the call is successful (no exception is
        /// thrown), a watch will be left on the node with the given path. The watch
        /// will be triggered by a successful operation that sets data on the node, or
        /// deletes the node.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @param path the given path
        /// @param watch whether need to watch this node
        /// @param stat the stat of the node
        /// @return the data of the node
        /// @throws KeeperException If the server signals an error with a non-zero error code
        /// @throws InterruptedException If the server transaction is interrupted.
        /// </summary>
        public byte[] GetData(string path, bool watch, Stat stat)
        {
            return GetData(path, watch ? watchManager.DefaultWatcher : null, stat);
        }

        /// <summary>
        /// Set the data for the node of the given path if such a node exists and the
        /// given version matches the version of the node (if the given version is
        /// -1, it matches any node's versions). Return the stat of the node.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by getData calls.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// The maximum allowable size of the data array is 1 MB (1,048,576 bytes).
        /// Arrays larger than this will cause a KeeperExecption to be thrown.
        /// @param path
        ///                the path of the node
        /// @param data
        ///                the data to set
        /// @param version
        ///                the expected matching version
        /// @return the state of the node
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero error code.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>
        public Stat SetData(string path, byte[] data, int version)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.SetData;
            SetDataRequest request = new SetDataRequest(serverPath, data, version);
            SetDataResponse response = new SetDataResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, null);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            return response.Stat;
        }

        /// <summary>
        /// Return the ACL and stat of the node of the given path.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @param path
        ///                the given path for the node
        /// @param stat
        ///                the stat of the node will be copied to this parameter.
        /// @return the ACL array of the given node.
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero error code.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>
        public IEnumerable<ACL> GetACL(string path, Stat stat)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.GetACL;
            GetACLRequest request = new GetACLRequest(serverPath);
            GetACLResponse response = new GetACLResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, null);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            DataTree.CopyStat(response.Stat, stat);
            return response.Acl;
        }

        /// <summary>
        /// Set the ACL for the node of the given path if such a node exists and the
        /// given version matches the version of the node. Return the stat of the
        /// node.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        /// @param path
        /// @param acl
        /// @param version
        /// @return the stat of the node.
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero error code.
        /// @throws org.apache.zookeeper.KeeperException.InvalidACLException If the acl is invalide.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>
        public Stat SetACL(string path, IEnumerable<ACL> acl, int version)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);
            if (acl != null && acl.Count() == 0)
            {
                throw new KeeperException.InvalidACLException();
            }

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.SetACL;
            SetACLRequest request = new SetACLRequest(serverPath, acl, version);
            SetACLResponse response = new SetACLResponse();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, null);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            return response.Stat;
        }

        /// <summary>
        /// Return the list of the children of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @param path
        /// @param watcher explicit watcher
        /// @return an unordered array of children of the node with the given path
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero error code.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>
        public IEnumerable<string> GetChildren(string path, IWatcher watcher)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            // the watch contains the un-chroot path
            WatchRegistration wcb = null;
            if (watcher != null)
            {
                wcb = new ChildWatchRegistration(watchManager, watcher, clientPath);
            }

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.GetChildren2;
            GetChildren2Request request = new GetChildren2Request(serverPath, watcher != null);
            GetChildren2Response response = new GetChildren2Response();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, wcb);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            return response.Children;
        }

        public IEnumerable<string> GetChildren(string path, bool watch)
        {
            return GetChildren(path, watch ? watchManager.DefaultWatcher : null);
        }

        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @since 3.3.0
        /// 
        /// @param path
        /// @param watcher explicit watcher
        /// @param stat stat of the znode designated by path
        /// @return an unordered array of children of the node with the given path
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero error code.
        /// @throws IllegalArgumentException if an invalid path is specified
        /// </summary>

        public IEnumerable<string> GetChildren(string path, IWatcher watcher, Stat stat)
        {
            string clientPath = path;
            PathUtils.ValidatePath(clientPath);

            // the watch contains the un-chroot path
            WatchRegistration wcb = null;
            if (watcher != null)
            {
                wcb = new ChildWatchRegistration(watchManager, watcher, clientPath);
            }

            string serverPath = PrependChroot(clientPath);

            RequestHeader h = new RequestHeader();
            h.Type = (int)OpCode.GetChildren2;
            GetChildren2Request request = new GetChildren2Request(serverPath, watcher != null);
            GetChildren2Response response = new GetChildren2Response();
            ReplyHeader r = cnxn.SubmitRequest(h, request, response, wcb);
            if (r.Err != 0)
            {
                throw KeeperException.Create((KeeperException.Code)Enum.ToObject(typeof(KeeperException.Code), r.Err), clientPath);
            }
            if (stat != null)
            {
                DataTree.CopyStat(response.Stat, stat);
            }
            return response.Children;
        }

        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// @since 3.3.0
        /// 
        /// @param path
        /// @param watch
        /// @param stat stat of the znode designated by path
        /// @return an unordered array of children of the node with the given path
        /// @throws InterruptedException If the server transaction is interrupted.
        /// @throws KeeperException If the server signals an error with a non-zero
        ///  error code.
        /// </summary>
        public IEnumerable<string> GetChildren(string path, bool watch, Stat stat)
        {
            return GetChildren(path, watch ? watchManager.DefaultWatcher : null, stat);
        }

        /// <summary>
        /// string representation of this ZooKeeper client. Suitable for things
        /// like logging.
        /// 
        /// Do NOT count on the format of this string, it may change without
        /// warning.
        /// 
        /// @since 3.3.0
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Id: ")
                .Append(id)
                .Append(" State:")
                .Append(State);
            if (State == States.CONNECTED)
                builder.Append(" Timeout:")
                    .Append(SessionTimeout);
            builder.Append(" ")
                .Append(cnxn);
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            InternalDispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~ZooKeeper()
        {
            InternalDispose();
        }
    }
}