#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-master

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext-libs autoconf httpd httpd-devel libtool wget libtiff-devel libexif-devel libexif libjpeg-devel gtk2-devel atk-devel pango-devel giflib-devel libglade2-devel gettext-devel

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

PROJECTS=("mono"
"libgdiplus"
"gtk-sharp"
"xsp"
"mod_mono")

SOURCES=("git://github.com/mono/mono.git"
"git://github.com/mono/libgdiplus.git"
"git://github.com/mono/gtk-sharp.git"
"git://github.com/mono/xsp.git"
"git://github.com/mono/mod_mono.git")


cd $BUILDDIR

echo Cloning projects
for i in "${SOURCES[@]}"
do
git clone $i
done

echo
echo "building mono packages"
echo

for i in "${PROJECTS[@]}"
do
	cd $BUILDDIR/$i
	./autogen.sh --prefix=$PREFIX
	make
	
	if [ "$i" = ${PROJECTS[0]} ]
	then
		sudo make install
	fi
done

echo
echo "installing mono packages"
echo

for i in "${PROJECTS[@]:1}"
do
	cd $BUILDDIR/$i
	sudo make install
done

cd $BUILDDIR
echo
echo "done"


