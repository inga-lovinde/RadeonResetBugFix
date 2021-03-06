# RadeonResetBugFixService
Radeon Reset Bug fix service

## Use case

You have configured pass-through of Radeon GPU into Windows Guest VM,
intending to use it as the primary GPU,
but whenever this VM reboots, the entire host system hangs
and you have to power cycle the entire system, losing all data.

You have attempted solving this by various fixes found in the internet
(startup/shutdown scripts in group policies, etc),
but did not find them reliable.
And even then, after applying these fixes,
you find out that virtual GPU is now the primary display adapter,
GPU acceleration is unavailable,
and the screen connected to Radeon GPU is treated as the secondary screen.

This service intends to solve all the above problems.
With it, you will be able to use Radeon GPU as your only GPU,
with your actual display connected to Radeon as a primary display,
and reboot your VM without triggering AMD reset bug - even installing Windows updates!

In effect, you can use VM with pass-through GPU plus pass-through keyboard and mouse
as entirely separate PC.

## Limitations

Currently this project is only tested with Windows 10 VMs on Hyper-V,
and was not tested with Windows 7 VMs, KVM, QEMU, VMWare and VirtualBox,
although the preliminary support for these systems has been implemented
and they should probably work too.

Bug reports are welcome!

**Note that you will still have to add a virtual GPU to your VM,
otherwise Windows won't boot.**

Note that it will add around 1 minute or up to 5 minutes in some cases to startup time,
and a fraction of minute or up to several minutes in some cases to shutdown time.
So don't panic if your screen is black immediately after VM startup,
it is expected.

## Install instructions

Make sure you have added an active virtual/synthetic GPU to your VM,
and that you have installed all the relevant drivers / Guest Additions.

Put `RadeonResetBugFixService.exe` in a permanent location.

In elevated command prompt in Guest VM, run

```
RadeonResetBugFixService.exe install
```

The screen may go blank several times during the process.
It may take up to 15 minutes total (but should take less than 5).

Do not remove the file after that, or the service won't be able to start or stop.
The `install` command does not create any copies, does not create a folder in `Program Files`,
it simply adds a service to Windows, but the service refers to the `exe` file you invoked.

## Upgrade instructions

In elevated command prompt in Guest VM, run

```
RadeonResetBugFixService.exe reinstall
```

The screen may go blank several times during the process.
It may take up to 20 minutes total (but should take less than 5).

## Uninstall instructions

In elevated command prompt in Guest VM, run

```
RadeonResetBugFixService.exe uninstall
```

The screen may go blank several times during the process.
It may take up to 5 minutes total (but should take several seconds).

## Debugging, bug reports

The service stores its verbose log files in `logs` directory located next to the executable.

If the service does not work correctly on your system or does not solve the reset bug,
please also run the diagnostics command
(which will gather the detailed information about your hardware and will also write it to log):

```
RadeonResetBugFixService.exe diagnose
```

Attach to your bug report the diagnose log file plus the relevant recent service logs:
one from the latest startup when you encountered a problem,
and another one from the startup before that.

## Known issues

### Connecting to VM from the Host

This service disables virtual video adapter,
so you can no longer connect to VM from a host, using Basic sessions.
It will only work during startup / shutdown, but not during normal usage.
If you connected to VM from a host during startup,
the screen will go blank in a couple of minutes
(as the display connected to Radeon GPU wil light up),
that is expected.

That is one of the points of this service;
otherwise, virtual display would always be present as primary or secondary display,
even when you are not connected to VM from the host.

Enhanced sessions in Hyper-V (which use RDP protocol) continue to work fine.
Other hypervisors can miss the enhanced sessions feature,
but you still can connect to VM using RDP.

### Unsuccessful reboots

This problem may **rarely** occur after updating Windows in Guest VM.
It never happened for me during the ordinary usage.

If, after reboot, Guest VM for some reason did not recognize Radeon GPU
(the screen connected to Radeon GPU remains inactive),
or is unresponsive,
**DO NOT** force-reboot Guest VM, or your Host system will hang up,
and you will have to power cycle the entire system, losing all unsaved data.

Instead, gracefully reboot your Host (gracefully shutting down all guests).

If, after that, Guest VM will continue to behave oddly
(using your actual display connected to Radeon GPU as the secondary one),
just reboot the Guest VM gracefully.

Alternative (but more difficult) option: connect to VM from host,
and restart the "Radeon reset bug fix" service.
That way, you won't need to reboot the host;
however, you have to be able to open graphic VM terminal sessions.

The cause of this problem:

Sometimes, while installing updates which require restart,
Windows reboots not once but twice:
first time from UI into "installing updates, step X of Y" screen,
and second time from this screen back into UI.

For some reason, on that "installing updates" step, Windows starts all the services,
but does not invoke pre-shutdown sequence,
so the service ends up in an inconsistent state
(because not everything could be done in shutdown sequence).

### Force-rebooting Guest VM

If, for some reason, you find that you need to force reboot Guest VM
(e.g. if you ran some program that made it unresponsive),
**DO NOT DO IT**.
This will prevent the service from shutting down GPU gracefully,
and you will encounter the same old Radeon reset bug,
which will force you to power cycle the entire host system.

Instead, if you are unable to make Guest VM response to your actions,
reboot **the host system** gracefully.

You may need to gracefully reboot Guest VM again after that,
similar to the previous example.

There is no simple way to shut down GPU gracefully
when it is connected to the unresponsive Guest VM.

## How it works

The extremely simplified description is as follows:

On service start (startup):

* Re-enable "basic video" system service automatic startup
(so that in case of unexpected reboot, Windows will have at least one working (virtual) display adapter,
otherwise Windows will not boot, and the only way to fix it is to boot into Recovery Console,
and re-enable "basic video" system service automatic startup manually using `regedit`)

* Enable Radeon GPU

* Now that it's not the only GPU, disable virtual GPU

On service stop (pre-shutdown / shutdown):

* Stop Windows Audio service
(otherwise it won't let us disable Radeon devices)

* Enable virtual GPU

* Now that it's not the only GPU, disable Radeon GPU

* Disable "basic video" system service automatic startup

Plus a bunch of magic to ensure that:

* After every graceful shutdown Radeon GPU is turned off whenever possible,
so that Radeon reset bug won't occur on subsequent startup
during the same host uptime session (again, whenever possible);

* At every startup, Windows has at least one enabled GPU driver/service
(otherwise it won't get through the kernel boot sequence);

* At every attempt to disable a GPU, it is not the only available GPU
(otherwise Windows won't let us disable it).
