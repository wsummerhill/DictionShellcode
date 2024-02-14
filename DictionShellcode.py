import sys
import random
import argparse
import pyperclip 	# clipboard for all OS's
import re			# regex

# Get English wordlist
with open('english-words.txt') as f:
    english_words = [line.rstrip('\n') for line in f]

# Create unique list of 256 dictionary words
dictionary = random.sample(english_words, 256)

################################################
#      		     MAIN     	       	       #
################################################
def main(args=sys.argv[1:]):

	# Instantiate the argument parser
	parser = argparse.ArgumentParser(description='Shellcode converter to Dictionary list')
	parser.add_argument('-file', '-f', help="Raw binary shellcode file for input")
	parser.add_argument('-lang', '-l', help="Output language format", choices=['cs','cpp'])
	parser.add_argument('-outfile', '-o', help="OPTIONAL: File output with encoded dictionary words separated by newlines", required=False)
	args = parser.parse_args(args)

	shellcode_file = args.file
	output_lang = args.lang
	outfile = args.outfile

	if not shellcode_file:
		print("[-] ERROR! Missing input shellcode file parameter '-f'")
		print("[-] Enter '-h' for help menu")
		sys.exit()
	elif not output_lang:
		print("[-] ERROR! Missing output language type parameter '-l'")
		print("[-] Enter '-h' for help menu")
		sys.exit()


	# Build dictionary translation from list
	hex_word_dict = assign_hex_to_words(dictionary)

	# Output variable placeholder for shellcode dictionary list
	if output_lang == 'cpp':
		translation_var = "const char* translate_dict[256] = { "
	else: #cpp
		translation_var = "public static string[] translate_dict = new string[256] { "

	# Loop through Dictionary wordlist to build variable
	for hex_value, word in hex_word_dict.items():
	    translation_var += f'"{word}",'
	
	# Cleanup output dictionary variable
	translation_var = translation_var[:-1]
	translation_var += " };"

	# Read shellcode
	shellcode = getRawShellcode(shellcode_file)

	# Get hex format of shellcode
	shellcode_hex = getHexShellcode(shellcode)
	shellcode_len = len(shellcode_hex.split(','))
	
	# Output variable placeholder for converted shellcode to dictionary words
	if output_lang == 'cpp':
		translation_shellcode = "const char* dict_words[XYZ] = { "
	else: #cpp
		translation_shellcode = "string[] dict_words = new string[XYZ] { "

	lineCount = 0
	file_output = ""

	# Loop through shellcode hex bytes to assign their equivalent dictionary-byte
	for hex_byte in shellcode_hex.split(','):
		convert_int = int(hex_byte, 16) # Get position of int within dictionary
		translation_shellcode += '"' + dictionary[convert_int] + '",' 
		
		# Only print max 30 words per line
		if lineCount == 30:
			translation_shellcode += '\n'
			lineCount = 0
		lineCount += 1
	# end for

	# Cleanup output shellcode variable
	translation_shellcode = translation_shellcode[:-1]
	translation_shellcode += " };"
	translation_shellcode = translation_shellcode.replace('XYZ', str(shellcode_len)) #Add length to shellcode var


	################ Output ################

	print('[+] Shellcode Dictionary (256-bytes):\n')
	print(translation_var + '\n')

	print(f'[+] Shellcode length: {shellcode_len}')

	# Copy Dictionary-encoded shellcode to Clipboard
	pyperclip.copy(translation_shellcode)
	print('[+] Converted shellcode-to-dictionary variable copied to Clipboard!')

	# Optionally save encoded output into file with dictionary words split on newlines
	if outfile:
		printOutputToFile(translation_shellcode, outfile)
		print(f'[+] Converted shellcode-to-dictionary written to output file "{outfile}".\n')


################################################
#          	     FUNCIONS 	      	       #
################################################

# Read shellcode bytes from file
def getRawShellcode(filePath):
	# RAW shellcode
	try:
		with open(filePath, 'rb') as shellcode_file:
			file_shellcode = shellcode_file.read()
			return file_shellcode
	except FileNotFoundError:
		exit("\nThe input file was not found. Exiting...\n")


# Get shellcode in Hex format (0x00,0x01,0x02...)
def getHexShellcode(shellcode):
	output = ""
	for byte in bytearray(shellcode):
		output += '0x'
		output += '%02x,' % byte
	return output[:-1] #remove last , character at the end


# Read dictionary list of 256-length and assign each word to an integer value (from 0 to 255)
def assign_hex_to_words(dictionary):
    word_dict = {}
    for i in range(256):
        hex_value = f"0x{i:02X}"
        word_dict[hex_value] = dictionary[i] if i < len(dictionary) else 'Unknown'
    return word_dict


# Regex extract dictionary words from "translation_shellcode" variable and write to output file
def printOutputToFile(encodedShellcode, outfile):
	regex = r'"(.*?)"'
	matches = re.findall(regex, encodedShellcode, flags=re.IGNORECASE)
	
	# Write encoded dictionary words to file
	with open(outfile, "w") as out_file:
		for i, item in enumerate(matches):
			out_file.write(item)
			if i < len(matches) - 1:
				out_file.write('\n')


if __name__ == '__main__':
    main()
