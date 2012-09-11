#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-2.11

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext-libs autoconf httpd httpd-devel libtool libtiff-devel libexif-devel libexif libjpeg-devel gtk2-devel atk-devel pango-devel giflib-devel libglade2-devel gettext-devel

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

PACKAGES=("mono-2.11.3"
"libgdiplus-2.10.9"
"gtk-sharp-2.12.11"
"xsp-2.10.2"
"mod_mono-2.10")

URLS=("http://download.mono-project.com/sources/mono/mono-2.11.3.tar.bz2"
"http://download.mono-project.com/sources/libgdiplus/libgdiplus-2.10.9.tar.bz2"
"http://download.mono-project.com/sources/gtk-sharp212/gtk-sharp-2.12.11.tar.bz2"
"http://download.mono-project.com/sources/xsp/xsp-2.10.2.tar.bz2"
"http://download.mono-project.com/sources/mod_mono/mod_mono-2.10.tar.bz2")



echo Downloading
count=${#PACKAGES[@]}
index=0
while [ "$index" -lt "$count" ]
do
	#only download it if you don't already have it. 
	if [ ! -f "${PACKAGES[$index]}.tar" -a  ! -f "${PACKAGES[$index]}.tar.gz" ]
	then
		curl -O "${URLS[@]:$index:1}"
	fi

	#extract
	if [ -f "${PACKAGES[$index]}.tar.gz" ]
	then
		tar -zxvf "${PACKAGES[$index]}.tar.gz"
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
	
	if [ "$i" = ${PACKAGES[0]} ]
	then
		make install
	fi
done

echo
echo "installing mono packages"
echo

for i in "${PACKAGES[@]:1}"
do
	cd $BUILDDIR/$i
	make install
done

cd $BUILDDIR
echo
echo "done"


