# coding: Shift_JIS

import re
import os
import os.path
import sys
import array

# strings
rep_ary = [
	[' Sce.Pss.', ' Sce.PlayStation.'],
	['"Sce.Pss.', '"Sce.PlayStation.'],
	['\Sce.Pss.', '\Sce.Psm.']
]

# arg ...
argvs = sys.argv
argc = len(argvs)

# usage
if argc != 2:
	print 'usage:'
	print '> python.exe pss2psm_rename_tool.py [directory path]'
	quit()

# replace string in file
def replace_str(fname):
	tname = fname + '.rep_tmp';
	work = False
	try:
		print fname
		read_file = open(fname, 'r')
		write_file = open(tname, 'w')
		for line in read_file:
			for rep in rep_ary:
				if line.find(rep[0]) != -1:
					line = line.replace(rep[0], rep[1])
					work = True
			write_file.write(line)
	finally:
		read_file.close()
		write_file.close()

	if work:
		if os.path.isfile(tname) and os.path.isfile(fname):
			os.remove(fname)
			os.rename(tname, fname)
			print 'Replaced.'
	else:
		os.remove(tname)
		print 'Skip.'

# check file
def check_file(fname):
	path, ext = os.path.splitext(fname)
	if ext == '.cs' or ext == '.csproj':
		replace_str(fname)

# search file in directory
def search_dir(dname):
	if os.path.isdir(dname):
		files = os.listdir(dname)
		for file in files:
			fname  = dname + '\\' + file
			if os.path.isfile(fname):
				check_file(fname)
			else:
				search_dir(fname)

# main
dir_name = argvs[1]
search_dir(dir_name)

