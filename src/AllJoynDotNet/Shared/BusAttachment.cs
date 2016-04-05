using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;

namespace AllJoynDotNet
{
    public partial class BusAttachment
    {
        private static Dictionary<IntPtr, WeakReference<BusAttachment>> _sBusAttachmentMap = new Dictionary<IntPtr, WeakReference<BusAttachment>>();
        internal static BusAttachment GetBusAttachment(IntPtr key)
        {
            BusAttachment bus = null;
            if (_sBusAttachmentMap.ContainsKey(key))
            {
                var instance = _sBusAttachmentMap[key];
                instance.TryGetTarget(out bus);
            }
            return bus;
        }

        private readonly string _busName;
        private readonly bool _allowRemoteMessages;

        static BusAttachment()
        {
            Init.Initialize();
        }

        #region Construct and destroy

        private static IntPtr CreateHandle(string busName, bool allowRemoteMessages)
        {
            var handle = alljoyn_busattachment_create(busName, allowRemoteMessages.ToQccBool());
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Could not create bus attachment");
#if DEBUG
            alljoyn_busattachment_setdaemondebug(handle, "ALL", 7);
#endif
            return handle;
        }
        private static IntPtr CreateConcurrencyHandle(string busName, bool allowRemoteMessages, UInt32 concurrency)
        {
            var handle = alljoyn_busattachment_create_concurrency(busName, allowRemoteMessages.ToQccBool(), concurrency);
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Could not create bus attachment");
#if DEBUG
            alljoyn_busattachment_setdaemondebug(handle, "ALL", 7);
#endif
            return handle;
        }

        public BusAttachment() : this(GenerateBusName(), true)
        {
        }

        public BusAttachment(string busName, bool allowRemoteMessages) : base(CreateHandle(busName, allowRemoteMessages))
        {
            _busName = busName;
            _allowRemoteMessages = allowRemoteMessages;
            _sBusAttachmentMap.Add(Handle, new WeakReference<BusAttachment>(this));
        }
        public BusAttachment(string busName, bool allowRemoteMessages, UInt32 concurrency) : base(CreateConcurrencyHandle(busName, allowRemoteMessages, concurrency))
        {
            _busName = busName;
            _allowRemoteMessages = allowRemoteMessages;
            _sBusAttachmentMap.Add(Handle, new WeakReference<BusAttachment>(this));
        }

        protected override void Dispose(bool disposing)
        {
            if (IsStarted && !IsStopping)
                Stop();
            if (IsStopping)
                Join();
            if (Handle != IntPtr.Zero)
                alljoyn_busattachment_destroy(Handle);
            base.Dispose(disposing);
        }

        #endregion

        public string UniqueName
        {
            get
            {
                if (Handle == IntPtr.Zero)
                    throw new InvalidOperationException("Bus is not connected");
                var p = alljoyn_busattachment_getuniquename(Handle);
                return Marshal.PtrToStringAnsi(p);
            }
        }

        #region Bus Attachment Lifecycle

        public void Start()
        {
            var result = alljoyn_busattachment_start(Handle);
            if (result != 0)
                throw new AllJoynException(result, "Failed to start bus attachment.");
        }

        public void Stop()
        {
            var result = alljoyn_busattachment_stop(Handle);
            if (result != 0)
                throw new AllJoynException(result, "Failed to stop bus attachment.");
        }

        public void Join()
        {
            var status = alljoyn_busattachment_join(Handle);
            if (status != 0)
                throw new AllJoynException(status); //, "Failed to join bus attachment.");
        }

        public void JoinSession(string sessionHost, UInt16 sessionPort)
        {
            //TODO: Missing parameters
            var status = alljoyn_busattachment_joinsession(Handle, sessionHost, sessionPort, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (status != 0)
                throw new AllJoynException(status);

        }

        public void Connect(string connectSpec = null)
        {
            var result = alljoyn_busattachment_connect(Handle, connectSpec);
            if (result != 0)
                throw new AllJoynException(result); //, "Failed to connect bus attachment.");
            Debug.WriteLine($"BusAttachment connect succeeded. Bus name = {_busName}");
        }
        
        public void Disconnect()
        {
            var result = alljoyn_busattachment_disconnect(Handle, "");
            if (result != 0)
                throw new AllJoynException(result, "Failed to disconnect bus attachment");
        }

        public bool IsStarted
        {
            get
            {
                return (alljoyn_busattachment_isstarted(Handle) == 1);
            }
        }

        public bool IsStopping
        {
            get
            {
                return (alljoyn_busattachment_isstopping(Handle) == 1);
            }
        }

        public bool IsConnected
        {
            get
            {
                return (alljoyn_busattachment_isconnected(Handle) == 1);
            }
        }

        public UInt32 Concurrency
        {
            get
            {
                return alljoyn_busattachment_getconcurrency(Handle);
            }
        }

        public string ConnectionSpec
        {
            get
            {
                IntPtr str = alljoyn_busattachment_getconnectspec(Handle);
                return Marshal.PtrToStringAnsi(str);
            }
        }

        public void EnableConcurrentCallbacks()
        {
            alljoyn_busattachment_enableconcurrentcallbacks(Handle);
        }
        #endregion

        #region Interfaces

        public void CreateInterface(string name, InterfaceDescription iface)
        {
            var status = alljoyn_busattachment_createinterface(Handle, name, iface.Handle);
            if (status != 0)
                throw new AllJoynException(status, "Failed to create interface");
        }

        public void CreateInterfaceSecure(string name, InterfaceDescription iface, InterfaceDescription.SecurityPolicy policy)
        {
            var status = alljoyn_busattachment_createinterface_secure(Handle, name, iface.Handle, (alljoyn_interfacedescription_securitypolicy)policy);
            if (status != 0)
                throw new AllJoynException(status, "Failed to create interface");
        }

        public void CreateInterfacesFromXml(string xml)
        {
            var status = alljoyn_busattachment_createinterfacesfromxml(Handle, xml);
            if (status != 0)
                throw new AllJoynException(status, "Failed to create interface");
        }

        public InterfaceDescription GetInterface(string name)
        {
            var handle = alljoyn_busattachment_getinterface(Handle, name);
            return InterfaceDescription.Create(handle);
        }

        public InterfaceDescription[] GetInterfaces()
        {
            ulong numIfaces = (ulong)alljoyn_busattachment_getinterfaces(Handle, IntPtr.Zero,  UIntPtr.Zero);
            IntPtr[] ifaces = new IntPtr[(int)numIfaces];
            GCHandle gch = GCHandle.Alloc(ifaces, GCHandleType.Pinned);
            ulong numIfacesFilled = (ulong)alljoyn_busattachment_getinterfaces(Handle,
                gch.AddrOfPinnedObject(), (UIntPtr)numIfaces);
            gch.Free();
            if (numIfaces != numIfacesFilled)
            {
                // Warn? 
            }
            InterfaceDescription[] ret = new InterfaceDescription[(int)numIfacesFilled];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new InterfaceDescription(ifaces[i]);
            }
            return ret;
        }

        public void WhoImplementsInterfaces(string[] interfaces)
        {
            var status = alljoyn_busattachment_whoimplements_interfaces(Handle, interfaces, (UIntPtr)interfaces.Length);
            if (status > 0)
                throw new AllJoynException(status);
        }

        #endregion
        public void RegisterBusObject(BusObject busObject)
        {
            var status = alljoyn_busattachment_registerbusobject(Handle, busObject.Handle);
        }
        public void UnregisterBusObject(BusObject busObject)
        {
            alljoyn_busattachment_unregisterbusobject(Handle, busObject.Handle);
        }

        public void RegisterBusObjectSecure(BusObject busObject)
        {
            var status = alljoyn_busattachment_registerbusobject_secure(Handle, busObject.Handle);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void RequestName(string requestedName, uint flags)
        {
            var status = alljoyn_busattachment_requestname(Handle, requestedName, flags);
            if (status > 0)
                throw new AllJoynException(status);
        }

        //TODO: These listeners should probable just be event handlers instead
        public void RegisterBusListener(BusListener listener)
        {
            alljoyn_busattachment_registerbuslistener(Handle, listener.Handle);
        }
        public void UnregisterBusListener(BusListener listener)
        {
            alljoyn_busattachment_unregisterbuslistener(Handle, listener.Handle);
        }

        public void RegisterAboutListener(AboutListener listener)
        {
            alljoyn_busattachment_registeraboutlistener(Handle, listener.Handle);
        }

        public void UnregisterAboutListener(AboutListener listener)
        {
            alljoyn_busattachment_unregisteraboutlistener(Handle, listener.Handle);
        }

        public void FindAdvertisedName(string namePrefix)
        {
            var status = alljoyn_busattachment_findadvertisedname(Handle, namePrefix);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void CancelFindAdvertisedName(string namePrefix)
        {
            var status = alljoyn_busattachment_cancelfindadvertisedname(Handle, namePrefix);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void FindAdvertisedNameByTransport(string namePrefix, ushort transports)
        {
            var status = alljoyn_busattachment_findadvertisednamebytransport(Handle, namePrefix, transports);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void CancelFindAdvertisedNameByTransport(string namePrefix, ushort transports)
        {
            var status = alljoyn_busattachment_cancelfindadvertisednamebytransport(Handle, namePrefix, transports);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void AdvertiseName(string namePrefix, ushort transports)
        {
            var status = alljoyn_busattachment_advertisename(Handle, namePrefix, transports);
            if (status > 0)
                throw new AllJoynException(status);
        }

        public void CancelAdvertiseName(string namePrefix, ushort transports)
        {
            var status = alljoyn_busattachment_canceladvertisename(Handle, namePrefix, transports);
            if (status > 0)
                throw new AllJoynException(status);
        }

        private static string GenerateBusName()
        {
#if NETFX_CORE
            string name  = Windows.ApplicationModel.Package.Current.Id.FamilyName;
#else
            //TODO: Xamarin

            var assy = Assembly.GetEntryAssembly();
            if (assy == null)
                assy = Assembly.GetCallingAssembly();

            string name = assy.GetName().Name;
#endif
            return System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9-.]+", "");
        }
    }
}