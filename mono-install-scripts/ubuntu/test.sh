#!/bin/bash -e

PACKAGES=("mono-2.10.1"
"libgdiplus-2.10"
"gtk-sharp-2.12.10"
"xsp-2.10"
"mod_mono-2.10")

URLS=("http://ftp.novell.com/pub/mono/sources/mono/mono-2.10.1.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/gtk-sharp212/gtk-sharp-2.12.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.10.tar.bz2")

# PACKAGEFILES=(["mono-2.10.1"]="http://ftp.novell.com/pub/mono/sources/mono/mono-2.10.1.tar.bz2"
# ["libgdiplus-2.10"]="http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.10.tar.bz2"
# ["gtk-sharp-2.12.10"]="http://ftp.novell.com/pub/mono/sources/gtk-sharp212/gtk-sharp-2.12.10.tar.bz2"
# ["xsp-2.10"]="http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.10.tar.bz2"
# ["mod_mono-2.10"]="http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.10.tar.bz2")

count=${#PACKAGES[@]}
index=0
while [ "$index" -lt "$count" ]
do
	echo -n "${PACKAGES[$index]} == "
	echo "${URLS[@]:$index:1}"
	let "index = $index + 1"
done

echo
