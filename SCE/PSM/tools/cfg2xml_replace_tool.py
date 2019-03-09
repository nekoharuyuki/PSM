# coding: Shift_JIS

import re
import os
import os.path
import sys
import array

# strings
xml_txt = ""\
"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"\
"<application project_name=\"[project_name]\" version=\"1.00\" default_locale=\"en-US\">\n"\
"	<runtime_config>\n"\
"		<memory managed_heap_size=\"[managed_heap_size]\" resource_heap_size=\"[resource_heap_size]\" />\n"\
"	</runtime_config>\n"\
"	<feature_list>\n"\
"[feature_list]"\
"	</feature_list>\n"\
"</application>\n"\
""

# arg ...
argvs = sys.argv
argc = len(argvs)

# usage
if argc != 2:
	print 'usage:'
	print '> python.exe cfg2xml_replace_tool.py [directory path]'
	quit()

# rename and edit app.xml
def rename_cfg(fname, aname):
	tname = fname.replace('app.cfg', 'app.xml');
	try:
		print fname
		read_file = open(fname, 'r')
		write_file = open(tname, 'w')
		project_name = aname
		managed_heap_size = '32768'
		resource_heap_size = '65536'
		feature_list = ''
		for line in read_file:
			if line.find(':') != -1:
				key, value = line.split(':')
				key =  key.strip()
				value =  value.strip()
				if key == 'resource_heap_size':
					resource_heap_size = value
				if key == 'managed_heap_size':
					managed_heap_size = value
				if key == 'gamepad' and value == 'true':
					feature_list += '		<feature value=\"GamePad\" />\n'
				if key == 'touch' and value == 'true':
					feature_list += '		<feature value=\"Touch\" />\n'
				if key == 'motion' and value == 'true':
					feature_list += '		<feature value=\"Motion\" />\n'
		write_text = xml_txt
		write_text = write_text.replace('[project_name]', project_name)
		write_text = write_text.replace('[resource_heap_size]', resource_heap_size)
		write_text = write_text.replace('[managed_heap_size]', managed_heap_size)
		write_text = write_text.replace('[feature_list]', feature_list)
		write_file.write(write_text)
	finally:
		read_file.close()
		write_file.close()

	if os.path.isfile(fname):
		bname = fname + '.bak'
		if os.path.isfile(bname):
			os.remove(bname)
		os.rename(fname, bname)
		print 'Renamed.'

# replace string in file
def replace_str(fname):
	tname = fname + '.rep_tmp';
	work = False
	try:
		print fname
		read_file = open(fname, 'r')
		write_file = open(tname, 'w')
		aname = ""
		aname_tmp = ""
		aname_ok = False
		rep_con = False
		for line in read_file:
			if line.find('<PropertyGroup>') != -1 or line.find('</PropertyGroup>') != -1:
				if aname_ok:
					aname = aname_tmp
			if line.find('<OutputType>Exe</OutputType>') != -1:
				aname_ok = True
			if line.find('<AssemblyName>') != -1:
				aname_tmp = line.replace('<AssemblyName>', '').replace('</AssemblyName>', '').strip()
			if line.find('<Content ') != -1 and line.find('Include="app.cfg"') != -1:
				line = line.replace('Content', 'PsmMetadata')
				if line.find('/>') == -1:
					rep_con = True
			if rep_con and line.find('</Content>') != -1:
				line = line.replace('Content', 'PsmMetadata')
				rep_con = False
			if line.find('app.cfg') != -1:
				line = line.replace('app.cfg', 'app.xml')
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

	return aname

# check file
def check_file(fname):
	path, ext = os.path.splitext(fname)
	if ext == '.csproj':
		aname = replace_str(fname)

		cname = os.path.dirname(fname) + '\\app.cfg'
		if os.path.isfile(cname):
			rename_cfg(cname, aname)

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

