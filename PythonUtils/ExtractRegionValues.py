import sys
import io

input_file = "C:\\test_py\\bo.h"
output_file = "C:\\test_py\NativeEnums.cs"

def read_header(file_name):
	print(file_name)
	f = open(file_name, 'r+')
	print(f)

def write_csharp_file():
	print("Writing File.")

def main():
	read_header(input_file)
	write_csharp_file()


if __name__ == '__main__':
	main()


