using System;
using System.Diagnostics;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Modules;

namespace UnityInjector2
{
    static class Program
    {
        const string TargetProcessName = "warmode"; // WARMODE: http://store.steampowered.com/app/391460/
        const string DesktopMonoDll = "mono.dll";
        //const string WebMonoDll = "mono-1-vc.dll";

        static void GetMonoRemoteFunctions(MemorySharp memorySharp, out RemoteFunction mono_get_root_domain, out RemoteFunction mono_thread_attach, out RemoteFunction mono_security_set_mode, out RemoteFunction mono_domain_get, out RemoteFunction mono_domain_assembly_open, out RemoteFunction mono_assembly_get_image, out RemoteFunction mono_class_from_name, out RemoteFunction mono_class_get_method_from_name, out RemoteFunction mono_runtime_invoke)
        {
            Console.ForegroundColor = ConsoleColor.White;
            mono_get_root_domain = memorySharp[DesktopMonoDll].FindFunction("mono_get_root_domain"); // Returns MonoDomain.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_get_root_domain.Name, mono_get_root_domain.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_thread_attach = memorySharp[DesktopMonoDll].FindFunction("mono_thread_attach"); // Returns MonoThread.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_thread_attach.Name, mono_thread_attach.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_security_set_mode = memorySharp[DesktopMonoDll].FindFunction("mono_security_set_mode"); // Returns void.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_security_set_mode.Name, mono_security_set_mode.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_domain_get = memorySharp[DesktopMonoDll].FindFunction("mono_domain_get"); // Returns MonoDomain.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_domain_get.Name, mono_domain_get.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_domain_assembly_open = memorySharp[DesktopMonoDll].FindFunction("mono_domain_assembly_open"); // Returns MonoAssembly.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_domain_assembly_open.Name, mono_domain_assembly_open.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_assembly_get_image = memorySharp[DesktopMonoDll].FindFunction("mono_assembly_get_image"); // Returns MonoImage.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_assembly_get_image.Name, mono_assembly_get_image.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_class_from_name = memorySharp[DesktopMonoDll].FindFunction("mono_class_from_name"); // Returns MonoClass.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_class_from_name.Name, mono_class_from_name.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_class_get_method_from_name = memorySharp[DesktopMonoDll].FindFunction("mono_class_get_method_from_name"); // Returns MonoMethod.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_class_get_method_from_name.Name, mono_class_get_method_from_name.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
            mono_runtime_invoke = memorySharp[DesktopMonoDll].FindFunction("mono_runtime_invoke"); // Returns MonoObject.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Function \"{0}\" found at 0x{1} address", mono_runtime_invoke.Name, mono_runtime_invoke.BaseAddress.ToString("X2"));
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void AttachToMainThread(RemoteFunction mono_get_root_domain, RemoteFunction mono_thread_attach)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Attaching to/Entering the primary/main mono thread/context... ");
            IntPtr mono_get_root_domain_result = mono_get_root_domain.Execute(CallingConventions.Cdecl); // Returns MonoDomain.
            IntPtr mono_thread_attach_result = mono_thread_attach.Execute(CallingConventions.Cdecl, mono_get_root_domain_result); // Returns MonoThread.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} returned {1} (0x{2})", mono_get_root_domain.Name, mono_get_root_domain_result, mono_get_root_domain_result.ToString("X2"));
            Console.WriteLine("{0} returned {1} (0x{2})", mono_thread_attach.Name, mono_thread_attach_result, mono_thread_attach_result.ToString("X2"));
        }

        static void NullMonoSecurity(RemotePointer mono_security_set_mode)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Nulling mono security... ");
            mono_security_set_mode.Execute(CallingConventions.Cdecl, (int)MonoSecurityMode.MONO_SECURITY_MODE_NONE); // void (returns IntPtr.Zero as expected?)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        [STAThread]
        static /*unsafe*/ void Main()
        {
            Console.Clear();
            Console.Title = ".NET Mono Injector for Unity games by UnityInjector Team";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Waiting for {0}.exe process to start", TargetProcessName);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Process p;
            while (true)
            {
                try
                {
                    // Using "dumb" and fixed target process name, keeping it very very simple and straightforward just for now.
                    // Start the game first, wait for loading to complete and then start UnityInjector 2 to avoid injector crash. We don't like to sleep here :zzzZZzzZZZzZzzZz
                    p = Process.GetProcessesByName(TargetProcessName)[0];
                    break;
                }
                catch
                {
                    Console.Write(".");
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}{1} process was found", Environment.NewLine, p.MainModule.FileName);
            Console.ForegroundColor = ConsoleColor.White;
            using (MemorySharp m = new MemorySharp(p.Id))
            {
                /* Using THE following PLAN:
mono_thread_attach(mono_get_root_domain());
mono_security_set_mode(0);
mono_runtime_invoke(
    mono_class_get_method_from_name(
        mono_class_from_name(
            mono_assembly_get_image(
                mono_domain_assembly_open(
                    mono_domain_get(),
                    "full_path_to_payload_file.dll"
                )
            ),
            "namespace",
            "class"
        ),
        "method",
        0
    ),
    0,
    0,
    0
);
                */
                // NOTE: int mono_security_get_mode() function is no longer exported (after security gets nulled, it can't be restored back).
                RemoteFunction mono_get_root_domain;
                RemoteFunction mono_thread_attach;
                RemoteFunction mono_security_set_mode;
                RemoteFunction mono_domain_get;
                RemoteFunction mono_domain_assembly_open;
                RemoteFunction mono_assembly_get_image;
                RemoteFunction mono_class_from_name;
                RemoteFunction mono_class_get_method_from_name;
                RemoteFunction mono_runtime_invoke;
                GetMonoRemoteFunctions(m, out mono_get_root_domain, out mono_thread_attach, out mono_security_set_mode, out mono_domain_get, out mono_domain_assembly_open, out mono_assembly_get_image, out mono_class_from_name, out mono_class_get_method_from_name, out mono_runtime_invoke);

                // Above code runs fine. From this point everything gets dirty, running/executing functions in order as the plan stated above :P

                AttachToMainThread(mono_get_root_domain, mono_thread_attach);

                NullMonoSecurity(mono_security_set_mode);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Injecting a payload...");
                Console.ForegroundColor = ConsoleColor.White;

                // We need to use generic RemotePointer.Execute<T> function instead of RemotePointer.Execute, where T is a return type of each mono function but its .NET representation (which are missing, that is an issue which has arrived)...
                // Above is a list of functions and their return types on unmanaged side. We need their .NET representation or C++/CLI wrapper... Help will be appreciated.

                IntPtr mono_domain_get_result = mono_domain_get.Execute(CallingConventions.Cdecl); // Pointer to MonoDomain? There is no MonoDomain in C# Mono (perhaps it is AppDomain?).
                // Imagination of the code if there was .NET representation for unmanaged types like, MonoDomain which is returned by mono_domain_get function:
                // MonoDomain mono_domain_get_return = mono_domain_get.Execute<MonoDomain>();
                Console.WriteLine("{0} returned {1} (0x{2})", mono_domain_get.Name, mono_domain_get_result, mono_domain_get_result.ToString("X2"));

                IntPtr mono_domain_assembly_open_result = mono_domain_assembly_open.Execute(CallingConventions.Cdecl,
                    mono_domain_get_result,
                    @"C:\Users\User\Documents\Visual Studio 2015\Projects\UnityInjector2\UnityInjector2\References\WarmodePayload.dll" // Update full path on your computer to match macro: @"$(SolutionDir)$(SolutionName)\References\WarmodePayload.dll"
                    //"WarmodePayload.dll" // Only DLL filename can be used if payload DLL is inside the game's EXE folder or registered in GAC; otherwise, full path to payload DLL file have to be specified? That's just OOP guess.
                ); // Pointer to MonoAssembly?
                Console.WriteLine("{0} returned {1} (0x{2})", mono_domain_assembly_open.Name, mono_domain_assembly_open_result, mono_domain_assembly_open_result.ToString("X2"));

                IntPtr mono_assembly_get_image_result = mono_assembly_get_image.Execute(CallingConventions.Cdecl, mono_domain_assembly_open_result); // Pointer to MonoImage?
                Console.WriteLine("{0} returned {1} (0x{2})", mono_assembly_get_image.Name, mono_assembly_get_image_result, mono_assembly_get_image_result.ToString("X2"));

                IntPtr mono_class_from_name_result = mono_class_from_name.Execute(CallingConventions.Cdecl, mono_assembly_get_image_result, "WarmodePayload", "MainPayloadLoader"); // Pointer to MonoClass?
                Console.WriteLine("{0} returned {1} (0x{2})", mono_class_from_name.Name, mono_class_from_name_result, mono_class_from_name_result.ToString("X2"));

                IntPtr mono_class_get_method_from_name_result = mono_class_get_method_from_name.Execute(CallingConventions.Cdecl, mono_class_from_name_result, "OnMainPayloadLoad", 0); // Pointer to MonoMethod?
                Console.WriteLine("{0} returned {1} (0x{2})", mono_class_get_method_from_name.Name, mono_class_get_method_from_name_result, mono_class_get_method_from_name_result.ToString("X2"));

                IntPtr mono_runtime_invoke_result = mono_runtime_invoke.Execute(CallingConventions.Cdecl,
                    mono_class_get_method_from_name_result,
                    0,
                    IntPtr.Zero,
                    IntPtr.Zero
                ); // Pointer to MonoObject?
                Console.WriteLine("{0} returned {1} (0x{2})", mono_runtime_invoke.Name, mono_runtime_invoke_result, mono_runtime_invoke_result.ToString("X2"));
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Payload has been successfully injected into: {0} (0x{1}) {2}:{3} process", p.Id, p.Id.ToString("X2"), p.MainModule.FileName, p.MainWindowTitle);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}