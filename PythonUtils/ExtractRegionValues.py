#I cant even remember what I wanted this file to do.
#Extract all the #defines and create C# enums.
#make this a web page. make all #defines selectable/de-selectable.
#allow resulting enums to be named.

import sys
import io

input_file = "C:\\test_py\\bo.h"
output_file = "C:\\test_py\\NativeEnums.cs"

def read_header(file_name):
	print(file_name)
	f = open(file_name, 'r+')
	print(f)

def write_csharp_file():
	print("Writing File.")

def main():
	read_header(input_file)
	write_csharp_file()


def parse_file():
	

if __name__ == '__main__':
	main()
