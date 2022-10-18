using iMobileDevice;
using iMobileDevice.Afc;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using iMobileDevice.Plist;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace imobileDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NativeLibraries.Load();

            ReadOnlyCollection<string> udids;

            int count = 0;

            var afc = LibiMobileDevice.Instance.Afc;
            var idevice = LibiMobileDevice.Instance.iDevice;

            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var ret = idevice.idevice_get_device_list(out udids, ref count);



            if (ret == iDeviceError.NoDevice)
            {
                // Not actually an error in our case
                return;
            }

            ret.ThrowOnError();

            // Get the device name
            foreach (var udid in udids)
            {
                iDeviceHandle deviceHandle;
                idevice.idevice_new(out deviceHandle, udid).ThrowOnError();

                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "Quamotion").ThrowOnError();


                string deviceName;

                PlistHandle tested1;
                PlistHandle tested2;
                PlistHandle tested3;
                string SerialNumber;
                string ProductVersion;
                string imei;

                lockdown.lockdownd_get_value(lockdownHandle, null, "SerialNumber", out tested1);
                lockdown.lockdownd_get_value(lockdownHandle, null, "ProductVersion", out tested2);
                lockdown.lockdownd_get_value(lockdownHandle, null, "IMEI", out tested3);
                // lockdown.lockdownd_get_value(lockdownHandle, "com.apple.disk_usage", "TotalDiskCapacity", out tested3);

                lockdown.lockdownd_get_value(lockdownHandle, null, "InternationalMobileEquipmentIdentity", out tested3);


                tested1.Api.Plist.plist_get_string_val(tested1, out SerialNumber);
                tested2.Api.Plist.plist_get_string_val(tested2, out ProductVersion);
                tested3.Api.Plist.plist_get_string_val(tested3, out imei);
                // tested3.Api.Plist.plist_get_string_val(tested3, out t3);

                lockdown.lockdownd_get_device_name(lockdownHandle, out deviceName).ThrowOnError();

                LockdownServiceDescriptorHandle ldsHandle;
                AfcClientHandle afcClient;
                string TotalDiskCapacity;


                lockdown.lockdownd_start_service(lockdownHandle, "com.apple.afc", out ldsHandle);

                ldsHandle.Api.Afc.afc_client_new(deviceHandle, ldsHandle, out afcClient);


                ldsHandle.Api.Afc.afc_get_device_info_key(afcClient, "FSTotalBytes", out TotalDiskCapacity);


                deviceHandle.Dispose();
                lockdownHandle.Dispose();


            }
        }
    }
}
