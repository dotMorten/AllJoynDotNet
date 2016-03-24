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

        private IntPtr _aboutListenerCallback;
        private readonly string _busName;
        private readonly bool _allowRemoteMessages;

        static BusAttachment()
        {
            Init.Initialize();
        }

        private static IntPtr CreateHandle(string busName, bool allowRemoteMessages)
        {
            var handle = alljoyn_busattachment_create(busName, allowRemoteMessages.ToQccBool());
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Could not create bus attachment");
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

        protected override void Dispose(bool disposing)
        {
            Disconnect();
            if (Handle != IntPtr.Zero)
                alljoyn_busattachment_destroy(Handle);
            base.Dispose(disposing);
        }

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

        public void Start()
        {
            var result = alljoyn_busattachment_start(Handle);
            if (result != 0)
                throw new AllJoynException(result, "Failed to start bus attachment.");
        }
        public void Connect(string connectSpec = null)
        {
            var result = alljoyn_busattachment_connect(Handle, connectSpec);
            if (result != 0)
                throw new AllJoynException(result, "Failed to connect bus attachment.");
            Debug.WriteLine($"BusAttachment connect succeeded. Bus name = {_busName}");
        }
        public void CreateInterfacesFromXml(string xml)
        {
            var result = alljoyn_busattachment_createinterfacesfromxml(Handle, xml);
            if (result != 0)
                throw new AllJoynException(result, "Failed to create interface");
        }
        public InterfaceDescription GetInterface(string name)
        {
            var handle = alljoyn_busattachment_getinterface(Handle, name);
            return InterfaceDescription.Create(handle);
        }

        public InterfaceDescription[] GetInterfaces()
        {
            ulong numIfaces = alljoyn_busattachment_getinterfaces(Handle, IntPtr.Zero, 0);
            IntPtr[] ifaces = new IntPtr[(int)numIfaces];
            GCHandle gch = GCHandle.Alloc(ifaces, GCHandleType.Pinned);
            ulong numIfacesFilled = alljoyn_busattachment_getinterfaces(Handle,
                gch.AddrOfPinnedObject(), numIfaces);
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


        public void RegisterAboutListener(AboutListener listener)
        {
            //alljoyn_aboutlistener_create()
            alljoyn_busattachment_registeraboutlistener(Handle, listener.Handle);



            //alljoyn_busobject altObj =
            //create_about_obj_test_bus_object(serviceBus, "/org/test/about", ifaceName);
            //
            //result = AllJoynNative.alljoyn_busattachment_registerbusobject(_busAttachmentHandle, obj);

            //var callback = new alljoyn_aboutlistener_callback(ListenerCallback);
            //var callback = new CallbackHandler();
            //_aboutListenerCallback = AllJoynNative.alljoyn_aboutlistener_create(ListenerCallback, IntPtr.Zero);
            //AllJoynNative.alljoyn_busattachment_registeraboutlistener(_busAttachmentHandle, _aboutListenerCallback);
            //AllJoynNative.alljoyn_busattachment_whoimplements_interfaces(_busAttachmentHandle, null, 0);

        }

        public void WhoImplementsInterfaces(string[] interfaces)
        {
            var result = alljoyn_busattachment_whoimplements_interfaces(Handle, interfaces, (ulong)interfaces.Length);
            if (result > 0)
                throw new AllJoynException(result);
        }

        public void Disconnect()
        {
            if (Handle != IntPtr.Zero)
            {
                var result = alljoyn_busattachment_disconnect(Handle, null);
                if (result != 0)
                    throw new Exception($"Failed to disconnect bus attachment. Error code={result}");
                //Handle = IntPtr.Zero;
            }
            if (_aboutListenerCallback != IntPtr.Zero)
            {
                //alljoyn_aboutlistener_destroy(_aboutListenerCallback);
                _aboutListenerCallback = IntPtr.Zero;
            }
        }

        private static string GenerateBusName()
        {
#if NETFX_CORE
            string name  = Windows.ApplicationModel.Package.Current.Id.FamilyName;
#else
            var assy = Assembly.GetEntryAssembly();
            if (assy == null)
                assy = Assembly.GetCallingAssembly();

            string name = assy.GetName().Name;
#endif
            return System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9-.]+", "");
        }

        private static void ListenerCallback(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPStr)] string busName,
            UInt16 version,
            UInt16 port, //alljoyn_sessionport 
            IntPtr objectDescriptionArg, //alljoyn_msgarg
            IntPtr aboutDataArg //alljoyn_msgarg
        )
        {
            Debug.WriteLine("Listener Callback arrived");
        }


    }
}