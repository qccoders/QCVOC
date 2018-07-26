# Quad City Veterans Outreach Center Android App

## Getting Started:
Open in android studio and build.

## Debugging with Wireless Devices:
[Source: Stack Overflow](https://stackoverflow.com/questions/4893953/run-install-debug-android-applications-over-wi-fi)

Make sure the `adb` service is started.

Connect device via usb and ensure debugging works.

Listen locally on port 5555:
```bash
adb tcpip 5555
```

Get the IP address of your device:
```bash
adb shell
adb ip a
```

Connect to your device:
```bash
adb connect <DEVICE_IP_ADDRESS>:5555
```

Disconnect USB and debug wirelessly.

**Do not forget to turn this off when you are finished for security purposes:**
```bash
adb -s <DEVICE_IP_ADDRESS>:555 usb
```