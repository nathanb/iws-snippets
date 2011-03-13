#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-2.10.1

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext-libs autoconf httpd httpd-devel libtool wget libtiff-devel libexif-devel libexif libjpeg-devel gtk2-devel atk-devel pango-devel giflib-devel libglade2-develui-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

PACKAGES=("mono-addins-0.5"
"mono-debugger-2.10"
"mono-tools-2.10"
"gnome-sharp-2.24.1"
"monodevelop-2.4.2"
"monodevelop-debugger-mdb-2.4"
"monodevelop-debugger-gdb-2.4"
"monodevelop-database-2.4"
)

URLS=("http://ftp.novell.com/pub/mono/sources/mono-addins/mono-addins-0.5.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/mono-debugger/mono-debugger-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/mono-tools/mono-tools-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/gnome-sharp2/gnome-sharp-2.24.1.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/monodevelop/monodevelop-2.4.2.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-mdb/monodevelop-debugger-mdb-2.4.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-gdb/monodevelop-debugger-gdb-2.4.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/monodevelop-database/monodevelop-database-2.4.tar.bz2"
)

echo Downloading
count=${#PACKAGES[@]}
index=0
while [ "$index" -lt "$count" ]
do
	#only download it if you don't already have it. 
	if [ ! -f "${PACKAGES[$index]}.tar" ]
	then
		wget "${URLS[@]:$index:1}"
	fi
	if [ -f "${PACKAGES[$index]}.tar.bz2" ]
	then
		bunzip2 -df "${PACKAGES[$index]}.tar.bz2"
	fi
	if [ -f "${PACKAGES[$index]}.tar" ]
	then
		tar -xvf "${PACKAGES[$index]}.tar"
	fi
	
	let "index = $index + 1"
done


echo
echo "building mono packages"
echo

for i in "${PACKAGES[@]}"
do
	cd $BUILDDIR/$i
	./configure --prefix=$PREFIX
	make
	sudo make install
done

cd $BUILDDIR
echo
echo "done"


